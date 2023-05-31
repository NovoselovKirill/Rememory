using Rememory.Persistence.Entities;
using Rememory.Persistence.Repositories.Base;

namespace Rememory.Persistence.Repositories.UserRepository;

public interface IUserRepository : IRepository<User>
{
   public Task<User?> GetByTelegramIdAsync(long id);
}