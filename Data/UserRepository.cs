using Microsoft.Data.SqlClient;
using UserCacheApi.Models;

namespace UserCacheApi.Data
{
    public class UserRepository(IConfiguration configuration) : IUserRepository
    {
        private readonly string _connectionString = configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");

        public async Task<IReadOnlyList<User>> GetAllAsync(CancellationToken cancellationToken)
        {
            const string sql = """
            SELECT Id, Name, Username, Email, Phone, Website, CompanyName, City, LastFetchedAt
            FROM dbo.Users
            ORDER BY Id;
            """;

            await using var connection = new SqlConnection(_connectionString);
            await using var command = new SqlCommand(sql, connection);
            await connection.OpenAsync(cancellationToken);
            await using var reader = await command.ExecuteReaderAsync(cancellationToken);

            var users = new List<User>();
            while (await reader.ReadAsync(cancellationToken))
            {
                users.Add(MapUser(reader));
            }

            return users;
        }

        public async Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            const string sql = """
            SELECT Id, Name, Username, Email, Phone, Website, CompanyName, City, LastFetchedAt
            FROM dbo.Users
            WHERE Id = @Id;
            """;

            await using var connection = new SqlConnection(_connectionString);
            await using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Id", id);

            await connection.OpenAsync(cancellationToken);
            await using var reader = await command.ExecuteReaderAsync(cancellationToken);

            return await reader.ReadAsync(cancellationToken) ? MapUser(reader) : null;
        }

        public async Task UpsertAsync(User user, CancellationToken cancellationToken)
        {
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);
            await UpsertAsync(user, connection, null, cancellationToken);
        }

        public async Task UpsertManyAsync(IEnumerable<User> users, CancellationToken cancellationToken)
        {
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);
            await using var transaction = await connection.BeginTransactionAsync(cancellationToken);

            try
            {
                foreach (var user in users)
                {
                    await UpsertAsync(user, connection, (SqlTransaction)transaction, cancellationToken);
                }

                await transaction.CommitAsync(cancellationToken);
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }

        private static User MapUser(SqlDataReader reader)
        {
            return new User
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Username = reader.GetString(2),
                Email = reader.GetString(3),
                Phone = reader.IsDBNull(4) ? null : reader.GetString(4),
                Website = reader.IsDBNull(5) ? null : reader.GetString(5),
                CompanyName = reader.IsDBNull(6) ? null : reader.GetString(6),
                City = reader.IsDBNull(7) ? null : reader.GetString(7),
                LastFetchedAt = reader.GetDateTimeOffset(8)
            };
        }

        private static async Task UpsertAsync(
        User user,
        SqlConnection connection,
        SqlTransaction? transaction,
        CancellationToken cancellationToken)
        {
            const string sql = """
            UPDATE dbo.Users
            SET Name = @Name,
                Username = @Username,
                Email = @Email,
                Phone = @Phone,
                Website = @Website,
                CompanyName = @CompanyName,
                City = @City,
                LastFetchedAt = @LastFetchedAt
            WHERE Id = @Id;

            IF @@ROWCOUNT = 0
            BEGIN
                INSERT INTO dbo.Users (Id, Name, Username, Email, Phone, Website, CompanyName, City, LastFetchedAt)
                VALUES (@Id, @Name, @Username, @Email, @Phone, @Website, @CompanyName, @City, @LastFetchedAt);
            END;
            """;

            await using var command = new SqlCommand(sql, connection, transaction);
            command.Parameters.AddWithValue("@Id", user.Id);
            command.Parameters.AddWithValue("@Name", user.Name);
            command.Parameters.AddWithValue("@Username", user.Username);
            command.Parameters.AddWithValue("@Email", user.Email);
            command.Parameters.AddWithValue("@Phone", (object?)user.Phone ?? DBNull.Value);
            command.Parameters.AddWithValue("@Website", (object?)user.Website ?? DBNull.Value);
            command.Parameters.AddWithValue("@CompanyName", (object?)user.CompanyName ?? DBNull.Value);
            command.Parameters.AddWithValue("@City", (object?)user.City ?? DBNull.Value);
            command.Parameters.AddWithValue("@LastFetchedAt", user.LastFetchedAt);

            await command.ExecuteNonQueryAsync(cancellationToken);
        }

    }
}
