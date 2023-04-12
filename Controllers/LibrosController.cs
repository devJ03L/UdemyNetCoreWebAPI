using AutoMapper;
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

    [HttpGet("{id:int}", Name = "öbtenerLibro")]
    public async Task<ActionResult<LibroConAutoresDTO>> Get(int id)
    {
        var libro =
            await context.Libros
            .Include(libroDB => libroDB.AutoresLibros)
            .ThenInclude(autorLibroDB => autorLibroDB.Autor)
            .FirstOrDefaultAsync(x => x.Id == id);

        libro.AutoresLibros = libro.AutoresLibros.OrderBy(x => x.Orden).ToList();

        return mapper.Map<LibroConAutoresDTO>(libro);
    }

    [HttpPost]
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

        if (libro.AutoresLibros != null)
            for (int i = 0; i < libro.AutoresLibros.Count; i++)
                libro.AutoresLibros[i].Orden = i;

        context.Add(libro);
        await context.SaveChangesAsync();

        var libroDTO = mapper.Map<LibroDTO>(libro);
        return CreatedAtRoute("öbtenerLibro", new { id = libro.Id }, libroDTO);
    }
}