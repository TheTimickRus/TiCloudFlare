namespace TiCloudFlareConfig.Console.Services.LicGenerate.Models.Requests;

public record RegRequestParams
{
    public string? Id { get; set; }
    public string? Token { get; set; }
    public string? Key { get; set; }
    public string? License { get; set; }
    public string? SecondId { get; set; }
    public string? SecondToken { get; set; }
    
}