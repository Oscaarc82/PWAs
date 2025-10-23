using Microsoft.AspNetCore.Mvc;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
using PWAs.Context;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using PWAs.Models.Reportes;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using PWAs.Services;
using Newtonsoft.Json;

namespace PWAs.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class ComprasController : ControllerBase
    {
        private readonly IConfiguration? _config;
        private readonly AppDbContext _ctx;

        public ComprasController(IConfiguration? config, AppDbContext ctx)
        {
            _config = config;
            _ctx = ctx;
        }

        [HttpPost]
        [Route("CrearCompra")]
        [Authorize(Roles = "Admin, Editor")]
        public async Task<IActionResult> CrearCompra([FromBody] CompraDto compraDto)
        {
            ComprasService cps = new ComprasService(_config);
            MovimientosService ms = new(_config);
            int idCompra;

            var usuarioId = await _ctx.tUsuarios.FirstOrDefaultAsync(x => x.IID == compraDto.IIdUsuario);

            if (usuarioId.IID != compraDto.IIdUsuario) return NotFound(new { mensaje = "Usuario no encontrado" });

            decimal? totalCompra = 0;
            var detallesDb = new List<CompraDetalles>();

            foreach (var detalleDto in compraDto.Detalles)
            {
                var producto = await _ctx.tProducto.FindAsync(detalleDto.IIdProducto);

                if (producto == null) return BadRequest(new { mensaje = $"Producto con id {detalleDto.IIdProducto} no encontrado"});

                if (producto.IStock < detalleDto.ICantidad) return BadRequest(new { mensaje = $"Producto con id {detalleDto.IIdProducto} no cuenta con el stock suficiente" });

                totalCompra += producto.DePrecio * detalleDto.ICantidad;

                detallesDb.Add(new CompraDetalles
                {
                    IProducto = detalleDto.IIdProducto,
                    ICantidad = detalleDto.ICantidad,
                    DePrecio = producto.DePrecio
                });

                producto.IStock -= detalleDto.ICantidad;

                ms.registrarMovimiento(2, detalleDto.IIdProducto, detalleDto.ICantidad, producto.SCodigo, usuarioId.SNombre);
            }

            var compra = new Compra
            {
                IIdUsuario = usuarioId.IID,
                DFechaCompra = DateTime.Now,
                DeTotal = totalCompra,
                Detalles = detallesDb
            };

            idCompra = cps.CrearCompra(detallesDb, compra);
            await _ctx.SaveChangesAsync();

            return Ok(new { mensaje = "Compra realizada con exito", iDCompra = idCompra });
        }

        [HttpGet]
        [Route("ListarCompras")]
        [Authorize(Roles = "Admin, Editor")]
        public async Task<IActionResult> ListarCompras()
        {
            try
            {
                var compras = await _ctx.tCompra
                    .Include(c => c.Usuario)
                    .Include(c => c.Detalles)
                    .ThenInclude(d => d.Producto)
                    .ToListAsync();

                if (compras == null || !compras.Any()) return NotFound(new { mensaje = "No se encontraron compras" });

                return Ok(compras);
            } 
            catch
            {
                return NotFound(new { mensaje = "No se encontraron compras" });
            }
        }

        public class CompraDto
        {
            [JsonRequired] public int IIdUsuario { get; set; }
            public List<CompraDetalleDto>? Detalles { get; set; }
        }

        public class CompraDetalleDto
        {
            public int IIdProducto { get; set; }
            public int ICantidad {  get; set; }
        }
    }
}
