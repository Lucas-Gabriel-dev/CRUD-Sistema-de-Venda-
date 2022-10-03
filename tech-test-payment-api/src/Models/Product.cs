namespace tech_test_payment_api.src.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        // public ICollection<Sale> Sale { get; set; }
        public ICollection<ProductSale> ProductSales { get; set; }
    }
}