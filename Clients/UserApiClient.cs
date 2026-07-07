using System.Net;
using UserCacheApi.Dtos;
using UserCacheApi.Models;

namespace UserCacheApi.Clients
{
    public class UserApiClient(HttpClient httpClient, ILogger<UserApiClient> logger) : IUserApiClient
    {
        public async Task<IReadOnlyList<User>> GetAllAsync(CancellationToken cancellationToken)
        {
            var response = await httpClient.GetAsync("users", cancellationToken);
            response.EnsureSuccessStatusCode();

            var users = await response.Content.ReadFromJsonAsync<List<UserDto>>(cancellationToken);
            return users?.Select(ToUserRecord).Where(user => user is not null).Cast<User>().ToList()
                ?? [];
        }

        public async Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            var response = await httpClient.GetAsync($"users/{id}", cancellationToken);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            response.EnsureSuccessStatusCode();

            var user = await response.Content.ReadFromJsonAsync<UserDto>(cancellationToken);
            if (user is null)
            {
                logger.LogWarning("JSONPlaceholder returned an empty result for user ID {UserId}.", id);
                return null;
            }

            return ToUserRecord(user);
        }

        private static User? ToUserRecord(UserDto dto)
        {
            if (dto.Id <= 0 || string.IsNullOrWhiteSpace(dto.Name) || string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Email))
            {
                return null;
            }

            return new User
            {
                Id = dto.Id,
                Name = dto.Name,
                Username = dto.Username,
                Email = dto.Email,
                Phone = dto.Phone,
                Website = dto.Website,
                CompanyName = dto.Company?.Name,
                City = dto.Address?.City,
                LastFetchedAt = DateTimeOffset.UtcNow
            };
        }
    }
}
