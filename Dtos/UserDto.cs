using System.Text.Json.Serialization;

namespace UserCacheApi.Dtos
{
    public class UserDto
    {
        [JsonPropertyName("id")]
        public int Id { get; init; }

        [JsonPropertyName("name")]
        public string? Name { get; init; }

        [JsonPropertyName("username")]
        public string? Username { get; init; }

        [JsonPropertyName("email")]
        public string? Email { get; init; }

        [JsonPropertyName("phone")]
        public string? Phone { get; init; }

        [JsonPropertyName("website")]
        public string? Website { get; init; }

        [JsonPropertyName("company")]
        public CompanyDto? Company { get; init; }

        [JsonPropertyName("address")]
        public AddressDto? Address { get; init; }
    }

    public class AddressDto
    {
        [JsonPropertyName("city")]
        public string? City { get; init; }
    }

    public class CompanyDto
    {
        [JsonPropertyName("name")]
        public string? Name { get; init; }
    }
}
