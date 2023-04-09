using System.ComponentModel.DataAnnotations;
using webAPIAutores.Validaciones;

namespace webAPIAutores.Entidades;

public class Autor
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El campo {0} es requerido")]
    [StringLength(maximumLength: 4, ErrorMessage = "El campo {0} no debe tener mas de {1} caracteres")]    
    [PrimeraLetraMayuscula]
    public string Nombre { get; set; }
}