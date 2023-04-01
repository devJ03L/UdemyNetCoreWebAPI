using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using webAPIAutores.Validaciones;

namespace webAPIAutores.Entidades;
public class Autor
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El campo {0} es requerido")]
    [StringLength(maximumLength: 4, ErrorMessage = "El campo {0} no debe tener mas de {1} caracteres")]
    [PrimeraLetraMayuscula]
    public string Nombre { get; set; }
    
    [Range(0,120)]
    [NotMapped]
    public int Edad { get; set; }

    [NotMapped]
    //[CreditCard]
    public string TarjetaCredito { get; set; }

    [Url]
    [NotMapped]
    public string URL { get; set; }
    public List<Libro> Libros { get; set; }
}