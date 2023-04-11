using AutoMapper;
using webAPIAutores.DTOs;
using webAPIAutores.Entidades;

namespace webAPIAutores.Utilidades;

public class AutoMapperProfiles : Profile
{
    public AutoMapperProfiles()
    {
        CreateMap<AutorCreacionDTO, Autor>();
        CreateMap<Autor, AutorDTO>();
        CreateMap<LibroCreacionDTO, Libro>()
            .ForMember(
                libro => libro.AutoresLibros,
                opciones => opciones.MapFrom(MapAutoresLibros)
            );
        CreateMap<Libro, LibroDTO>();
        CreateMap<ComentarioCreacionDTO, Comentario>();
        CreateMap<Comentario, ComentarioDTO>();
    }

    private List<AutorLibro> MapAutoresLibros(LibroCreacionDTO libroCreacionDTO, Libro libro)
    {
        var resultado = new List<AutorLibro>();
        if (libroCreacionDTO.AutoresIds == null)
            return resultado;

        foreach (var autorId in libroCreacionDTO.AutoresIds)
            resultado.Add(new AutorLibro() { AutorId = autorId, Orden = autorId });
        
        return resultado;
    }
}