using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.HttpSys;
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

    [HttpPost("registrar", Name = "registrarUsuario")]
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
        return await ConstruirToken(credencialesUsuario);
    }

    [HttpPost("login", Name = "loginUsuario")]
    public async Task<ActionResult<RespuestaAutenticacion>> Login(CredencialesUsuario credencialesUsuario)
    {
        var resultado = await signInManager.PasswordSignInAsync(credencialesUsuario.Email, credencialesUsuario.Password, isPersistent: false, lockoutOnFailure: false);
        if (!resultado.Succeeded)
            return BadRequest("Login incorrecto");
        return await ConstruirToken(credencialesUsuario);
    }

    [HttpGet("RenovarToken", Name = "renovarToken")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<ActionResult<RespuestaAutenticacion>> Renovar()
    {
        var email = HttpContext.User.Claims.Where(claim => claim.Type == "email").FirstOrDefault().Value;
        var credencialesUsuario = new CredencialesUsuario()
        {
            Email = email
        };
        return await ConstruirToken(credencialesUsuario);
    }

    [HttpPost("HacerAdmin", Name = "hacerAdmin")]
    public async Task<ActionResult> HacerAdmin(EditarAdminDTO editarAdminDTO)
    {
        var usuario = await userManager.FindByEmailAsync(editarAdminDTO.Email);
        await userManager.AddClaimAsync(usuario, new Claim("esAdmin", "1"));
        return NoContent();
    }

    [HttpPost("RemoverAdmin", Name = "removerAdmin")]
    public async Task<ActionResult> RemoverAdmin(EditarAdminDTO editarAdminDTO)
    {
        var usuario = await userManager.FindByEmailAsync(editarAdminDTO.Email);
        await userManager.RemoveClaimAsync(usuario, new Claim("esAdmin", "1"));
        return NoContent();
    }

    private async Task<RespuestaAutenticacion> ConstruirToken(CredencialesUsuario credencialesUsuario)
    {
        var claims = new List<Claim>()
        {
            new Claim("email", credencialesUsuario.Email),
            new Claim("lo que yo quiera", "cualquier valor")
        };

        var usuario = await userManager.FindByEmailAsync(credencialesUsuario.Email);
        var claimsDB = await userManager.GetClaimsAsync(usuario);
        claims.AddRange(claimsDB);


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