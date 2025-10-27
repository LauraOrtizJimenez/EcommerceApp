namespace EcommerceApp.Models
{
    public class Producto
    {
        public int Id { get; set; }  // 👈 ID visible y editable (si quieres ponerlo manual)
        public string Nombre { get; set; } = string.Empty;
        public string Datos { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public int Stock { get; set; }

        // Relación con la empresa (usuario que creó el producto)
        public int EmpresaId { get; set; }
        public User? Empresa { get; set; }
    }
}