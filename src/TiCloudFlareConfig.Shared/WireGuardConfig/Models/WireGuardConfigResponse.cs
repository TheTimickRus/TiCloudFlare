namespace TiCloudFlareConfig.Shared.WireGuardConfig.Models;

public record WireGuardConfigResponse
{
    public string? FileToml { get; set; }
    public string? FileConfig { get; set; }
}