using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webAPIAutores.DTOs;
using webAPIAutores.Entidades;

namespace webAPIAutores.Controllers;

[ApiController]
[Route("api/libros/{libroId:int}/comentarios")]
public class ComentariosController : ControllerBase
{
    private readonly ApplicationDbContext context;
    private readonly IMapper mapper;
    private readonly UserManager<IdentityUser> userManager;

    public ComentariosController(
        ApplicationDbContext _context,
        IMapper _mapper,
        UserManager<IdentityUser> _userManager)
    {
        context = _context;
        mapper = _mapper;
        userManager = _userManager;
    }

    [HttpGet]
    public async Task<ActionResult<List<ComentarioDTO>>> Get(int libroId)
    {
        var existeLibro = await context.Libros.AnyAsync(libroDB => libroDB.Id == libroId);
        if (!existeLibro)
            return NotFound();

        var comentarios = await context.Comentarios.Where(c => c.LibroId == libroId).ToListAsync();
        return mapper.Map<List<ComentarioDTO>>(comentarios);
    }

    [HttpGet("{id:int}", Name = "obtenerComentario")]
    public async Task<ActionResult<ComentarioDTO>> GetById(int id)
    {
        var existComentario = await context.Comentarios.AnyAsync(comentario => comentario.Id == id);
        if (!existComentario)
            return NotFound();

        var comentario = await context.Comentarios.FirstOrDefaultAsync(coment => coment.Id == id);
        return mapper.Map<ComentarioDTO>(comentario);
    }

    [HttpPost]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<ActionResult> Post(int libroId, ComentarioCreacionDTO comentarioCreacionDTO)
    {        
        var existeLibro = await context.Libros.AnyAsync(libroDB => libroDB.Id == libroId);
        if (!existeLibro)
            return NotFound();

        var emailClaim = HttpContext.User.Claims.Where(claim => claim.Type == "email").FirstOrDefault();
        var usuario = await userManager.FindByEmailAsync(emailClaim.Value);
        var comentario = mapper.Map<Comentario>(comentarioCreacionDTO);
        comentario.UsuarioId = usuario.Id;
        comentario.LibroId = libroId;

        context.Add(comentario);
        await context.SaveChangesAsync();

        var comentarioDTO = mapper.Map<ComentarioDTO>(comentario);
        return CreatedAtRoute("obtenerComentario", new{id = comentario.Id, libroId = libroId}, comentarioDTO);
    }

    
    [HttpPut("{id:int}")]
    public async Task<ActionResult> Put(ComentarioCreacionDTO comentarioCreacion, int id, int libroId)
    {
        var existLibro = await context.Libros.AnyAsync(libro => libro.Id == libroId);
        if (!existLibro)
            return NotFound();

        var existComentario = await context.Comentarios.AnyAsync(comentario => comentario.Id == id);
        if (!existComentario)
            return NotFound();

        var comentario = mapper.Map<Comentario>(comentarioCreacion);
        comentario.Id = id;
        comentario.LibroId = libroId;

        context.Update(comentario);
        await context.SaveChangesAsync();

        return NoContent();
    }
}