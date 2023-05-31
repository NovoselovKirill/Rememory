using MongoDB.Driver;
using Rememory.Persistence.Client;
using Rememory.Persistence.Entities;
using Rememory.Persistence.Repositories.Base;

namespace Rememory.Persistence.Repositories.JourneyRepository;

public class JourneyRepository : BaseRepository<Journey>, IJourneyRepository
{
    private const string CollectionName = "journeys";

    public JourneyRepository(IDatabaseClient databaseClient) : base(databaseClient, CollectionName)
    {
    }

    public Task<List<Journey>> GetByUserAsync(Guid userId)
        => MongoCollection.Find(journey => journey.UserId == userId).ToListAsync();

    public Task<Journey> GetByDateAndUserAsync(DateTime dateTime, Guid userId)
        => MongoCollection
            .Find(journey => 
                userId == journey.UserId
                && journey.Start <= dateTime 
                && dateTime <= journey.End)
            .FirstOrDefaultAsync();

}