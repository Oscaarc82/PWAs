using Microsoft.AspNetCore.Mvc;
using PWAs.Context;
using PWAs.Models.Usuarios;
using PWAs.Sistema;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.AspNetCore.Authorization;
using PWAs.Services;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace PWAs.Controller
{
    [Route("api/[controller]")]
    [ApiController]

    public class UsuariosController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public UsuariosController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        [Route("AltaUsuario")]
        [AllowAnonymous]
        public async Task<IActionResult> AltaUsuario(UsuarioNvo usuario)
        {
            if (MailService.IsValidMail(usuario.SEmail)) return BadRequest(new { mensaje = "Ingresa un correo valido" });

            UsuariosService usc = new UsuariosService(_configuration);
            bool res;
            string sSalt = MailService.GTokenRec();

            try
            {
                res = usc.AltaUsuario(usuario, sSalt);
            }
            catch
            {
                res = false;
            }
            if (res) return Ok(new { statusCode = "200", status = "Success", sMensaje = "Alta exitosa", sSalt }); else return Ok(new Response() { statusCode = "400", status = "Error", sMensaje = "Ocurrio un error en el proceso" });
        }

        [HttpGet]
        [Route("ListarUsuarios")]
        [Authorize(Roles = "Admin, Lector")]
        public Task<List<UsuariosList>> ListarUsuarios()
        {
            UsuariosService usc = new UsuariosService(_configuration);
            List<UsuariosList> lstUsuarios = new List<UsuariosList>();

            try
            {
                lstUsuarios = usc.ListarUsuarios();
            }
            catch
            {
                lstUsuarios.Add(new UsuariosList());
            }
            return Task.FromResult(lstUsuarios);
        }

        [HttpDelete]
        [Route("BajaUsuario")]
        [Authorize(Roles = "Admin, Editor")]
        public Task<IActionResult> BajaUsuario(BajaUsuarioDto usuario)
        {
            UsuariosService usc = new UsuariosService(_configuration);
            bool res;

            try
            {
                res = usc.EliminarUsuario(usuario.iIdUsuario, usuario.iAccion);
            }
            catch
            {
                res = false;
            }
            if (res) return Task.FromResult<IActionResult>(Ok(new Response() { statusCode = "200", status = "Success", sMensaje = "Baja exitosa" })); else return Task.FromResult<IActionResult>(Ok(new Response() { statusCode = "304", status = "Error", sMensaje = "Ocurrio un error en el proceso" }));
        }

        [HttpPost]
        [Route("ActualizarUsuario")]
        [Authorize(Roles = "Admin, Editor")]
        public async Task<IActionResult> ActualizarUsuario(ActualizarUsuarioDto usuario)
        {
            if (MailService.IsValidMail(usuario.sEmail)) return BadRequest(new { mensaje = "Ingresa un correo valido" });

            UsuariosService usc = new UsuariosService(_configuration);
            bool res;

            try
            {
                res = usc.ActualizarUsuario(usuario.iIdUsuario, usuario.sNombre, usuario.sEmail);
            }
            catch
            {
                res = false;
            }
            if (res) return Ok(new Response() { statusCode = "200", status = "Success", sMensaje = "Actualizacion exitosa" }); else return Ok(new Response() { statusCode = "304", status = "Error", sMensaje = "Ocurrio un error en el proceso" });
        }

        public class BajaUsuarioDto
        {
            [Required]
            [JsonRequired] public int iIdUsuario { get; set; }
            [Required]
            [JsonRequired] public int iAccion {  get; set; }
        }

        public class ActualizarUsuarioDto
        {
            [Required]
            public int iIdUsuario { get; private set; }
            [JsonRequired] public string sNombre { get; set; }
            [JsonRequired] public string sEmail { get; set; }
        }
    }
}
