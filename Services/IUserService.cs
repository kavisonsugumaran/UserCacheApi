using UserCacheApi.Models;

namespace UserCacheApi.Services
{
    public interface IUserService
    {
        Task<IReadOnlyList<User>> GetUsersAsync(CancellationToken cancellationToken);
        Task<User?> GetUserByIdAsync(int id, CancellationToken cancellationToken);
    }
}
