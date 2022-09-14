using AutoMapper;
using BackEndGeneros.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BackEndGeneros.Controllers
{
    [Route("api/cuentas")]
    [ApiController]
    public class CuentasController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;



        public CuentasController(UserManager<IdentityUser> userManager,
                        IConfiguration configuration,
                        SignInManager<IdentityUser> signInManager,
                        ApplicationDbContext context,
                        IMapper mapper)
        {
            this._userManager = userManager;
            this._configuration = configuration;
            this._signInManager = signInManager;
            this._context = context;
            this._mapper = mapper;
        }

        [HttpPost("crear")]
        public async Task<ActionResult<RespuestaAutenticacion>> Crear([FromBody] CredencialesUsuario credenciales)
        {
            var usuario = new  IdentityUser { UserName = credenciales.Email, Email = credenciales.Email};
            var resultado = await _userManager.CreateAsync(usuario, credenciales.Password);

            if (resultado.Succeeded)
            {
                return await ConstruirToken(credenciales);
            }
            else 
            {
                return BadRequest();
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<RespuestaAutenticacion>> Login([FromBody] CredencialesUsuario credenciales)
        {
            var resultado = await _signInManager.PasswordSignInAsync(credenciales.Email, credenciales.Password,
                isPersistent: false, lockoutOnFailure: false);

            if (resultado.Succeeded)
            {
                return await ConstruirToken(credenciales);
            }
            else
            {
                return BadRequest("Login incorrecto");
            }
        }



        private async Task<RespuestaAutenticacion> ConstruirToken(CredencialesUsuario credenciales) 
        {
            var claims = new List<Claim>()
            {
                new Claim("email", credenciales.Email)
            };

            var usuario = await _userManager.FindByEmailAsync(credenciales.Email);
            var claimsDB = await _userManager.GetClaimsAsync(usuario);

            claims.AddRange(claimsDB);

            var llave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["llavejwt"]));
            var creds = new SigningCredentials(llave, SecurityAlgorithms.HmacSha256);

            var expiracion = DateTime.UtcNow.AddYears(1);

            var token = new JwtSecurityToken(issuer: null, audience: null, claims: claims,
               expires: expiracion, signingCredentials: creds);

            return new RespuestaAutenticacion()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiracion = expiracion
            };

        }
    }
}
