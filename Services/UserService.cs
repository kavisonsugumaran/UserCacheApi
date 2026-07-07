using UserCacheApi.Clients;
using UserCacheApi.Data;
using UserCacheApi.Models;

namespace UserCacheApi.Services
{
    public class UserService(IUserRepository userRepository, IUserApiClient userApiClient ) : IUserService
    {
        public async Task<IReadOnlyList<User>> GetUsersAsync(CancellationToken cancellationToken)
        {
            var cachedUsers = await userRepository.GetAllAsync(cancellationToken);
            if (cachedUsers.Count > 0)
            {
                return cachedUsers;
            }

            var freshUsers = await userApiClient.GetAllAsync(cancellationToken);
            await userRepository.UpsertManyAsync(freshUsers, cancellationToken);
            return freshUsers.OrderBy(user => user.Id).ToList();
        }

        public async Task<User?> GetUserByIdAsync(int id, CancellationToken cancellationToken)
        {
            var cachedUser = await userRepository.GetByIdAsync(id, cancellationToken);
            if (cachedUser is not null)
            {
                return cachedUser;
            }

            var freshUser = await userApiClient.GetByIdAsync(id, cancellationToken);
            if (freshUser is null)
            {
                return null;
            }

            await userRepository.UpsertAsync(freshUser, cancellationToken);
            return freshUser;
        }
    }
}
