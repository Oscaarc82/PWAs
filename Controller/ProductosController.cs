using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PWAs.Context;
using PWAs.Models;
using PWAs.Models.Reportes;
using PWAs.Services;
using PWAs.Sistema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

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
        public async Task<IActionResult> AltaProducto(ProductoNvo producto)
        {
            ProductosService pps = new ProductosService(_configuration);
            MovimientosService ms = new(_configuration);
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

                    var prov = await ctx.tProveedor.FirstOrDefaultAsync(x => x.Iid == producto.iProveedor);

                    string nombreProv = (prov != null) ? prov.SNombre : "Desconocido";

                    bAlta =  ms.registrarMovimiento(1, res, producto.IStock, codigo, nombreProv);
                }
            }
            catch
            {
                bAlta = false;
            }
            if (bAlta) return Ok(new { statusCode = "200", status = "Success", sMensaje = "Alta exitosa" }); else return Ok(new Response() { statusCode = "400", status = "Error", sMensaje = "Ocurrio un error en el proceso" });
        }

        [HttpPut]
        [Route("ActualizarProducto")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ActualizarProducto(ActualizarProductoDTO producto)
        {
            ProductosService ps = new ProductosService(_configuration);
            bool bBandera;

            try
            {
                var prod = new Producto { IId = producto.iProducto };

                ctx.tProducto.Attach(prod);

                prod.SNombre = producto.sNombre;
                prod.SDescripcion = producto.sDescripcion;
                prod.DePrecio = producto.dePrecio;
                prod.IStockMin = producto.iStockMin;

                ctx.Entry(prod).Property(p => p.SNombre).IsModified = true;
                ctx.Entry(prod).Property(p => p.SDescripcion).IsModified = true;
                ctx.Entry(prod).Property(p => p.DePrecio).IsModified = true;
                ctx.Entry(prod).Property(p => p.IStockMin).IsModified = true;

                await ctx.SaveChangesAsync();

                bBandera = true;
            }
            catch
            {
                bBandera = false;
            }
            if (bBandera) return Ok(new { statusCode = "200", status = "Success", sMensaje = "Actualizacion exitosa" }); else return Ok(new Response() { statusCode = "400", status = "Error", sMensaje = "Ocurrio un error en el proceso" });
        }

        [HttpPut]
        [Route("AumentarStock")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AumentarStock(AumentarStockDTO producto)
        {
            MovimientosService ms = new(_configuration);
            bool bBandera;

            try
            {
                var prod = await ctx.tProducto.FirstOrDefaultAsync(x => x.IId == producto.iProducto);

                if (prod == null) NotFound(new { mensaje = "Producto no encontrado" });

                prod.IStock += producto.iCantidad;

                bBandera = await ctx.SaveChangesAsync() > 0;

                if (bBandera)
                {
                    var prov = await ctx.tProveedor.FirstOrDefaultAsync(x => x.Iid == producto.iProveedor);

                    string nomProv = prov?.SNombre ?? "Proveedor desconocido";

                    bBandera = ms.registrarMovimiento(1, producto.iProducto, producto.iCantidad, prod.SCodigo, nomProv);
                }
            }
            catch
            {
                bBandera = false;
            }
            if (bBandera) return Ok(new { statusCode = "200", status = "Success", sMensaje = "Actualizacion exitosa" }); else return Ok(new Response() { statusCode = "400", status = "Error", sMensaje = "Ocurrio un error en el proceso" });
        }

        [HttpGet]
        [Route("ProductosBajoStock")]
        [Authorize(Roles = "Admin, Editor")]
        public async Task<IActionResult> ListarProductosBajoStock()
        {
            ProductosService ps = new ProductosService(_configuration);
            var productos = ps.ListarProductosBajoStock();

            if (productos == null || !productos.Any())
                return NotFound(new { mensaje = "No hay productos con bajo stock" });

            return Ok(productos);
        }

        public class ActualizarProductoDTO
        {
            [Required]
            public int iProducto { get; set; }
            public string sNombre { get; set; }
            public string sDescripcion { get; set; }
            public decimal dePrecio { get; set; }
            public int iStockMin { get; set; }
        }

        public class AumentarStockDTO
        {
            [Required]
            public int iProducto { get; set; }
            public int iCantidad { get; set; }
            public int iProveedor { get; set; }
        }
    }
}
