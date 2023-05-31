using MongoDB.Driver;

namespace Rememory.Persistence.Client;

public interface IDatabaseClient
{
    IClientSessionHandle GetSession();
    IMongoCollection<T> GetCollection<T>(string collectionName);
}