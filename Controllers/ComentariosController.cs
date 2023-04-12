using AutoMapper;
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

    public ComentariosController(
        ApplicationDbContext _context,
        IMapper _mapper)
    {
        context = _context;
        mapper = _mapper;
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
    public async Task<ActionResult> Post(int libroId, ComentarioCreacionDTO comentarioCreacionDTO)
    {
        var existeLibro = await context.Libros.AnyAsync(libroDB => libroDB.Id == libroId);
        if (!existeLibro)
            return NotFound();

        var comentario = mapper.Map<Comentario>(comentarioCreacionDTO);
        comentario.LibroId = libroId;

        context.Add(comentario);
        await context.SaveChangesAsync();

        var comentarioDTO = mapper.Map<ComentarioDTO>(comentario);
        return CreatedAtRoute("obtenerComentario", new{id = comentario.Id, libroId = libroId}, comentarioDTO);
    }
}