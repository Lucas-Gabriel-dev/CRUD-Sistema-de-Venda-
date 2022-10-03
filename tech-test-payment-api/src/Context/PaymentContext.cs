using Microsoft.EntityFrameworkCore;
using tech_test_payment_api.src.Models;

namespace tech_test_payment_api.src.Context
{
    public class PaymentContext : DbContext
    {
        public PaymentContext(DbContextOptions<PaymentContext> options) : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductSale>()
                .HasKey(bc => new { bc.IdSale, bc.IdProduct });  
            modelBuilder.Entity<ProductSale>()
                .HasOne(bc => bc.Product)
                .WithMany(b => b.ProductSales)
                .HasForeignKey(bc => bc.IdProduct);  
            modelBuilder.Entity<ProductSale>()
                .HasOne(bc => bc.Sale)
                .WithMany(c => c.ProductSales)
                .HasForeignKey(bc => bc.IdSale);

            modelBuilder.Entity<Sale>()
            .HasOne<Seller>(s => s.Seller)
            .WithMany(g => g.Sales)
            .HasForeignKey(s => s.ResponsibleSellerId);

        //     modelBuilder.Entity<ProductSale>().HasKey(sc => new { sc.IdSale, sc.IdProduct });
        }

        public DbSet<Seller> Sellers { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductSale> ProductSales { get; set; }
    }
}