using System.ComponentModel.DataAnnotations;
using webAPIAutores.Validaciones;

namespace webAPIAutores.DTOs;

public class LibroCreacionDTO
{
    [StringLength(maximumLength: 250, ErrorMessage = "El campo {0} no debe tener mas de {1} caracteres")]
    [PrimeraLetraMayuscula]
    public string Titulo { get; set; }

    public DateTime FechaPublicacion { get; set; }

    public List<int> AutoresIds { get; set; }
}