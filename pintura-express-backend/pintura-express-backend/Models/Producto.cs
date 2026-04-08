namespace pintura_express_backend.Models
{
    public class Producto
    {
        public int ID { get; set; }
        public string Nombre { get; set; } = "";
        public string Color { get; set; } = "";
        public string Tipo { get; set; } = "";
        public decimal Precio { get; set; }
        public int Stock { get; set; }
    }
}
