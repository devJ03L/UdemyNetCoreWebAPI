using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using webAPIAutores.DTOs;

namespace webAPIAutores.Controllers;

[ApiController]
[Route("api")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class RootController : ControllerBase
{
    private readonly IAuthorizationService authorizationService;

    public RootController(IAuthorizationService _authorizationService)
    {
        authorizationService = _authorizationService;
    }

    [HttpGet(Name = "ObtenerRoot")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<DatoHATEOAS>>> Get()
    {
        var esAdmin = await authorizationService.AuthorizeAsync(User, "esAdmin");

        var datosHateoas = new List<DatoHATEOAS>();

        datosHateoas.Add(new DatoHATEOAS(
            enlace: Url.Link("ObtenerRoot", new { }),
            descripcion: "self",
            metodo: "GET"));

        datosHateoas.Add(new DatoHATEOAS(
            enlace: Url.Link("obtenerAutores", new { }),
            descripcion: "autores",
            metodo: "GET"));

        if (esAdmin.Succeeded)
        {
            datosHateoas.Add(new DatoHATEOAS(
                enlace: Url.Link("crearAutor", new { }),
                descripcion: "autor-crear",
                metodo: "POST"));

            datosHateoas.Add(new DatoHATEOAS(
                enlace: Url.Link("crearLibro", new { }),
                descripcion: "libro-crear",
                metodo: "POST"));
        }

        return datosHateoas;
    }
}