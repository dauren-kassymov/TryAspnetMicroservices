using System.Diagnostics.CodeAnalysis;
using Catalog.API.Entities;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Catalog.API.Data
{
    public class CatalogContext : ICatalogContext
    {
        public CatalogContext(IOptions<DatabaseSettings> options)
        {
            var dbSettings = options.Value;
            var client = new MongoClient(dbSettings.ConnectionString);
            var db = client.GetDatabase(dbSettings.DatabaseName);
            
            Products = db.GetCollection<Product>(dbSettings.CollectionName);
            CatalogContextSeed.Seed(Products);
        }
        
        public IMongoCollection<Product> Products { get; }

        
        [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
        public class DatabaseSettings
        {
            public string ConnectionString { get; set; }
            public string DatabaseName { get; set; }
            public string CollectionName { get; set; }
        }
    }
}