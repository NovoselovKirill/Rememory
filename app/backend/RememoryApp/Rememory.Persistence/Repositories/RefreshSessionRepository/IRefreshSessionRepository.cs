using Rememory.Persistence.Entities;
using Rememory.Persistence.Repositories.Base;

namespace Rememory.Persistence.Repositories.RefreshSessionRepository;

public interface IRefreshSessionRepository : IRepository<RefreshSession>
{ 
    public Task CreateNewAndRemoveOldAsync(RefreshSession refreshSession);
}