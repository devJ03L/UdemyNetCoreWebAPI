using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using webAPIAutores.DTOs;

namespace webAPIAutores.Controllers;

[ApiController]
[Route("api/cuentas")]
public class CuentasController : ControllerBase
{
    private readonly UserManager<IdentityUser> userManager;
    private readonly IConfiguration configuration;
    private readonly SignInManager<IdentityUser> signInManager;

    public CuentasController(
        UserManager<IdentityUser> _userManager,
        IConfiguration _configuration,
        SignInManager<IdentityUser> _signInManager
        )
    {
        userManager = _userManager;
        configuration = _configuration;
        signInManager = _signInManager;
    }

    [HttpPost("registrar")]
    public async Task<ActionResult<RespuestaAutenticacion>> Registrar(CredencialesUsuario credencialesUsuario)
    {
        var usuario = new IdentityUser
        {
            UserName = credencialesUsuario.Email,
            Email = credencialesUsuario.Email
        };

        var resultado = await userManager.CreateAsync(usuario, credencialesUsuario.Password);
        if (!resultado.Succeeded)
            return BadRequest(resultado.Errors);
        return ConstruirToken(credencialesUsuario);
    }

    [HttpPost("login")]
    public async Task<ActionResult<RespuestaAutenticacion>> Login(CredencialesUsuario credencialesUsuario)
    {
        var resultado = await signInManager.PasswordSignInAsync(credencialesUsuario.Email, credencialesUsuario.Password, isPersistent: false, lockoutOnFailure: false);
        if (!resultado.Succeeded)
            return BadRequest("Login incorrecto");
        return ConstruirToken(credencialesUsuario);
    }

    private RespuestaAutenticacion ConstruirToken(CredencialesUsuario credencialesUsuario)
    {
        var claims = new List<Claim>()
        {
            new Claim("email", credencialesUsuario.Email),
            new Claim("lo que yo quiera", "cualquier valor")
        };

        var llave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["llavejwt"]));
        var creds = new SigningCredentials(llave, SecurityAlgorithms.HmacSha256);
        var expiracion = DateTime.UtcNow.AddYears(1);
        var securityToken = new JwtSecurityToken(issuer: null, audience: null, claims: claims, expires: expiracion, signingCredentials: creds);

        return new RespuestaAutenticacion()
        {
            Token = new JwtSecurityTokenHandler().WriteToken(securityToken),
            Expiracion = expiracion
        };
    }
}