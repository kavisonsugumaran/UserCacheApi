using UserCacheApi.Models;

namespace UserCacheApi.Clients
{
    public interface IUserApiClient
    {
        Task<IReadOnlyList<User>> GetAllAsync(CancellationToken cancellationToken);
        Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken);
    }
}
