// ReSharper disable InconsistentNaming

namespace TiCloudFlareConfig.Shared.WireGuardConfig.Models;

public record KeysResponse
{
    public List<string> WarpKeys { get; set; } = new();
    public List<string> EndPoints { get; set; } = new();
    public List<string> Ports { get; set; } = new();
    public List<string> MTU { get; set; } = new();
}