namespace pintura_express_backend.Models
{
    public class Orden
    {
        public int ID { get; set; }
        public string ClientNom { get; set; } = "";
        public string VendorNom { get; set; } = "";
        public decimal Total { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string Estatus { get; set; } = "Pendiente";

        public List<OrdenItem> Items { get; set; } = new();
    }
}
