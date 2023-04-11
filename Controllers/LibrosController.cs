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

    [HttpGet("{id:int}")]
    public async Task<ActionResult<LibroDTO>> Get(int id)
    {
        var libro = 
            await context.Libros
            .Include(x => x.Comentarios)
            .FirstOrDefaultAsync(x => x.Id == id);
        
        return mapper.Map<LibroDTO>(libro);
    }

    [HttpPost]
    public async Task<ActionResult> Post(LibroCreacionDTO librocreacionDTO)
    {
        //asadadad var existeAutor = await context.Autores.AnyAsync(x => x.Id == libro.AutorId);
        // if (!existeAutor)
        // return BadRequest($"No existe el autor con el id: {libro.AutorId}");
        var libro = mapper.Map<Libro>(librocreacionDTO);
        context.Add(libro);
        await context.SaveChangesAsync();
        return Ok();
    }
}