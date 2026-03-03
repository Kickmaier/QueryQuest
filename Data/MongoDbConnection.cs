using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using MongoDB.Driver.Encryption;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryQuest.Data
{
    internal class MongoDbConnection
    {
        private readonly IMongoDatabase _database;

        public MongoDbConnection(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("MongoDbConnection");

            var client = new MongoClient(connectionString);

            _database = client.GetDatabase("QueryQuest");
        }
        public IMongoCollection<T> GetCollection<T>(string collectionName)
        {
            return _database.GetCollection<T>(collectionName);
        }
    }
}
