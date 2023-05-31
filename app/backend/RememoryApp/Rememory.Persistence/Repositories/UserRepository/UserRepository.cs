using MongoDB.Driver;
using Rememory.Persistence.Client;
using Rememory.Persistence.Entities;
using Rememory.Persistence.Repositories.Base;

namespace Rememory.Persistence.Repositories.UserRepository;

public class UserRepository : BaseRepository<User>, IUserRepository
{
    private const string CollectionName = "users";

    public UserRepository(IDatabaseClient databaseClient) : base(databaseClient, CollectionName)
    {
    }
    
    public Task<User?> GetByTelegramIdAsync(long id)
        => MongoCollection
            .Find(entity => entity.TelegramId == id)
            .FirstOrDefaultAsync()!;
}