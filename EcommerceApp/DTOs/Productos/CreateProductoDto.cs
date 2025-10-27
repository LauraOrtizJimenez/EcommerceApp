namespace EcommerceApp.DTOs.Productos
{
    public class CreateProductoDto
    {
        public int Id { get; set; } // 👈 Ahora puedes escribirlo manualmente
        public string Nombre { get; set; } = string.Empty;
        public string Datos { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public int Stock { get; set; }
    }
}