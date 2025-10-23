using Google.Apis.Auth;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PWAs.Context;
using PWAs.Helper;
using PWAs.Models;
using PWAs.Models.Conexion;
using PWAs.Models.Usuarios;
using PWAs.Models.Geolocalizacion;
using PWAs.Services;
using PWAs.Sistema;
using RTools_NTS.Util;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using NetTopologySuite.Geometries;

namespace PWAs.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorizationController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private MailService _mailSer = new();
        private readonly GeolocalizacionService _gSer;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly GeometryFactory _gFc;

        public static string mailpt1 = "<!DOCTYPE html> <html lang='es'> <head>     <meta charset='UTF-8'>     <meta name='viewport' content='width=device-width, initial-scale=1.0'>     <title>Tu Código de Seguridad</title> </head> <body style='font-family: sans-serif; margin: 0; padding: 20px; background-color: #f4f4f4;'>     <div style='background-color: #ffffff; max-width: 600px; margin: 20px auto; padding: 30px; border-radius: 10px; box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);'>         <h2 style='color: #333; text-align: center; margin-bottom: 30px;'>Tu Código de Seguridad</h2>         <p style='color: #555; line-height: 1.6; margin-bottom: 20px; text-align: center;'>             Este es tu código de seguridad único. Utilízalo para completar el proceso donde se te solicitó.         </p>         <div style='background-color: #e0f7fa; padding: 20px; border-radius: 8px; text-align: center; margin-bottom: 30px;'>             <strong style='font-size: 2.5em; color: #007bff; letter-spacing: 10px;'> ";
        public static string mailpt2 = "</strong>         </div>         <p style='color: #555; line-height: 1.6; margin-bottom: 20px; text-align: center;'>             Por favor, introduce este código en la casilla correspondiente dentro de la aplicación o sitio web.         </p>         <p style='color: #777; font-size: 0.9em; text-align: center;'>             Este código es válido por un tiempo limitado. No lo compartas con nadie.         </p>     </div> </body> </html>";

        public AuthorizationController(AppDbContext context, IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            _gFc = new GeometryFactory(new PrecisionModel(), 4326);
        }

        [AllowAnonymous]
        [HttpPost("GetToken")]
        public async Task<IActionResult> ObtenerToken(string correo)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                var extRes = await client.GetAsync("https://www.google.com");
                extRes.EnsureSuccessStatusCode();

                if (!_mailSer.IsValidMail(correo)) return BadRequest(new { mensaje = "Ingresa un correo valido" });

                var usuario = await _context.tUsuarios.FirstOrDefaultAsync(x => x.SEmail == correo);

                if (usuario == null) return NotFound(new { mensaje = "No se ha encontrado ningun usuario con ese correo." });

                var token = _mailSer.GTokenRec();
                var tokenTemp = new TokenTemp
                {
                    SToken = token,
                    IUsuarioId = usuario.IID,
                    DFechaExpiracion = DateTime.Now.AddMinutes(5)
                };
                _context.tTokensTemp.Add(tokenTemp);
                await _context.SaveChangesAsync();

                string asunto = mailpt1 + token + mailpt2;

                _mailSer.EnviarMail(correo, "Inicio de sesion", asunto);

                return Ok(new { token });
            }
            catch (HttpRequestException exp)
            {
                Console.WriteLine(exp.Message);
                return BadRequest(new { mensaje = "Sin conexion a internet", token = _mailSer.GTokenOff(correo) });
            }
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Authorization login, [FromHeader] string Token)
        {
            if (!ModelState.IsValid) return BadRequest(ErrorHelper.GetModelStateErrors(ModelState));

            if (!_mailSer.IsValidMail(login.Usuario)) return BadRequest(new { mensaje = "Ingresa un correo valido" });

            var sesion = await _context.tSesiones.FirstOrDefaultAsync(x => x.SUsuario == login.Usuario);

            if (sesion != null)
            {
                if (sesion.DExpiracion < DateTime.UtcNow) {
                    _context.tSesiones.Remove(sesion);
                    _context.SaveChanges();
                }
                else return BadRequest(new { mensaje = "Existe una sesion activa en otro dispositivo" });
            }

            var tokenTemp = await _context.tTokensTemp
                                            .FirstOrDefaultAsync(x => x.SToken == Token && x.DFechaExpiracion > DateTime.Now);

            if (tokenTemp == null) return BadRequest(new { mensaje = "Token no valido" });

            Usuario? usuario = await _context.tUsuarios.Include(x => x.Rol).FirstOrDefaultAsync(x => x.SEmail == login.Usuario);

            if (usuario == null) return Unauthorized(ErrorHelper.Response(401, "Username doesn't exist", "Authentication failure"));
            
            if (BCrypt.Net.BCrypt.Verify(login.Clave, usuario.SPasswd))
            {
                var secretKey = _configuration.GetValue<string>("SecretKey");
                var key = Encoding.ASCII.GetBytes(secretKey);

                var claims = new ClaimsIdentity();
                claims.AddClaim(new Claim(ClaimTypes.NameIdentifier, login.Usuario));
                claims.AddClaim(new Claim(ClaimTypes.Role, usuario.Rol.SRole));
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = claims,
                    Expires = DateTime.UtcNow.AddHours(0.5),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenT = tokenHandler.CreateToken(tokenDescriptor);

                string bearer_token = tokenHandler.WriteToken(tokenT);

                DateTime hrExpr = DateTime.Now.AddMinutes(30);

                var nvaLocacion = _gFc.CreatePoint(new Coordinate(login.Longitude, login.Latitude));

                var nvaSesion = new Sesiones
                {
                    IUsuario = usuario.IID,
                    SUsuario = usuario.SEmail,
                    Location = nvaLocacion,
                    TimeStamp = DateTime.Now,
                    DExpiracion = hrExpr
                };

                _context.tSesiones.Add(nvaSesion);
                await _context.SaveChangesAsync();

                _context.tTokensTemp.Remove(tokenTemp);
                _context.SaveChanges();

                return Ok(ErrorHelper.ResponseToken(200, bearer_token, hrExpr.ToString()));
            }
            else
            {
                return BadRequest(new Response() { statusCode = "403", status = "Authentication failure", sMensaje = "Incorrect password" });
            }
        }

        [AllowAnonymous]
        [HttpPost("GoogleLogin")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginDTO login)
        {
            var clientId = _configuration["Authentication:Google:ClientId"];

            try
            {
                var settings = new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new[] { clientId }
                };
                var payload = await GoogleJsonWebSignature.ValidateAsync(login.token, settings);

                Usuario? usuario = await _context.tUsuarios.Include(x => x.Rol).FirstOrDefaultAsync(x => x.SEmail == payload.Email);

                if (usuario == null)
                {
                    var rolInvitado = await _context.tRoles.FirstOrDefaultAsync(r => r.SRole == "Editor");

                    if (rolInvitado == null) return StatusCode(500, new { mensaje = "El rol 'Editor' no está configurado." });

                    usuario = new Usuario
                    {
                        SNombre = payload.Name,
                        SEmail = payload.Email,
                        DFechaC = DateTime.Now,
                        IEstatus = 1,
                        iIdRol = rolInvitado.IIdRol
                    };

                    _context.tUsuarios.Add(usuario);
                    await _context.SaveChangesAsync();
                }

                var secretKey = _configuration.GetValue<string>("SecretKey");
                var key = Encoding.ASCII.GetBytes(secretKey);

                var claims = new ClaimsIdentity();
                claims.AddClaim(new Claim(ClaimTypes.NameIdentifier, usuario.SEmail));
                claims.AddClaim(new Claim(ClaimTypes.Role, usuario.Rol.SRole));
        
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = claims,
                    Expires = DateTime.UtcNow.AddHours(0.5),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenT = tokenHandler.CreateToken(tokenDescriptor);

                string token = tokenHandler.WriteToken(tokenT);

                return Ok(new { token });
            }
            catch (InvalidJwtException)
            {
                return Unauthorized(new { mensaje = "Token de Google no válido." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error interno del servidor.", detalle = ex.Message });
            }
        }

        [HttpPost]
        [Route("SolRecPasswd")]
        public async Task<IActionResult> SolRecPasswd(string correo)
        {
            if (!_mailSer.IsValidMail(correo)) return BadRequest(new { mensaje = "Ingresa un correo valido" });

            var usuario = await _context.tUsuarios.FirstOrDefaultAsync(u => u.SEmail == correo);

            if (usuario == null) return Ok();

            var token = _mailSer.GTokenRec();
            var tokenTemp = new TokenTemp
            {
                SToken = token,
                IUsuarioId = usuario.IID,
                DFechaExpiracion = DateTime.Now.AddMinutes(5)
            };

            _context.tTokensTemp.Add(tokenTemp);
            await _context.SaveChangesAsync();

            string asunto = mailpt1 + token + mailpt2;
            _mailSer.EnviarMail(correo, "Restablecer contrasena", asunto);

            return Ok();
        }

        [HttpPost]
        [Route("RestablecerPasswd")]
        public async Task<IActionResult> RestablecerPasswd(string Token, string NvaPasswd)
        {
            var tokenTemp = await _context.tTokensTemp
                                            .FirstOrDefaultAsync(x => x.SToken == Token && x.DFechaExpiracion > DateTime.Now);

            if (tokenTemp == null) return BadRequest(new { mensaje = "Token invalido" });

            var usuario = await _context.tUsuarios.FindAsync(tokenTemp.IUsuarioId);

            if (usuario == null) return BadRequest();

            usuario.SPasswd = BCrypt.Net.BCrypt.HashPassword(NvaPasswd);

            _context.tTokensTemp.Remove(tokenTemp);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Password restablecida con exito" });
        }

        [HttpDelete]
        [Route("CerrarSesion")]
        public async Task<IActionResult> CerrarSesion(int iUsuario)
        {
            var sesion = await _context.tSesiones.FirstOrDefaultAsync(s => s.IUsuario == iUsuario);

            if (sesion != null)
            {
                _context.tSesiones.Remove(sesion);
                _context.SaveChanges();
                
                return Ok(new { mensaje = "Sesion cerrada con exito" });
            }

            return BadRequest(new { mensaje = "No hay ninguna sesion activa con este usuario" });
        }

        public class Authorization
        {
            [Required(ErrorMessage = "The user is required")]
            public string? Usuario { get; set; }
            [Required(ErrorMessage = "The key is mandatory")]
            public string? Clave { get; set; }
            public double Latitude { get; set; }
            public double Longitude { get; set; }
        }

        public class GoogleLoginDTO
        {
            public string? token { get; set; }
        }

        public class Coords
        {
            public double Latitude { get; set; }
            public double Longitude { get; set; }
        }
    }
}
