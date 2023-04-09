using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webAPIAutores.Entidades;
using webAPIAutores.Filtros;
using webAPIAutores.Servicios;

namespace webAPIAutores.Controllers;

[ApiController]
[Route("api/autores")]
public class AutoresController : ControllerBase
{
    private readonly ApplicationDbContext context;

    public AutoresController(ApplicationDbContext _context)
    {
        context = _context;        
    }    

    [HttpGet]   //   /api/autores    
    public async Task<ActionResult<List<Autor>>> Get()
    {                
        return await context.Autores.ToListAsync();
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Autor>> Get(int id)
    {        
        var autor = await context.Autores.FirstOrDefaultAsync(x => x.Id == id);
        if (autor == null)
            return NotFound();
        return autor;
    }

    [HttpGet("{nombre}")]
    public async Task<ActionResult<Autor>> Get(string nombre)
    {
        var autor = await context.Autores.FirstOrDefaultAsync(x => x.Nombre.Contains(nombre));
        if (autor == null)
            return NotFound();
        return autor;
    }

    [HttpPost]
    public async Task<ActionResult> Post(Autor autor)
    {
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