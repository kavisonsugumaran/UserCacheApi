using Microsoft.EntityFrameworkCore;
using UserCacheApi.Models;

namespace UserCacheApi.Data
{
    public sealed class EfUserRepository(AppDbContext dbContext) : IUserRepository
    {
        public async Task<IReadOnlyList<User>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await dbContext.Users
                .OrderBy(user => user.Id)
                .ToListAsync(cancellationToken);
        }

        public async Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await dbContext.Users.FindAsync([id], cancellationToken);
        }

        public async Task UpsertAsync(User user, CancellationToken cancellationToken)
        {
            var exisitingUser = await dbContext.Users.FindAsync([user.Id], cancellationToken);

            if (exisitingUser is null)
            {
                dbContext.Users.Add(user);
            }
            else
            {
                dbContext.Entry(exisitingUser).CurrentValues.SetValues(user);
            }

            await dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task UpsertManyAsync(IEnumerable<User> users, CancellationToken cancellationToken)
        {
            foreach (var user in users)
            {
                var existingUser = await dbContext.Users.FindAsync([user.Id], cancellationToken);

                if (existingUser is null)
                {
                    dbContext.Users.Add(user);
                }
                else
                {
                    dbContext.Entry(existingUser).CurrentValues.SetValues(user);
                }
            }

            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}