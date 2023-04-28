using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webAPIAutores.DTOs;
using webAPIAutores.Entidades;

namespace webAPIAutores.Controllers;

[ApiController]
[Route("api/libros")]
public class LibrosController : ControllerBase
{
    private readonly ApplicationDbContext context;
    private readonly IMapper mapper;

    public LibrosController(
        ApplicationDbContext _context,
        IMapper _mapper)
    {
        context = _context;
        mapper = _mapper;
    }

    [HttpGet("{id:int}", Name = "obtenerLibro")]
    public async Task<ActionResult<LibroConAutoresDTO>> Get(int id)
    {
        var libro =
            await context.Libros
            .Include(libroDB => libroDB.AutoresLibros)
            .ThenInclude(autorLibroDB => autorLibroDB.Autor)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (libro == null)
            return NotFound();

        libro.AutoresLibros = libro.AutoresLibros.OrderBy(x => x.Orden).ToList();

        return mapper.Map<LibroConAutoresDTO>(libro);
    }

    [HttpPost(Name = "crearLibro")]
    public async Task<ActionResult> Post(LibroCreacionDTO librocreacionDTO)
    {
        if (librocreacionDTO.AutoresIds == null)
            return BadRequest("No se puede crear un libro sin autores");

        var autores =
             context.Autores
            .Where(autorBD => librocreacionDTO.AutoresIds.Contains(autorBD.Id))
            .Select(x => x.Id);

        if (librocreacionDTO.AutoresIds.Count != autores.Count())
            return BadRequest("Alguno de los autores no existe");

        var libro = mapper.Map<Libro>(librocreacionDTO);
        AsignarOrdenAutores(libro);

        context.Add(libro);
        await context.SaveChangesAsync();

        var libroDTO = mapper.Map<LibroDTO>(libro);
        return CreatedAtRoute("öbtenerLibro", new { id = libro.Id }, libroDTO);
    }

    [HttpPut("{id:int}", Name = "actualizarLibro")]
    public async Task<ActionResult> Put(int id, LibroCreacionDTO libroCreacionDTO)
    {
        var libroDB =
            await context.Libros
            .Include(x => x.AutoresLibros)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (libroDB == null)
            return NotFound();

        libroDB = mapper.Map(libroCreacionDTO, libroDB);
        AsignarOrdenAutores(libroDB);

        await context.SaveChangesAsync();
        return NoContent();
    }

    [HttpPatch("{id:int}", Name = "patchLibro")]
    public async Task<ActionResult> Patch(int id, JsonPatchDocument<LibroPatchDTO> patchDocument)
    {
        if (patchDocument == null)
            return BadRequest();

        var libroDB = await context.Libros.FirstOrDefaultAsync(libro => libro.Id == id);
        if (libroDB == null)
            return NotFound();

        var libroDTO = mapper.Map<LibroPatchDTO>(libroDB);
        patchDocument.ApplyTo(libroDTO, ModelState);

        var esValido = TryValidateModel(libroDTO);
        if (!esValido)
            return BadRequest(ModelState);

        mapper.Map(libroDTO, libroDB);
        await context.SaveChangesAsync();
        return NoContent();
    }


    [HttpDelete("{id:int}", Name = "borrarLibro")]
    public async Task<ActionResult> Delete(int id)
    {
        var existe = await context.Libros.AnyAsync(x => x.Id == id);

        if (!existe)
            return NotFound();

        context.Remove(new Libro() { Id = id });
        await context.SaveChangesAsync();

        return NoContent();
    }

    private void AsignarOrdenAutores(Libro libro)
    {
        if (libro.AutoresLibros != null)
            for (int i = 0; i < libro.AutoresLibros.Count; i++)
                libro.AutoresLibros[i].Orden = i;
    }
}