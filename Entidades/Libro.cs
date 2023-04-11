using System.ComponentModel.DataAnnotations;
using webAPIAutores.Validaciones;

namespace webAPIAutores.Entidades;

public class Libro
{
    public int Id { get; set; }

    [Required]
    [StringLength(maximumLength: 250, ErrorMessage = "El campo {0} no debe tener mas de {1} caracteres")]    
    [PrimeraLetraMayuscula]
    public string Titulo { get; set; }

    public List<Comentario> Comentarios { get; set; }   
    
    public List<AutorLibro> AutoresLibros { get; set; }
}