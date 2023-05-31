using Rememory.Persistence.Entities;
using Rememory.Persistence.Repositories.Base;

namespace Rememory.Persistence.Repositories.JourneyRepository;

public interface IJourneyRepository : IRepository<Journey>
{
    public Task<List<Journey>> GetByUserAsync(Guid userId);
    public Task<Journey> GetByDateAndUserAsync(DateTime dateTime, Guid userId);
}