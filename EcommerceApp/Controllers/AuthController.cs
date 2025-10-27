using EcommerceApp.Data;
using EcommerceApp.DTOs.Users;
using EcommerceApp.Models;
using EcommerceApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace EcommerceApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly TokenService _tokenService;

        public AuthController(AppDbContext context, TokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        // 🔹 Registro
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterDto dto)
        {
            var user = new User
            {
                Nombre = dto.Nombre,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Rol = int.Parse(dto.Rol)
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok("Usuario registrado correctamente.");
        }

        // 🔹 Login
        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return Unauthorized("Credenciales inválidas.");

            var token = _tokenService.CreateToken(user);

            return Ok(new
            {
                Token = token,
                Rol = user.Rol,
                Nombre = user.Nombre,
                Email = user.Email
            });
        }

        // 🔹 Obtener datos del usuario logueado (para mostrar en dashboard)
        [HttpGet("user-info")]
        [Authorize]
        public async Task<IActionResult> GetUserInfo()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized(new { message = "Token inválido o expirado" });

            if (!int.TryParse(userIdClaim, out int userId))
                return Unauthorized(new { message = "ID de usuario no válido" });

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                return NotFound(new { message = "Usuario no encontrado" });

            return Ok(new
            {
                Id = user.Id,
                Nombre = user.Nombre,
                Email = user.Email,
                Rol = user.Rol
            });
        }
    }
}
