using Microsoft.AspNetCore.Mvc;
using tech_test_payment_api.src.Context;
using tech_test_payment_api.src.Interface;
using tech_test_payment_api.src.Models;

namespace tech_test_payment_api.src.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SaleController : ControllerBase
    {
        private readonly PaymentContext _context;

        public SaleController(PaymentContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        [ActionName(nameof(SeekSale))]
        public IActionResult SeekSale(int id)
        {
            try
            {
                var sale = _context.Sales.Find(id);

                if (sale == null)
                {
                    return NotFound();
                }
                
                var seller = _context.Sellers.Find(sale.ResponsibleSellerId);

                var productsSold = _context.ProductSales.Where(x => x.IdSale == id).ToList();

                List<Product> products = new List<Product>();

                List<AllProducts> allProducts = new List<AllProducts>();

                for (var i = 0; i < productsSold.Count; i++)
                {
                    products.Add(_context.Products.Find(productsSold[i].IdProduct));

                    AllProducts products1 = new AllProducts(products[i].Id, products[i].Name, products[i].Price);

                    allProducts.Add(products1);
                }

                return Ok(new
                {
                    idCompra = id,
                    idVendedor = seller.SellerId,
                    nomeVendedor = seller.Name,
                    emailVendedor = seller.Email,
                    statusCompra = sale.Status,
                    dataCompra = sale.SaleDate.ToString("dd/MM/yyyy HH:mm"),
                    produtosComprados = allProducts
                });
            }
            catch (System.Exception error)
            {
                return BadRequest(new {
                    msg = $"Não foi possível realizar o cadastro, erro: {error}"
                });
            }
        }

        [HttpGet("ConsultarVendedor/{id}")]
        public IActionResult ConsultSeller(int id)
        {
            var seller = _context.Sellers.Find(id);

            if(seller == null){
                return NotFound();
            }

            return Ok(new
            {
                idVendedor = seller.SellerId,
                nomeVendedor = seller.Name,
                emailVendedor = seller.Email
            });
        }

        [HttpGet("ConsultarProduto/{id}")]
        public IActionResult ConsultProduct(int id)
        {
            var product = _context.Products.Find(id);

            return Ok(new
            {
                productId = product.Id,
                productName = product.Name,
                productPrice = product.Price
            });
        }

        [HttpGet("ConsultarTodosProduto")]
        public IActionResult ConsultAllProduct()
        {
            var product = _context.Products.ToList();

            List<AllProducts> allProducts = new List<AllProducts>();
            List<Product> products = new List<Product>();

            for (var i = 0; i < product.Count; i++)
            {
                products.Add(_context.Products.Find(product[i].Id));

                AllProducts products1 = new AllProducts(products[i].Id, products[i].Name, products[i].Price);

                allProducts.Add(products1);
            }


            return Ok(new
            {
                allProducts
            });
        }

        [HttpPost("AdicionarVendedor")]
        public IActionResult AddSeller(
            string name,
            string cpf,
            string email,
            string telephone
        )
        {
            try
            {
                Console.WriteLine($"Lucas");
                Seller newSeller = new Seller();

                newSeller.Name = name;
                newSeller.Cpf = cpf;
                newSeller.Email = email;
                newSeller.Telephone = telephone;

                _context.Sellers.Add(newSeller);
                
                
                
                _context.SaveChanges();

                return CreatedAtAction(nameof(ConsultSeller), new { id = newSeller.SellerId }, newSeller);
            }
            catch (System.Exception error)
            {
                return BadRequest(new {
                    msg = $"Não foi possível realizar o cadastro, erro: {error}"
                });
            }
        }

        [HttpPost("AdicionarProduto")]
        public IActionResult AddProduct(string productName, decimal valueProduct)
        {
            try
            {
                Product products = new Product();

                products.Name = productName;
                products.Price = valueProduct;

                _context.Products.Add(products);
                _context.SaveChanges();

               return ConsultProduct(products.Id);
            }
            catch (System.Exception error)
            {
                return BadRequest(new 
                {
                    msg = $"Não foi possível realizar o cadastro, erro: {error}"
                });
            }
        }

        [HttpPost("Venda")]
        public IActionResult Venda(int idSeller, List<int> idProducts){
            try
            {
                if(idProducts == null || idProducts[0] == 0){
                    return BadRequest(new
                    {
                        msg = "Para realizar a venda é necessário adicionar ao menos " +
                        "1 item com ID maior que 0"
                    });
                }

                Sale newSale = new Sale();
                newSale.Seller = _context.Sellers.Find(idSeller);
                newSale.Status = "Aguardando pagamento";
                newSale.SaleDate = DateTime.Now;

                ProductSale productSales = new ProductSale();
                
                productSales.Sale = newSale;
                productSales.IdSale = newSale.Id;

                // Verifica se os itens escolhidos existem
                foreach (var item in idProducts)
                {
                    if(_context.Products.Find(item) == null){
                        return BadRequest(new { Erro = $"O produto de id: {item} não existe!" });
                    }
                }

                foreach (var item in idProducts)
                {  
                    productSales.IdProduct = item;
                    productSales.Product = _context.Products.Find(item);
                    
                    _context.ProductSales.Add(productSales);
                    _context.SaveChanges();
                }

                return SeekSale(newSale.Id);
            }
            catch (System.Exception error)
            {
                return BadRequest(new 
                {
                    msg = $"Não foi possível realizar a venda, erro: {error.Message}"
                });
            }
        }

        [HttpPut("AtualizarStatusVenda/{id}")]
        public IActionResult UpdateStatusSale(int id, string status)
        {
            var sale = _context.Sales.Find(id);

            string infoStatus = "";

            if(sale.Status == "Aguardando pagamento")
            {
                if(status == "Pagamento Aprovado" || status == "Cancelada")
                {
                    sale.Status = status;
                    
                    _context.Sales.Update(sale);
                    _context.SaveChanges();

                    return SeekSale(id);
                }
                else
                {
                    infoStatus = "Pagamento Aprovado ou Cancelada.";
                };
            }

            if(sale.Status == "Pagamento Aprovado")
            {
                if(status == "Enviado para Transportadora" || status == "Cancelada")
                {
                    sale.Status = status;
                    
                    _context.Sales.Update(sale);
                    _context.SaveChanges();

                    return SeekSale(id);
                }
                else
                {
                    infoStatus = "Enviado para Transportadora ou Cancelada.";
                }
            }

            if(sale.Status == "Enviado para Transportadora")
            {
                if(status == "Entregue")
                {
                    sale.Status = status;
                    
                    _context.Sales.Update(sale);
                    _context.SaveChanges();

                    return SeekSale(id);
                }
                else
                {
                    infoStatus = "Entregue.";
                }
            }

            if(infoStatus != "")
            {
                return BadRequest(new
                {
                    msg = "Você só pode alterar o status para " + infoStatus
                });
            }

            else
            {
                return BadRequest(new
                {
                    msg = "Não é possível atualizar o status da venda!"
                });
            }
        }

        [HttpDelete("DeletarProduto/{id}")]
        public IActionResult DeleteProduct(int id)
        { 
            var sale = _context.Products.Find(id);

            if(sale == null){
                return NotFound(new {
                    msg = "Esse produto não existe"
                });
            }

            _context.Products.Remove(sale);
            _context.SaveChanges();

            return Ok(new
            {
                msg = $"Produto de id {id} deletado!"
            });
        }
    }
}