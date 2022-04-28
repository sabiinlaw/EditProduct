using System.Collections.Generic;
using System.Linq;

namespace EditProductApp
{
    public class ProductRepository
    {
        private static ProductRepository repository = new ProductRepository();
        private List<Product> products = new List<Product>();

        public static ProductRepository GetRepository()
        {
            return repository;
        }

        public List<Product> GetAllProducts()
        {
            return products;
        }

        public Product GetProduct(int productId)
        {
            return products.FirstOrDefault(x=>x.ProductId == productId);
        }
        public void AddProduct(Product product)
        {
            products.Add(product);
        }

        public void UpdateProduct(Product product)
        {
            var oldProduct = products.FirstOrDefault(x => x.ProductId == product.ProductId);
            var oldProductIndex = products.IndexOf(oldProduct);
            products.Insert(oldProductIndex, product);
            products.Remove(oldProduct);
        }
    }
}