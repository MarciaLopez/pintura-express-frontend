namespace pintura_express_backend.Models
{
    public class OrdenItem
    {
        public int ID { get; set; }
        public int OrdenID { get; set; }
        public int ProductoID { get; set; }
        public int Cantidad { get; set; }
    }
}
