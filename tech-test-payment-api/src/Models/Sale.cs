namespace tech_test_payment_api.src.Models
{
    public class Sale
    {
        public int Id { get; set; }
        public string Status { get; set; }
        
        public int ResponsibleSellerId { get; set; }
        public Seller Seller { get; set; }

        public ICollection<ProductSale> ProductSales { get; set; }

        public DateTime SaleDate { get; set; }
    }
}