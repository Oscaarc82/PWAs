using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PWAs.Context;
using PWAs.Models.Reportes;
using PWAs.Services;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
using PWAs.Sistema;

namespace PWAs.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductosController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly AppDbContext ctx;
        private readonly IHostingEnvironment _hosting;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductosController(IConfiguration configuration, AppDbContext ctx, Microsoft.AspNetCore.Hosting.IHostingEnvironment hosting, IWebHostEnvironment webHostEnvironment)
        {
            _configuration = configuration;
            this.ctx = ctx;
            _hosting = hosting;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpPost]
        [Route("AltaProducto")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AltaProducto(Producto producto)
        {
            ProductosService pps = new ProductosService(_configuration);
            bool res;

            try
            {
                res = pps.AltaProducto(producto);
            }
            catch
            {
                res = false;
            }
            if (res) return Ok(new { statusCode = "200", status = "Success", sMensaje = "Alta exitosa" }); else return Ok(new Response() { statusCode = "400", status = "Error", sMensaje = "Ocurrio un error en el proceso" });
        }
<<<<<<< HEAD
    }
=======

        [HttpGet]
        [Route("ProductosBajoStock")]
        //[Authorize(Roles = "Admin, Editor")]
        public async Task<IActionResult> ListarProductosBajoStock()
        {
            ProductosService ps = new ProductosService(_configuration);
            var productos = ps.ListarProductosBajoStock();

            if (productos == null || !productos.Any())
                return NotFound(new { mensaje = "No hay productos con bajo stock" });

            return Ok(productos);
        }
    }

>>>>>>> edbb295 (Subiendo mi proyecto a Miguel)
}
