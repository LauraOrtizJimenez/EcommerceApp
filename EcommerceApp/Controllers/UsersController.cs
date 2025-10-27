using EcommerceApp.Data;
using EcommerceApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace EcommerceApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        // 🔹 Solo el admin puede crear empresas
        [Authorize(Roles = "0")]
        [HttpPost("crear-empresa")]
        public async Task<IActionResult> CrearEmpresa(string nombre, string email, string password)
        {
            if (await _context.Users.AnyAsync(u => u.Email == email))
                return BadRequest(new { message = "El correo ya está registrado." });

            var empresa = new User
            {
                Nombre = nombre,
                Email = email,
                Rol = 1, // Rol 1 = Empresa
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password)
            };

            _context.Users.Add(empresa);
            await _context.SaveChangesAsync();

            return Ok(new { message = $"Empresa '{nombre}' creada correctamente." });
        }

        // 🔹 Clientes pueden ver las empresas disponibles
        [Authorize(Roles = "2")] // o usa [AllowAnonymous] si quieres que cualquiera pueda verlas
        [HttpGet("empresas")]
        public async Task<IActionResult> GetEmpresas()
        {
            var empresas = await _context.Users
                .Where(u => u.Rol == 1)
                .Select(u => new
                {
                    u.Id,
                    u.Nombre,
                    u.Email
                })
                .ToListAsync();

            return Ok(empresas);
        }
    }
}