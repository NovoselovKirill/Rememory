using MongoDB.Driver;
using Rememory.Persistence.Client;
using Rememory.Persistence.Entities;
using Rememory.Persistence.Repositories.Base;

namespace Rememory.Persistence.Repositories.NoteRepository;

public class NoteRepository : BaseRepository<Note>, INoteRepository
{
    private const string CollectionName = "days";

    public NoteRepository(IDatabaseClient databaseClient) : base(databaseClient, CollectionName)
    {
    }

    public Task<List<Note>> GetByJourneyAsync(Guid journeyId)
        => MongoCollection.Find(day => day.JourneyId == journeyId).ToListAsync();
}