using MongoDB.Driver;
using Rememory.Persistence.Client;
using Rememory.Persistence.Entities;
using Rememory.Persistence.Repositories.Base;

namespace Rememory.Persistence.Repositories.RefreshSessionRepository;

public class RefreshSessionRepository : BaseRepository<RefreshSession>, IRefreshSessionRepository
{
    private const string CollectionName = "refreshSessions";
    
    public RefreshSessionRepository(IDatabaseClient databaseClient) 
        : base(databaseClient, CollectionName)
    {
    }

    public async Task CreateNewAndRemoveOldAsync(RefreshSession refreshSession)
    {
        var userId = refreshSession.UserId;
        var deviceId = refreshSession.DeviceId;

        using var session = MongoClient.GetSession();
        session.StartTransaction();

        try
        {
            await MongoCollection.DeleteOneAsync(s => s.UserId == userId && s.DeviceId == deviceId);

            await MongoCollection.InsertOneAsync(refreshSession);
                
            await session.CommitTransactionAsync();
        }
        catch (Exception)
        {
            await session.AbortTransactionAsync();
            throw;
        }
    }
}