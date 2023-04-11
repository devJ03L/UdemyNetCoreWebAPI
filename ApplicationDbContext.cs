using Microsoft.EntityFrameworkCore;
using webAPIAutores.Entidades;

namespace webAPIAutores;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions options) : base(options)
    {

    }

    public DbSet<Autor> Autores { get; set; }
    public DbSet<Libro> Libros { get; set; }
    public DbSet<Comentario> Comentarios { get; set; }
}