using System.ComponentModel.DataAnnotations;
using webAPIAutores.Validaciones;

namespace webAPIAutores.DTOs;

public class AutorConLibrosDTO : AutorDTO
{ 
    public List<LibroDTO> Libros { get; set; }
}