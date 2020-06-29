using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using PhotoDuel.Models;

namespace PhotoDuel.Services
{
    public class MongoService : IDbService
    {
        private IMongoDatabase _db;
        
        public void Init(string dbName, ILogger logger)
        {
            var connectionUrl = Environment.GetEnvironmentVariable("MONGO_URL") ?? $"mongodb://localhost:27017/{dbName}";
            var client = new MongoClient(connectionUrl);
            _db = client.GetDatabase(dbName);

            var isMongoLive = _db.RunCommandAsync((Command<BsonDocument>)"{ping:1}").Wait(5000);
            if (isMongoLive)
            {
                logger.LogInformation($"MongoDB connected: {_db.DatabaseNamespace.DatabaseName}");
            }
            else
            {
                logger.LogError("Cannot reach MongoDB server");
                throw new IOException("Cannot reach MongoDB server");
            }
        }

        public IQueryable<T> Collection<T>(string name)
        {
            return _db.GetCollection<T>(name).AsQueryable();
        }

        public void Update<T>(string collection, T document) where T : IIdentity
        {
            var filter = Builders<T>.Filter.Eq(x => x.Id, document.Id);
            _db.GetCollection<T>(collection).ReplaceOne(filter, document, new ReplaceOptions {IsUpsert = true});
        }

        public void UpdateAsync<T>(string collection, T document) where T : IIdentity
        {
            var filter = Builders<T>.Filter.Eq(x => x.Id, document.Id);
            _db.GetCollection<T>(collection).ReplaceOneAsync(filter, document, new ReplaceOptions {IsUpsert = true});
        }

        public void PushAsync<TDocument, TItem>(
            string collection,
            string docId,
            Expression<Func<TDocument, IEnumerable<TItem>>> expression,
            TItem value
        ) where TDocument : IIdentity
        {
            var update = Builders<TDocument>.Update.Push(expression, value);
            var filter = Builders<TDocument>.Filter.Eq(x => x.Id, docId);
            _db.GetCollection<TDocument>(collection).FindOneAndUpdateAsync(filter, update);
        }

        public void DeleteAsync<T>(string collection, string id) where T : IIdentity
        {
            var filter = Builders<T>.Filter.Eq(x => x.Id, id);
            _db.GetCollection<T>(collection).DeleteOneAsync(filter);
        }
    }
}