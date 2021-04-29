using System.Diagnostics.CodeAnalysis;
using Catalog.API.Entities;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace Catalog.API.Data
{
    public class CatalogContext : ICatalogContext
    {
        public CatalogContext(IConfiguration config)
        {
            var dbSettings = config.Get<DatabaseSettings>();
            var client = new MongoClient(dbSettings.ConnectionString);
            var db = client.GetDatabase(dbSettings.DatabaseName);
            
            Products = db.GetCollection<Product>(dbSettings.CollectionName);
            CatalogContextSeed.Seed(Products);
        }
        
        public IMongoCollection<Product> Products { get; }

        
        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
        [SuppressMessage("ReSharper", "ClassNeverInstantiated.Local")]
        private class DatabaseSettings
        {
            public string ConnectionString { get; set; }
            public string DatabaseName { get; set; }
            public string CollectionName { get; set; }
        }
    }
}