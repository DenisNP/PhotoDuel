using System.Linq;
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
    }
}