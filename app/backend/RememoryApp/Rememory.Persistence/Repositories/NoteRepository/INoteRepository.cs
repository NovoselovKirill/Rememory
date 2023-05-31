using Rememory.Persistence.Entities;
using Rememory.Persistence.Repositories.Base;

namespace Rememory.Persistence.Repositories.NoteRepository;

public interface INoteRepository : IRepository<Note>
{
    public Task<List<Note>> GetByJourneyAsync(Guid journeyId);
}