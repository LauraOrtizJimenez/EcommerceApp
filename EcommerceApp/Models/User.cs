using System.ComponentModel.DataAnnotations;

namespace EcommerceApp.Models;

public class User
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Nombre { get; set; } = string.Empty;

    [Required]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    // 0 = Admin | 1 = Empresa | 2 = Cliente
    [Required]
    public int Rol { get; set; }
}