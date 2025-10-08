using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PWAs.Context;
using PWAs.Models;
using System.Security.Claims;

namespace PWAs.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProveedoresController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProveedoresController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Proveedores
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProveedorDTO>>> GetProveedores()
        {
            try
            {
                var proveedores = await _context.tProveedores
                    .Select(p => new ProveedorDTO
                    {
                        Iid = p.Iid,
                        SNombre = p.SNombre,
                        STelefono = p.STelefono,
                        SContacto = p.SContacto
                    })
                    .ToListAsync();

                return Ok(proveedores);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al obtener los proveedores", error = ex.Message });
            }
        }

        // GET: api/Proveedores/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProveedorDTO>> GetProveedor(int id)
        {
            try
            {
                var proveedor = await _context.tProveedores
                    .Where(p => p.Iid == id)
                    .Select(p => new ProveedorDTO
                    {
                        Iid = p.Iid,
                        SNombre = p.SNombre,
                        STelefono = p.STelefono,
                        SContacto = p.SContacto
                    })
                    .FirstOrDefaultAsync();

                if (proveedor == null)
                {
                    return NotFound(new { mensaje = $"Proveedor con ID {id} no encontrado" });
                }

                return Ok(proveedor);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al obtener el proveedor", error = ex.Message });
            }
        }

        // POST: api/Proveedores
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ProveedorDTO>> PostProveedor(ProveedorCreateDTO proveedorCreateDTO)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { mensaje = "Datos inválidos", errores = ModelState.Values.SelectMany(v => v.Errors) });
                }

                // Validar si ya existe un proveedor con el mismo nombre
                var proveedorExistente = await _context.tProveedores
                    .FirstOrDefaultAsync(p => p.SNombre.ToLower() == proveedorCreateDTO.SNombre.ToLower());

                if (proveedorExistente != null)
                {
                    return Conflict(new { mensaje = "Ya existe un proveedor con ese nombre" });
                }

                var proveedor = new Proveedor
                {
                    SNombre = proveedorCreateDTO.SNombre,
                    STelefono = proveedorCreateDTO.STelefono,
                    SContacto = proveedorCreateDTO.SContacto
                };

                _context.tProveedores.Add(proveedor);
                await _context.SaveChangesAsync();

                var proveedorDTO = new ProveedorDTO
                {
                    Iid = proveedor.Iid,
                    SNombre = proveedor.SNombre,
                    STelefono = proveedor.STelefono,
                    SContacto = proveedor.SContacto
                };

                return CreatedAtAction(nameof(GetProveedor), new { id = proveedor.Iid }, proveedorDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al crear el proveedor", error = ex.Message });
            }
        }

        // PUT: api/Proveedores/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PutProveedor(int id, ProveedorUpdateDTO proveedorUpdateDTO)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { mensaje = "Datos inválidos", errores = ModelState.Values.SelectMany(v => v.Errors) });
                }

                var proveedor = await _context.tProveedores.FindAsync(id);
                if (proveedor == null)
                {
                    return NotFound(new { mensaje = $"Proveedor con ID {id} no encontrado" });
                }

                // Validar si ya existe otro proveedor con el mismo nombre (excluyendo el actual)
                var proveedorExistente = await _context.tProveedores
                    .FirstOrDefaultAsync(p => p.SNombre.ToLower() == proveedorUpdateDTO.SNombre.ToLower() && p.Iid != id);

                if (proveedorExistente != null)
                {
                    return Conflict(new { mensaje = "Ya existe otro proveedor con ese nombre" });
                }

                proveedor.SNombre = proveedorUpdateDTO.SNombre;
                proveedor.STelefono = proveedorUpdateDTO.STelefono;
                proveedor.SContacto = proveedorUpdateDTO.SContacto;

                _context.Entry(proveedor).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return Ok(new { mensaje = "Proveedor actualizado correctamente" });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProveedorExists(id))
                {
                    return NotFound(new { mensaje = $"Proveedor con ID {id} no encontrado" });
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al actualizar el proveedor", error = ex.Message });
            }
        }

        // DELETE: api/Proveedores/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteProveedor(int id)
        {
            try
            {
                var proveedor = await _context.tProveedores.FindAsync(id);
                if (proveedor == null)
                {
                    return NotFound(new { mensaje = $"Proveedor con ID {id} no encontrado" });
                }

          
                _context.tProveedores.Remove(proveedor);
                await _context.SaveChangesAsync();

                return Ok(new { mensaje = "Proveedor eliminado correctamente" });


            }
            catch (DbUpdateException ex)
            {
                
                return BadRequest(new { mensaje = "No se puede eliminar el proveedor porque tiene registros relacionados" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al eliminar el proveedor", error = ex.Message });
            }
        }

        private bool ProveedorExists(int id)
        {
            return _context.tProveedores.Any(e => e.Iid == id);
        }
    }
}