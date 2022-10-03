namespace tech_test_payment_api.src.Models
{
    public class ProductSale
    {
        public int IdSale { get; set; }
        public Sale Sale { get; set; }
        public int IdProduct { get; set; }
        public Product Product { get; set; }
    }
}