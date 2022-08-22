using System;

namespace TiCloudFlareConfig.WPF.Models.Pages.Configs;

public record Config
{
    public string? Title { get; set; }
    public DateTime? CreationAt { get; set; }
    public bool IsWarpPlus { get; set; }
    
    public string? FileConfig { get; set; }
    public string? FileToml { get; set; }
}