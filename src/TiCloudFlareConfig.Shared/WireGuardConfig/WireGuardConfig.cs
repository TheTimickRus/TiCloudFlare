﻿// ReSharper disable MemberCanBePrivate.Global

using System.Diagnostics;
using System.IO.Compression;
using System.Reflection;
using Ardalis.GuardClauses;
using TiCloudFlareConfig.Shared.WireGuardConfig.Models;
using Tomlyn;
using Tomlyn.Model;

namespace TiCloudFlareConfig.Shared.WireGuardConfig;

public static class WireGuardConfig
{
    #region Constants

    private const string FileName = "Resources\\wgcf.exe";

    #endregion
    
    #region PublicMethods

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
    
    public static void CreateArchive(WireGuardConfigResponse configs, string outFileName)
    {
        var dir = Path.GetDirectoryName(configs.FileConfig);
        Guard.Against.Null(dir);
        
        ZipFile.CreateFromDirectory(dir, outFileName);
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
        if (!File.Exists(FileName))
            ExtractResourceToFile("TiCloudFlareConfig.Shared", "Assets", Path.GetFileName(FileName), "Resources");
        
        var proc = Process.Start(
            new ProcessStartInfo($"{FileName}", args)
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
                $"Endpoint = {configParams.EndPoint}:{configParams.EndPointPort}");
        
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