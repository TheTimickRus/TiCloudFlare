namespace TiCloudFlareConfig.Shared.WireGuardConfig.Models;

public record WireGuardConfigParams
{
    public string? License { get; set; }
    public string? EndPoint { get; set; }
    public string? EndPointPort { get; set; }
    public string? Mtu { get; set; }
}