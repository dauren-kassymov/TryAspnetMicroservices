using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Catalog.API.Data;
using Catalog.API.Entities;
using MongoDB.Driver;

namespace Catalog.API.Repos
{
    public class ProductRepo : IProductRepo
    {
        private readonly ICatalogContext _context;

        public ProductRepo(ICatalogContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<Product>> GetProducts()
        {
            return await _context
                .Products
                .Find(x => true)
                .ToListAsync();
        }

        public async Task<Product> GetProduct(string id)
        {
            return await _context
                .Products
                .Find(x => x.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Product>> GetProductByName(string name)
        {
            var filter = Builders<Product>.Filter.ElemMatch()
            return await _context
                .Products
                .Find(filter)
                .ToListAsync();
        }

        public Task<IEnumerable<Product>> GetProductByCategory(string category)
        {
            throw new System.NotImplementedException();
        }

        public Task CreateProduct(Product product)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> UpdateProduct(Product product)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> DeleteProduct(string id)
        {
            throw new System.NotImplementedException();
        }
    }
}