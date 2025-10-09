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
            bool bAlta;
            int res;
            string prefijo, numSec, codigo;

            try
            {
                res = pps.AltaProducto(producto);
                bAlta = res > -1;

                if (bAlta)
                {
                    prefijo = (producto.SNombre.Length >= 3) ? producto.SNombre.Substring(0, 3).ToUpper() : producto.SNombre.ToUpper();
                    numSec = res.ToString("D3");
                    codigo = $"{prefijo}{numSec}";

                    var nProd = new Producto { IId = res };

                    ctx.tProducto.Attach(nProd);
                    nProd.SCodigo = codigo;

                    ctx.Entry(nProd).Property(p => p.SCodigo).IsModified = true;
                    await ctx.SaveChangesAsync();
                }
            }
            catch
            {
                bAlta = false;
            }
            if (bAlta) return Ok(new { statusCode = "200", status = "Success", sMensaje = "Alta exitosa" }); else return Ok(new Response() { statusCode = "400", status = "Error", sMensaje = "Ocurrio un error en el proceso" });
        }
    }
}
