#nullable enable 

// ReSharper disable UnusedAutoPropertyAccessor.Global

using System;

namespace TiCloudFlareConfig.WPF.Models.Pages.Configs;

public record ConfigItem
{
    public Guid Id { get; init; }
    public string? Title { get; init; }
    public DateTime? CreationAt { get; init; }
    public bool IsWarpPlus { get; init; }
    public string? FileConfig { get; init; }
    public string? FileToml { get; init; }
}