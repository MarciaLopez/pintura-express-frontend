namespace pintura_express_backend.Models
{
    public class OrdenCreateDTO
    {
        public string ClientNom { get; set; }
        public string VendorNom { get; set; }
        public List<OrdenItemDTO> Items { get; set; }
    }

    public class OrdenItemDTO
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
