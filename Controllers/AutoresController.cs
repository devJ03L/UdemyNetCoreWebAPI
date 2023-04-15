using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webAPIAutores.DTOs;
using webAPIAutores.Entidades;

namespace webAPIAutores.Controllers;

[ApiController]
[Route("api/autores")]
public class AutoresController : ControllerBase
{
    private readonly ApplicationDbContext context;
    private readonly IMapper mapper;
    private readonly IConfiguration configuration;

    public AutoresController(
        ApplicationDbContext _context,
        IMapper _mapper,
        IConfiguration _configuration)
    {
        context = _context;
        mapper = _mapper;
        configuration = _configuration;
    }

    [HttpGet]   //   /api/autores    
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<ActionResult<List<AutorDTO>>> Get()
    {
        var autores = await context.Autores.ToListAsync();
        return mapper.Map<List<AutorDTO>>(autores);
    }

    [HttpGet("{id:int}", Name = "obtenerAutor")]
    public async Task<ActionResult<AutorConLibrosDTO>> Get(int id)
    {
        var autor =
            await context.Autores
            .Include(autorDB => autorDB.AutoresLibros)
            .ThenInclude(autorLibro => autorLibro.Libro)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (autor == null)
            return NotFound();

        return mapper.Map<AutorConLibrosDTO>(autor);
    }

    [HttpGet("{nombre}")]
    public async Task<ActionResult<List<AutorDTO>>> Get(string nombre)
    {
        var autores = await context.Autores.Where(x => x.Nombre.Contains(nombre)).ToListAsync();

        return mapper.Map<List<AutorDTO>>(autores);
    }

    [HttpPost]
    public async Task<ActionResult> Post(AutorCreacionDTO autorCreacionDTO)
    {
        var existeAutor = await context.Autores.AnyAsync(x => x.Nombre == autorCreacionDTO.Nombre);
        if (existeAutor)
            return BadRequest($"Ya existe un autor con el nombre {autorCreacionDTO.Nombre}");

        var autor = mapper.Map<Autor>(autorCreacionDTO);
        context.Add(autor);
        await context.SaveChangesAsync();

        var autorDTO = mapper.Map<AutorDTO>(autor);
        return CreatedAtRoute("obtenerAutor", new { id = autor.Id }, autorDTO);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> Put(AutorCreacionDTO autorCreacion, int id)
    {
        var existe = await context.Autores.AnyAsync(aut => aut.Id == id);
        if (!existe)
            return NotFound();

        var autor = mapper.Map<Autor>(autorCreacion);
        autor.Id = id;

        context.Update(autor);
        await context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id)
    {
        var existe = await context.Autores.AnyAsync(x => x.Id == id);

        if (!existe)
            return NotFound();

        context.Remove(new Autor() { Id = id });
        await context.SaveChangesAsync();

        return NoContent();
    }
}