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
    private readonly IServicio servicio;
    private readonly ServicioTransient servicioTransient;
    private readonly ServicioScoped servicioScoped;
    private readonly ServicioSingleton servicioSingleton;
    private readonly ILogger<AutoresController> logger;

    public AutoresController(
        ApplicationDbContext _context, 
        IServicio _servicio,
        ServicioTransient _servicioTransient,
        ServicioScoped _servicioScoped,
        ServicioSingleton _servicioSingleton,
        ILogger<AutoresController> _logger
        )
    {
        context = _context;
        servicio = _servicio;
        servicioTransient = _servicioTransient;
        servicioScoped = _servicioScoped;
        servicioSingleton = _servicioSingleton;
        logger = _logger;
    }

    [HttpGet("GUID")]
    //[ResponseCache(Duration = 10)]
    [ServiceFilter(typeof(MiFiltroDeAccion))]
    public ActionResult ObtenerGuids()
    {
        return Ok(new {
            AutoresControllerTransient = servicioTransient.Guid,
            ServicioA_Transient = servicio.ObtenerTransient(),
            AutoresControllerScoped = servicioScoped.Guid,
            ServicioA_Scoped = servicio.ObtenerScoped(),
            AutoresControllerSingleton = servicioSingleton.Guid,
            ServicioA_Singleton = servicio.ObtenerSingleton()
        });
    }

    [HttpGet]               //   /api/autores
    [HttpGet("listado")]    //   /api/autores/listado
    [HttpGet("/listado")]   //   /listado
    //[ResponseCache(Duration = 10)]
    [ServiceFilter(typeof(MiFiltroDeAccion))]
    public async Task<ActionResult<List<Autor>>> Get()
    {
        //throw new NotImplementedException();
        logger.LogInformation("Obteniendo los autores");
        logger.LogWarning("Prueba warning");
        return await context.Autores.Include(x => x.Libros).ToListAsync();
    }

    [HttpGet("primero")]
    public async Task<ActionResult<Autor>> PrimerAutor(
        [FromHeader]int miValor,
        [FromQuery]string nombre)
    {
        return await context.Autores.FirstOrDefaultAsync();
    }

    [HttpGet("{id:int}/{param2=sultan}")]
    public async Task<ActionResult<Autor>> Get(int id, string param2)
    {
        Console.WriteLine(param2);
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