using EcommerceApp.Data;
using EcommerceApp.DTOs.ProductosLoan;
using EcommerceApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace EcommerceApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "2")] // 👈 Solo clientes pueden hacer pedidos o ver los suyos
    public class ProductosLoanController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductosLoanController(AppDbContext context)
        {
            _context = context;
        }

        // 🔹 POST: api/productosloan
        [HttpPost]
        public async Task<IActionResult> CrearPedido([FromBody] ProductoRequestDto dto)
        {
            // Buscar empresa por nombre
            var empresa = await _context.Users
                .FirstOrDefaultAsync(e => e.Nombre == dto.EmpresaNombre && e.Rol == 1);

            if (empresa == null)
                return NotFound(new { message = "Empresa no encontrada" });

            // Buscar producto dentro de esa empresa
            var producto = await _context.Productos
                .FirstOrDefaultAsync(p => p.Id == dto.ProductoId && p.EmpresaId == empresa.Id);

            if (producto == null)
                return NotFound(new { message = "Producto no encontrado para esa empresa" });

            // 🟩 Verificar stock suficiente antes de crear pedido
            if (producto.Stock < dto.Cantidad)
                return BadRequest(new { message = "No hay suficiente stock disponible" });

            // Obtener cliente autenticado
            var clienteId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            // ✅ Crear pedido con precio y total (decimal)
            var pedido = new ProductoLoan
            {
                ProductoId = producto.Id,
                ClienteId = clienteId,
                Cantidad = dto.Cantidad,
                PrecioUnitario = producto.Precio,                       // ✅ guarda el precio actual
                Total = producto.Precio * (decimal)dto.Cantidad,         // ✅ calcula total como decimal
                FechaPedido = DateTime.UtcNow
            };

            _context.ProductosLoan.Add(pedido);

            // 🟩 Descontar del stock del producto
            producto.Stock -= dto.Cantidad;

            // 🟩 Guardar los cambios
            await _context.SaveChangesAsync();

            // 🟩 Retornar mensaje con el stock actualizado
            return Ok(new
            {
                message = "Pedido creado exitosamente",
                stockRestante = producto.Stock,
                total = pedido.Total
            });
        }

        // 🔹 GET: api/productosloan
        // 🔹 GET: api/productosloan
        [HttpGet]
        public async Task<IActionResult> GetMisPedidos()
        {
            try
            {
                var clienteId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

                var pedidos = await _context.ProductosLoan
                    .Include(p => p.Producto)
                    .ThenInclude(prod => prod.Empresa)
                    .Where(p => p.ClienteId == clienteId)
                    .Select(p => new
                    {
                        p.Id,
                        ProductoNombre = p.Producto != null ? p.Producto.Nombre : "Desconocido",
                        EmpresaNombre = p.Producto != null && p.Producto.Empresa != null ? p.Producto.Empresa.Nombre : "Desconocida",
                        p.Cantidad,
                        p.PrecioUnitario,
                        p.Total,
                        p.FechaPedido
                    })
                    .ToListAsync();

                return Ok(pedidos);
            }
            catch (Exception ex)
            {
                Console.WriteLine("⚠️ ERROR en GetMisPedidos: " + ex.Message);
                Console.WriteLine("📄 StackTrace: " + ex.StackTrace);

                return StatusCode(500, new
                {
                    message = "Error interno del servidor",
                    error = ex.Message,
                    stack = ex.StackTrace
                });
            }
        }


    }
}
