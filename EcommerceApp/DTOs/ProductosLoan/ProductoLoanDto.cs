namespace EcommerceApp.DTOs.ProductosLoan;

public class ProductoLoanDto
{
    public int Id { get; set; }
    public int ProductoId { get; set; }
    public string NombreProducto { get; set; } = string.Empty;
    public int ClienteId { get; set; }
    public int Cantidad { get; set; }
    public DateTime FechaPedido { get; set; }
}