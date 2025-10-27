namespace EcommerceApp.DTOs.Productos;

public class ProductoDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Datos { get; set; } = string.Empty;
    public decimal Precio { get; set; }
    public int Stock { get; set; }
    public int EmpresaId { get; set; }
}