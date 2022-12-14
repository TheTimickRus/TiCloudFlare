// ReSharper disable MemberCanBePrivate.Global

using System.Diagnostics;
using System.Reflection;
using Ardalis.GuardClauses;
using ICSharpCode.SharpZipLib.Zip;
using SharpCompress.Archives;
using SharpCompress.Archives.SevenZip;
using SharpCompress.Common;
using SharpCompress.Readers;
using SharpCompress.Writers;
using TiCloudFlareConfig.Shared.WireGuardConfig.Models;
using Tomlyn;

namespace TiCloudFlareConfig.Shared.WireGuardConfig;

public static class WireGuardConfig
{
    #region Constants

    private const string KeysFileName = "Resources\\keys.toml";
    private const string WgCfFileName = "Resources\\wgcf.exe";

    #endregion
    
    #region PublicMethods

    public static bool ExtractResources()
    {
        if (File.Exists(KeysFileName) && File.Exists(WgCfFileName))
            return true;

        try
        {
            ExtractResourceToFile("TiCloudFlareConfig.Shared", "Assets", "Resources.7z", "Resources");

            using var archive = SevenZipArchive.Open("Resources\\Resources.7z");
            foreach (var entry in archive.Entries.Where(entry => !entry.IsDirectory))
                entry.WriteToDirectory("Resources", new ExtractionOptions { ExtractFullPath = true, Overwrite = true });
            archive.Dispose();
        
            File.Delete("Resources\\Resources.7z");

            return true;
        }
        catch
        {
            return false;
        }
    }

    public static KeysResponse FetchKeys()
    {
        return Toml.ToModel<KeysResponse>(File.ReadAllText(KeysFileName));
    }
    
    public static WireGuardConfigResponse Register(WireGuardConfigParams configParams)
    {
        var tagDate = $"{DateTime.Now.ToString("s").Replace(':', '-')}_Warp";
        var tagLic = $"{configParams.License}_Warp+";
        var tag = configParams.License is null ? tagDate : tagLic;
        
        var dir = Directory.CreateDirectory($"Configs\\{tag}");
        var fileToml = Path.Combine(dir.FullName, $"{tag}.toml");
        var fileConf = Path.Combine(dir.FullName, $"{tag}.conf");
        
        var configResponse = new WireGuardConfigResponse
        {
            FileConfig = fileConf,
            FileToml = fileToml
        };
        
        ProcStart($"register --accept-tos --config \"{fileToml}\"");

        if (configParams.License != null)
        {
            UpdateToml(configParams, configResponse);
            ProcStart($"update --config \"{fileToml}\"");
        }
        
        ProcStart($"generate -p \"{fileConf}\" --config \"{fileToml}\"");
        UpdateConfig(configParams, configResponse);

        return configResponse;
    }
    
    public static async Task<WireGuardConfigResponse> RegisterAsync(WireGuardConfigParams configParams)
    {
        return await Task.Run(() => Register(configParams));
    }
    
    public static void SaveAsConfig(WireGuardConfigResponse? configs, string directory, string filename)
    {
        var confFileName = Path.Combine(directory, $"{filename}.conf");
        var tomlFileName = Path.Combine(directory, $"{filename}.toml");

        Guard.Against.Null(configs?.FileConfig);
        Guard.Against.Null(configs.FileToml);
        
        File.Copy(configs.FileConfig, confFileName);
        File.Copy(configs.FileToml, tomlFileName);
        
        DeleteTempFiles();
    }

    public static void SaveAsZip(WireGuardConfigResponse? configs, string directory, string filename)
    {
        Guard.Against.Null(configs?.FileConfig);
        Guard.Against.Null(configs.FileToml);
        
        var zipFileName = Path.Combine(directory, $"{filename}.zip");
        
        using (var zipFile = ZipFile.Create(zipFileName))
        {
            zipFile.BeginUpdate();
            zipFile.Add(configs.FileConfig, $"{Path.GetFileNameWithoutExtension(zipFileName)}.conf");
            zipFile.Add(configs.FileToml, $"{Path.GetFileNameWithoutExtension(zipFileName)}.toml");
            zipFile.CommitUpdate();
            zipFile.Close();
        }
        
        DeleteTempFiles();
    }
    
    public static void DeleteTempFiles()
    {
        if (Directory.Exists("Configs"))
            Directory.Delete("Configs", true);
    }
    
    #endregion

    #region PrivateMethods

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
    
    private static void ProcStart(string args)
    {
        if (!File.Exists(WgCfFileName))
            ExtractResourceToFile("TiCloudFlareConfig.Shared", "Assets", Path.GetFileName(WgCfFileName), "Resources");
        
        var proc = Process.Start(
            new ProcessStartInfo($"{WgCfFileName}", args)
            {
                RedirectStandardError = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            });
        
        proc?.WaitForExit();
    }

    private static void UpdateConfig(WireGuardConfigParams configParams, WireGuardConfigResponse configResponse)
    {
        var config = configResponse.FileConfig;

        if (!File.Exists(config)) 
            return;

        var configStr = File.ReadAllText(config);
        
        configStr = configStr
            .Replace(
                "MTU = 1280", 
                $"MTU = {configParams.Mtu}");
        configStr = configStr
            .Replace(
                "Endpoint = engage.cloudflareclient.com:2408", 
                $"Endpoint = {configParams.EndPoint}:{configParams.Port}");
        
        File.WriteAllText(config, configStr);
    }

    private static void UpdateToml(WireGuardConfigParams configParams, WireGuardConfigResponse configResponse)
    {
        var toml = configResponse.FileToml;

        if (!File.Exists(toml)) 
            return;
        
        var tomlModel = Toml.ToModel(File.ReadAllText(toml));
        tomlModel["license_key"] = $"{configParams.License}";
        File.WriteAllText(toml, Toml.FromModel(tomlModel));
    }
    
    #endregion
}