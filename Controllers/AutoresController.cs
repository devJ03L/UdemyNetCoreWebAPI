using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webAPIAutores.DTOs;
using webAPIAutores.Entidades;
using webAPIAutores.Filtros;
using webAPIAutores.Servicios;

namespace webAPIAutores.Controllers;

[ApiController]
[Route("api/autores")]
public class AutoresController : ControllerBase
{
    private readonly ApplicationDbContext context;
    private readonly IMapper mapper;

    public AutoresController(
        ApplicationDbContext _context,
        IMapper _mapper)
    {
        context = _context;
        mapper = _mapper;
    }

    [HttpGet]   //   /api/autores    
    public async Task<ActionResult<List<AutorDTO>>> Get()
    {
        var autores = await context.Autores.ToListAsync();
        return mapper.Map<List<AutorDTO>>(autores);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<AutorDTO>> Get(int id)
    {
        var autor = 
            await context.Autores
            .Include(autorDB => autorDB.AutoresLibros)
            .ThenInclude(autorLibro => autorLibro.Libro)
            .FirstOrDefaultAsync(x => x.Id == id);
            
        if (autor == null)
            return NotFound();

        return mapper.Map<AutorDTO>(autor);
    }

    [HttpGet("{nombre}")]
    public async Task<ActionResult<List<AutorDTO>>> Get(string nombre)
    {
        var autores = await context.Autores.Where(x => x.Nombre.Contains(nombre)).ToListAsync();
        
        return mapper.Map<List<AutorDTO>>(autores);
    }

    [HttpPost]
    public async Task<ActionResult> Post(AutorCreacionDTO autorDTO)
    {
        var existeAutor = await context.Autores.AnyAsync(x => x.Nombre == autorDTO.Nombre);
        if (existeAutor)
            return BadRequest($"Ya existe un autor con el nombre {autorDTO.Nombre}");

        var autor =  mapper.Map<Autor>(autorDTO);
        context.Add(autor);
        await context.SaveChangesAsync();
        return Ok();
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> Put(Autor autor, int id)
    {
        if (autor.Id != id)
            return BadRequest("Ël id del autor no coincide con el id de la URL");

        context.Update(autor);
        await context.SaveChangesAsync();

        return Ok();
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id)
    {
        var existe = await context.Autores.AnyAsync(x => x.Id == id);

        if (!existe)
            return NotFound();

        context.Remove(new Autor() { Id = id });
        await context.SaveChangesAsync();

        return Ok();
    }
}