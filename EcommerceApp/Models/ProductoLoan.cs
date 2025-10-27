namespace EcommerceApp.Models
{
    public class ProductoLoan
    {
        public int Id { get; set; }
        public int ProductoId { get; set; }
        public Producto? Producto { get; set; }
        public int ClienteId { get; set; }
        public User? Cliente { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; } // 🟢 Nuevo campo
        public decimal Total { get; set; } // 🟢 Nuevo campo
        public DateTime FechaPedido { get; set; } = DateTime.UtcNow;
    }
}