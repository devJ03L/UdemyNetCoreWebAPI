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
        CreateMap<LibroCreacionDTO, Libro>();
        CreateMap<Libro, LibroDTO>();
        CreateMap<ComentarioCreacionDTO, Comentario>();
        CreateMap<Comentario, ComentarioDTO>();
    }
}