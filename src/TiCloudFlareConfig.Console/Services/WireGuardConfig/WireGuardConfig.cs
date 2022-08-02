using System.Diagnostics;
using System.Reflection;
using Tomlyn;

namespace TiCloudFlareConfig.Console.Services.WireGuardConfig;

public static class WireGuardConfig
{
    private const string FileName = "Resources\\wgcf.exe";
    
    public static bool RegisterWithoutLicense()
    {
        var tag = $"{DateTime.Now.ToString("s").Replace(':', '-')}_Warp";
        var dir = Directory.CreateDirectory($"Configs\\{tag}");
        var fileToml = Path.Combine(dir.FullName, $"{tag}.toml");
        var fileConf = Path.Combine(dir.FullName, $"{tag}.conf");
        
        ProcStart($"register --accept-tos --config \"{fileToml}\"");
        ProcStart($"generate -p \"{fileConf}\" --config \"{fileToml}\"");
        
        return File.Exists(fileToml) && File.Exists(fileConf);
    }

    public static bool RegisterWithLicense(string license)
    {
        var tag = $"{license}_Warp+";
        var dir = Directory.CreateDirectory($"Configs\\{tag}");
        var fileToml = Path.Combine(dir.FullName, $"{tag}.toml");
        var fileConf = Path.Combine(dir.FullName, $"{tag}.conf");
        
        ProcStart($"register --accept-tos --config \"{fileToml}\"");

        UpdateLicense(fileToml, license);
        ProcStart($"update --config \"{fileToml}\"");
        
        ProcStart($"generate -p \"{fileConf}\" --config \"{fileToml}\"");

        return File.Exists(fileToml) && File.Exists(fileConf);
    }
    
    private static void ProcStart(string args)
    {
        if (!File.Exists(FileName))
            ExtractResourceToFile("TiCloudFlareConfig.Console", "Assets", Path.GetFileName(FileName), "Resources");
        
        var proc = Process.Start(
            new ProcessStartInfo($"{FileName}", args)
            {
                RedirectStandardError = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                UseShellExecute = false
            });
        
        proc?.WaitForExit();
    }

    private static void UpdateLicense(string fileToml, string license)
    {
        var toml = Toml.ToModel(File.ReadAllText(fileToml));
        toml["license_key"] = license;
        File.WriteAllText(fileToml, Toml.FromModel(toml));
    }
    
    private static void ExtractResourceToFile(
        string nameSpace, 
        string internalFilePath, 
        string resourceName,
        string outDirectory 
        )
    {
        if (!Directory.Exists(outDirectory))
            Directory.CreateDirectory(outDirectory);
        
        var assembly = Assembly.GetCallingAssembly();
        using var s = assembly.GetManifestResourceStream(nameSpace + "." + (internalFilePath == "" ? "" : internalFilePath + ".") + resourceName);
        if (s == null) 
            return;
        
        using var r = new BinaryReader(s);
        using var fs = new FileStream(outDirectory + "\\" + resourceName, FileMode.OpenOrCreate);
        using var w = new BinaryWriter(fs);
        w.Write(r.ReadBytes((int)s.Length));
    }
}