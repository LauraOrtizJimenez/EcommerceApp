namespace EcommerceApp.DTOs.ProductosLoan
{
    public class ProductoRequestDto
    {
        public string EmpresaNombre { get; set; } = string.Empty;
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }
    }
}