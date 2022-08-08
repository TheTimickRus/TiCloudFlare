namespace TiCloudFlareConfig.Shared.WireGuardConfig.Models;

public record WireGuardConfigParams
{
    public bool IsLicGenerate { get; set; }
    public string? License { get; set; }
    public string? EndPoint { get; set; }
    public string? Port { get; set; }
    public string? Mtu { get; set; }
}