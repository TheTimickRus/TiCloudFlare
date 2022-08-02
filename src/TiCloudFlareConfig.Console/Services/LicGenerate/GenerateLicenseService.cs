﻿using System.Text.Json;
using Ardalis.GuardClauses;
using RestSharp;
using TiCloudFlareConfig.Console.Services.LicGenerate.Models.Requests;
using TiCloudFlareConfig.Console.Services.LicGenerate.Models.Responses;

namespace TiCloudFlareConfig.Console.Services.LicGenerate;

public class GenerateLicenseService
{
    private readonly RestClient _client = new("https://api.cloudflareclient.com/v0a2405");
    private readonly IDictionary<string, string> _headers = new Dictionary<string, string>(new[]
    {
        new KeyValuePair<string, string>("CF-Client-Version", "a-6.15-2405"),
        new KeyValuePair<string, string>("Host", "api.cloudflareclient.com"),
        new KeyValuePair<string, string>("Connection", "Keep-Alive"),
        new KeyValuePair<string, string>("Accept-Encoding", "gzip"),
        new KeyValuePair<string, string>("User-Agent", "okhttp/3.12.1")
    });
    private readonly IList<string> _keys = new List<string>
    {
        "Io935xs2-7z1P65Wi-93wC5Hi2",
        "d8D2tB46-1ZVB4G75-EFoy6817",
        "m9054gFd-s7r302Wm-Yd5e310W",
        "mE4370Tp-37uI5QX0-1520FzTu",
        "5Ij3k9x8-z89X03Bf-8I046Ttk",
        "7K142EYT-5h14ZJj9-7nfq46a5",
        "69R14Lgr-3217hSGw-F6y741jR",
        "23t1Zw8K-V9MR4v58-79G4oU6u",
        "9YDz3x04-970w6ptv-cC27y38t",
        "q7Wr91K4-5Mp8yT97-qtjS6109",
    };
    
    /// <summary>
    /// Генерация лицензионного ключа
    /// </summary>
    /// <returns></returns>
    public RegAccountResponse? Generate()
    {
        /* Создаем 2 аккаунта: основной и реферальный */
        var mainAccount = RegPost();
        var referrerAccount = RegPost();

        Guard.Against.Null(mainAccount);
        Guard.Against.Null(referrerAccount);
        /* Создаем 2 аккаунта: основной и реферальный */
        
        /* Добавляем реферальный аккаунт к основному */
        RegPatch(
            new RegRequestParams
            {
                Id = mainAccount.Id, 
                Token = mainAccount.Token,
                SecondId = referrerAccount.Id
            });
        /* Добавляем реферальный аккаунт к основному */
        
        /* Удаляем реферальный аккаунт */
        RegDelete(
            new RegRequestParams
            {
                Id = referrerAccount.Id,
                Token = referrerAccount.Token
            });
        /* Удаляем реферальный аккаунт */
        
        /* Добавляем ключ и лицензию для основного аккаунта */
        RegPut(new RegRequestParams
        {
            Id = mainAccount.Id,
            Token = mainAccount.Token,
            License = mainAccount.Account.License
        });
        /* Добавляем ключ и лицензию для основного аккаунта */

        /* Получаем данные основного аккаунта */
        var mainAccountInfo = RegGet(new RegRequestParams
        {
            Id = mainAccount.Id,
            Token = mainAccount.Token
        });
        /* Получаем данные основного аккаунта */
        
        /* Удаляем основной аккаунт */
        RegDelete(
            new RegRequestParams
            {
                Id = mainAccount.Id,
                Token = mainAccount.Token
            });
        /* Удаляем основной аккаунт */
        
        return mainAccountInfo;
    }

    private RegResponse? RegPost()
    {
        var request = new RestRequest("/reg", Method.Post);
        request.AddHeaders(_headers);

        var response = _client.Execute(request);
        if (response.IsSuccessful is false) 
            throw new Exception(response.ErrorMessage);
        
        Guard.Against.Null(response.Content);
        return JsonSerializer.Deserialize<RegResponse>(response.Content);
    }

    private void RegPatch(RegRequestParams @params)
    {
        var request = new RestRequest($"/reg/{@params.Id}", Method.Patch);
        request.AddHeaders(_headers);
        request.AddHeader("Authorization", $"Bearer {@params.Token}");
        request.AddJsonBody("{ \"referrer\" : \"" + $"{@params.SecondId}" + "\" }");

        var response = _client.Execute(request);
        if (response.IsSuccessful is false) 
            throw new Exception(response.ErrorMessage);
    }

    private void RegDelete(RegRequestParams @params)
    {
        var request = new RestRequest($"/reg/{@params.Id}", Method.Delete);
        request.AddHeaders(_headers);
        request.AddHeader("Authorization", $"Bearer {@params.Token}");
        
        var response = _client.Execute(request);
        if (response.IsSuccessful is false) 
            throw new Exception(response.ErrorMessage);
    }

    private void RegPut(RegRequestParams @params)
    {
        var isKeySuccess = false;
        
        foreach (var key in _keys)
        {
            var requestKey = new RestRequest($"/reg/{@params.Id}/account", Method.Put);
            requestKey.AddHeaders(_headers);
            requestKey.AddHeader("Authorization", $"Bearer {@params.Token}");
            requestKey.AddJsonBody("{ \"license\" : \"" + $"{key}" + "\" }");
                
            var response = _client.Execute(requestKey);
            if (!response.IsSuccessful) 
                continue;
            
            isKeySuccess = true;
            break;
        }

        if (!isKeySuccess)
            throw new Exception("Нет подходящего ключа...");
        
        var requestLicense = new RestRequest($"/reg/{@params.Id}/account", Method.Put);
        requestLicense.AddHeaders(_headers);
        requestLicense.AddHeader("Authorization", $"Bearer {@params.Token}");
        var licenseJson = "{ \"license\" : \"" + $"{@params.License}" + "\" }";
        requestLicense.AddJsonBody(licenseJson);
        
        var responseLicense = _client.Execute(requestLicense);
        if (responseLicense.IsSuccessful is false) 
            throw new Exception(responseLicense.ErrorMessage);
    }

    private RegAccountResponse? RegGet(RegRequestParams @params)
    {
        var request = new RestRequest($"/reg/{@params.Id}/account");
        request.AddHeaders(_headers);
        request.AddHeader("Authorization", $"Bearer {@params.Token}"); 
        
        var response = _client.Execute(request);
        if (response.IsSuccessful is false) 
            throw new Exception(response.ErrorMessage);
        
        Guard.Against.Null(response.Content);
        return JsonSerializer.Deserialize<RegAccountResponse>(response.Content);
    }
}