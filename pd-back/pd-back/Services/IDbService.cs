using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.Extensions.Logging;
using PhotoDuel.Models;

namespace PhotoDuel.Services
{
    public interface IDbService
    {
        void Init(string dbName, ILogger logger);
        
        IQueryable<T> Collection<T>(string name);
        
        void Update<T>(string collection, T document) where T : IIdentity;
        
        void UpdateAsync<T>(string collection, T document) where T : IIdentity;
        
        void PushAsync<TDocument, TItem>(
            string collection,
            string docId,
            Expression<Func<TDocument, IEnumerable<TItem>>> expression,
            TItem value
        ) where TDocument : IIdentity;
        
        void DeleteAsync<T>(string collection, string id) where T : IIdentity;
    }
}