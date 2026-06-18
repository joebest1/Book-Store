using System.Text.Json.Serialization;

public class CloudFlareViewModel
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("challenge_ts")]
    public string ChallengeTs { get; set; }

    [JsonPropertyName("hostname")]
    public string Hostname { get; set; }

    [JsonPropertyName("error-codes")]
    public List<string> ErrorCodes { get; set; }
}
