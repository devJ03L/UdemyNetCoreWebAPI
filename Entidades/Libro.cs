using System.ComponentModel.DataAnnotations;
using webAPIAutores.Validaciones;

namespace webAPIAutores.Entidades;

public class Libro
{
    public int Id { get; set; }

    [StringLength(maximumLength: 250, ErrorMessage = "El campo {0} no debe tener mas de {1} caracteres")]    
    [PrimeraLetraMayuscula]
    public string Titulo { get; set; }    
}