namespace EcommerceApp.DTOs.Users;

public class UserRegisterDto
{
    public string Nombre { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Rol { get; set; } = "Cliente"; // Se envía desde el frontend
}