using Microsoft.AspNetCore.Mvc;
using PWAs.Context;
using PWAs.Models.Usuarios;
using PWAs.Sistema;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.AspNetCore.Authorization;
using PWAs.Services;
using System.ComponentModel.DataAnnotations;

namespace PWAs.Controller
{
    [Route("api/[controller]")]
    [ApiController]

    public class UsuariosController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly AppDbContext ctx;
        private readonly IHostingEnvironment _hosting;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly MailService _mailSer = new MailService();

        public UsuariosController(IConfiguration configuration, AppDbContext ctx, IHostingEnvironment hosting, IWebHostEnvironment webHostEnvironment)
        {
            _configuration = configuration;
            this.ctx = ctx;
            _hosting = hosting;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpPost]
        [Route("AltaUsuario")]
        [AllowAnonymous]
        public async Task<IActionResult> AltaUsuario(UsuarioNvo usuario)
        {
            if (!_mailSer.IsValidMail(usuario.SEmail)) return BadRequest(new { mensaje = "Ingresa un correo valido" });

            UsuariosService usc = new UsuariosService(_configuration);
            bool res;
            string sSalt = _mailSer.GTokenRec();

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
        public async Task<IActionResult> ActualizarUsuario(int iIdUsuario, string sNombre, string sEmail)
        {
            if (!_mailSer.IsValidMail(sEmail)) return BadRequest(new { mensaje = "Ingresa un correo valido" });

            UsuariosService usc = new UsuariosService(_configuration);
            bool res;

            try
            {
                res = usc.ActualizarUsuario(iIdUsuario, sNombre, sEmail);
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
            public int iIdUsuario { get; set; }
            [Required]
            public int iAccion {  get; set; }
        }
    }
}
