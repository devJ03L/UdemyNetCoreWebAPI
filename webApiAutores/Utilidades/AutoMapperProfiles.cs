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

        CreateMap<Autor, AutorConLibrosDTO>()
            .ForMember(
                autor => autor.Libros,
                opciones => opciones.MapFrom(MapLibros)
            );

        CreateMap<LibroCreacionDTO, Libro>()
            .ForMember(
                libro => libro.AutoresLibros,
                opciones => opciones.MapFrom(MapAutoresLibros)
            );

        CreateMap<Libro, LibroDTO>().ReverseMap();
        CreateMap<LibroPatchDTO, Libro>().ReverseMap();

        CreateMap<Libro, LibroConAutoresDTO>()
            .ForMember(
                libro => libro.Autores,
                opciones => opciones.MapFrom(MapAutores)
            );

        CreateMap<ComentarioCreacionDTO, Comentario>();

        CreateMap<Comentario, ComentarioDTO>();
    }

    private List<AutorLibro> MapAutoresLibros(LibroCreacionDTO libroCreacionDTO, Libro libro)
    {
        var resultado = new List<AutorLibro>();
        if (libroCreacionDTO.AutoresIds == null)
            return resultado;

        libroCreacionDTO.AutoresIds.ForEach(autorId => resultado.Add(new AutorLibro() { AutorId = autorId }));

        return resultado;
    }

    private List<AutorDTO> MapAutores(Libro libro, LibroConAutoresDTO libroDTO)
    {
        var resultado = new List<AutorDTO>();

        if (libro.AutoresLibros == null)
            return resultado;

        foreach (var autorLibro in libro.AutoresLibros)
            resultado.Add(new AutorDTO() { Id = autorLibro.AutorId, Nombre = autorLibro.Autor.Nombre });

        return resultado;
    }

    private List<LibroDTO> MapLibros(Autor autor, AutorConLibrosDTO autorConLibrosDTO)
    {
        var resultado = new List<LibroDTO>();

        if (autor.AutoresLibros == null)
            return resultado;

        foreach (var libroAutor in autor.AutoresLibros)
            resultado.Add(new LibroDTO() { Id = libroAutor.LibroId, Titulo = libroAutor.Libro.Titulo });

        return resultado;
    }
}