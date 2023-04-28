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
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "EsAdmin")]
public class AutoresController : ControllerBase
{
    private readonly ApplicationDbContext context;
    private readonly IMapper mapper;
    private readonly IConfiguration configuration;
    private readonly IAuthorizationService authorizationService;

    public AutoresController(
        ApplicationDbContext _context,
        IMapper _mapper,
        IConfiguration _configuration,
        IAuthorizationService _authorizationService)
    {
        context = _context;
        mapper = _mapper;
        configuration = _configuration;
        authorizationService = _authorizationService;
    }

    [HttpGet(Name = "obtenerAutores")]   //   /api/autores    
    [AllowAnonymous]
    public async Task<ActionResult<ColeccionDeRecursos<AutorDTO>>> Get()
    {
        var autores = await context.Autores.ToListAsync();
        var dtos = mapper.Map<List<AutorDTO>>(autores);

        var esAdmin = await authorizationService.AuthorizeAsync(User, "esAdmin");

        dtos.ForEach(dto => GenerarEnlaces(dto, esAdmin.Succeeded));

        var resultado = new ColeccionDeRecursos<AutorDTO> { Valores = dtos };

        resultado.Enlaces.Add(new DatoHATEOAS(
            enlace: Url.Link("obtenerAutores", new { }),
            descripcion: "self",
            metodo: "GET"
        ));

        if (esAdmin.Succeeded)
            resultado.Enlaces.Add(new DatoHATEOAS(
                enlace: Url.Link("crearAutor", new { }),
                descripcion: "crear-autor",
                metodo: "POST"
            ));

        return resultado;
    }

    [HttpGet("{id:int}", Name = "obtenerAutor")]
    [AllowAnonymous]
    public async Task<ActionResult<AutorConLibrosDTO>> Get(int id)
    {
        var autor =
            await context.Autores
            .Include(autorDB => autorDB.AutoresLibros)
            .ThenInclude(autorLibro => autorLibro.Libro)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (autor == null)
            return NotFound();
        var esAdmin = await authorizationService.AuthorizeAsync(User, "esAdmin");
        var dto = mapper.Map<AutorConLibrosDTO>(autor);
        GenerarEnlaces(dto, esAdmin.Succeeded);

        return dto;
    }

    [HttpGet("{nombre}", Name = "obtenerAutorPorNombre")]
    public async Task<ActionResult<List<AutorDTO>>> Get(string nombre)
    {
        var autores = await context.Autores.Where(x => x.Nombre.Contains(nombre)).ToListAsync();

        return mapper.Map<List<AutorDTO>>(autores);
    }

    [HttpPost(Name = "crearAutor")]
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

    [HttpPut("{id:int}", Name = "actualizarAutor")]
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

    [HttpDelete("{id:int}", Name = "borrarAutor")]
    public async Task<ActionResult> Delete(int id)
    {
        var existe = await context.Autores.AnyAsync(x => x.Id == id);

        if (!existe)
            return NotFound();

        context.Remove(new Autor() { Id = id });
        await context.SaveChangesAsync();

        return NoContent();
    }

    private void GenerarEnlaces(AutorDTO autorDTO, bool esAdmin)
    {
        autorDTO.Enlaces.Add(new DatoHATEOAS(
            enlace: Url.Link("obtenerAutor", new { id = autorDTO.Id }),
            descripcion: "self",
            metodo: "GET"
        ));

        if (esAdmin)
        {
            autorDTO.Enlaces.Add(new DatoHATEOAS(
                enlace: Url.Link("actualizarAutor", new { id = autorDTO.Id }),
                descripcion: "autor-actualizar",
                metodo: "PUT"
            ));

            autorDTO.Enlaces.Add(new DatoHATEOAS(
                enlace: Url.Link("borrarAutor", new { id = autorDTO.Id }),
                descripcion: "self",
                metodo: "DELETE"
            ));
        }

    }
}