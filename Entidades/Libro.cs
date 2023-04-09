using webAPIAutores.Validaciones;

namespace webAPIAutores.Entidades;

public class Libro
{
    public int Id { get; set; }

    [PrimeraLetraMayuscula]
    public string Titulo { get; set; }    
}