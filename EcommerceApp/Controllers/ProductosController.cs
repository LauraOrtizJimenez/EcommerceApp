using EcommerceApp.Data;
using EcommerceApp.DTOs.Productos;
using EcommerceApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace EcommerceApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductosController(AppDbContext context)
        {
            _context = context;
        }

        // ✅ POST: api/productos (crear producto)
        [Authorize(Roles = "1")] // Empresa
        [HttpPost]
        public async Task<IActionResult> CrearProducto(CreateProductoDto dto)
        {
            try
            {
                // 🔍 Mostrar todos los claims en consola (útil para Azure)
                Console.WriteLine("=== Claims del usuario ===");
                foreach (var claim in User.Claims)
                {
                    Console.WriteLine($"{claim.Type}: {claim.Value}");
                }

                // ✅ Verificar que el claim del ID existe
                var empresaIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(empresaIdClaim))
                {
                    return BadRequest(new { message = "❌ No se encontró el ID de la empresa en el token." });
                }

                if (!int.TryParse(empresaIdClaim, out var empresaId))
                {
                    return BadRequest(new { message = "❌ El ID de la empresa en el token no es válido." });
                }

                // ✅ Crear el producto
                var producto = new Producto
                {
                    Id = dto.Id,
                    Nombre = dto.Nombre,
                    Datos = dto.Datos,
                    Precio = dto.Precio,
                    Stock = dto.Stock,
                    EmpresaId = empresaId
                };

                _context.Productos.Add(producto);
                await _context.SaveChangesAsync();

                return Ok(new { message = "✅ Producto creado exitosamente", producto.Id });
            }
            catch (Exception ex)
            {
                // 🧨 Capturar y mostrar cualquier error real
                Console.WriteLine($"🚨 ERROR en CrearProducto: {ex.Message}");
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }


        // ✅ GET: api/productos (ver productos de la empresa)
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetProductos()
        {
            var rol = User.FindFirstValue(ClaimTypes.Role);
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            // Si es empresa → ver solo sus productos
            if (rol == "1")
            {
                var productosEmpresa = await _context.Productos
                    .Where(p => p.EmpresaId == userId)
                    .Select(p => new
                    {
                        p.Id,
                        p.Nombre,
                        p.Datos,
                        p.Precio,
                        p.Stock,
                        Empresa = p.Empresa.Nombre
                    })
                    .ToListAsync();

                return Ok(productosEmpresa);
            }

            // Si es cliente → ver todos los productos disponibles
            var productos = await _context.Productos
                .Include(p => p.Empresa)
                .Select(p => new
                {
                    p.Id,
                    p.Nombre,
                    p.Datos,
                    p.Precio,
                    p.Stock,
                    Empresa = p.Empresa.Nombre // 🔹 Aquí ya no saldrá null
                })
                .ToListAsync();

            return Ok(productos);
        }




        // ✅ GET: api/productos/{id} (ver producto por ID)
        [Authorize(Roles = "1")] // Empresa
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductoById(int id)
        {
            var empresaId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var producto = await _context.Productos
                .Include(p => p.Empresa) // 👈 incluimos la relación con la empresa
                .FirstOrDefaultAsync(p => p.Id == id && p.EmpresaId == empresaId);

            if (producto == null)
                return NotFound(new { message = "Producto no encontrado" });

            return Ok(new
            {
                producto.Id,
                producto.Nombre,
                producto.Datos,
                producto.Precio,
                producto.Stock,
                Empresa = producto.Empresa?.Nombre // 👈 mostramos solo el nombre
            });
        }


        // ✅ PUT: api/productos/{id} (editar producto)
        [Authorize(Roles = "1")] // Empresa
        [HttpPut("{id}")]
        public async Task<IActionResult> EditarProducto(int id, CreateProductoDto dto)
        {
            var empresaId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var producto = await _context.Productos
                .FirstOrDefaultAsync(p => p.Id == id && p.EmpresaId == empresaId);

            if (producto == null)
                return NotFound(new { message = "Producto no encontrado" });

            producto.Nombre = dto.Nombre;
            producto.Datos = dto.Datos;
            producto.Precio = dto.Precio;
            producto.Stock = dto.Stock;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Producto actualizado correctamente" });
        }

        // ✅ DELETE: api/productos/{id}
        [Authorize(Roles = "1")] // Empresa
        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarProducto(int id)
        {
            var empresaId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var producto = await _context.Productos
                .FirstOrDefaultAsync(p => p.Id == id && p.EmpresaId == empresaId);

            if (producto == null)
                return NotFound(new { message = "Producto no encontrado" });

            _context.Productos.Remove(producto);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Producto eliminado correctamente" });
        }
    }
}

