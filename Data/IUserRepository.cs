using UserCacheApi.Models;

namespace UserCacheApi.Data
{
    public interface IUserRepository
    {
        Task<IReadOnlyList<User>> GetAllAsync(CancellationToken cancellationToken);
        Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task UpsertAsync(User user, CancellationToken cancellationToken);
        Task UpsertManyAsync(IEnumerable<User> users, CancellationToken cancellationToken);
    }
}
