namespace UserCacheApi.Models
{
    public class User
    {
        public int Id { get; init; }
        public required string Name { get; init; }
        public required string Username { get; init; }
        public required string Email { get; init; }
        public string? Phone { get; init; }
        public string? Website { get; init; }
        public string? CompanyName { get; init; }
        public string? City { get; init; }
        public DateTimeOffset LastFetchedAt { get; init; }
    }
}
