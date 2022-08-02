using System.Text.Json.Serialization;

namespace TiCloudFlareConfig.Shared.LicGenerate.Models.Responses;

public record RegResponse(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("type")] string Type,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("account")] Account Account,
    [property: JsonPropertyName("token")] string Token,
    [property: JsonPropertyName("warp_enabled")] bool WarpEnabled,
    [property: JsonPropertyName("waitlist_enabled")] bool WaitlistEnabled,
    [property: JsonPropertyName("created")] string Created,
    [property: JsonPropertyName("updated")] string Updated,
    [property: JsonPropertyName("place")] int Place,
    [property: JsonPropertyName("locale")] string Locale,
    [property: JsonPropertyName("enabled")] bool Enabled,
    [property: JsonPropertyName("install_id")] string InstallId
);

public record Account(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("account_type")] string AccountType,
    [property: JsonPropertyName("created")] string Created,
    [property: JsonPropertyName("updated")] string Updated,
    [property: JsonPropertyName("premium_data")] int PremiumData,
    [property: JsonPropertyName("quota")] int Quota,
    [property: JsonPropertyName("usage")] int Usage,
    [property: JsonPropertyName("warp_plus")] bool WarpPlus,
    [property: JsonPropertyName("referral_count")] int ReferralCount,
    [property: JsonPropertyName("referral_renewal_countdown")] int ReferralRenewalCountdown,
    [property: JsonPropertyName("role")] string Role,
    [property: JsonPropertyName("license")] string License
);