using System.Text.Json.Serialization;

namespace TiCloudFlareConfig.Console.Services.LicGenerate.Models.Responses;

public record RegAccountResponse(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("account_type")] string AccountType,
    [property: JsonPropertyName("created")] DateTime Created,
    [property: JsonPropertyName("updated")] string Updated,
    [property: JsonPropertyName("premium_data")] long PremiumData,
    [property: JsonPropertyName("quota")] long Quota,
    [property: JsonPropertyName("warp_plus")] bool WarpPlus,
    [property: JsonPropertyName("referral_count")] int ReferralCount,
    [property: JsonPropertyName("referral_renewal_countdown")] int ReferralRenewalCountdown,
    [property: JsonPropertyName("role")] string Role,
    [property: JsonPropertyName("license")] string License
);