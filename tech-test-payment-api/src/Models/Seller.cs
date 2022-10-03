namespace tech_test_payment_api.src.Models
{
    public class Seller
    {
        public int SellerId { get; set; }
        public string Name { get; set; }
        public string Cpf { get; set; }
        public string Email { get; set; }
        public string Telephone { get; set; }
        public ICollection<Sale> Sales { get; set; }
    }
}