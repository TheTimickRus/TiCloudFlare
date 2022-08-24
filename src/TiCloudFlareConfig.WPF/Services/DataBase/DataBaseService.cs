using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LiteDB;
using TiCloudFlareConfig.WPF.Models.Pages.Configs;

namespace TiCloudFlareConfig.WPF.Services.Database;

public class DataBaseService : IDataBaseService
{
    private readonly ILiteCollection<ConfigItem> _configCollection;

    public DataBaseService()
    {
        if (!Directory.Exists("Resources"))
            Directory.CreateDirectory("Resources");
        
        _configCollection = new LiteDatabase("Resources\\configs.db")
            .GetCollection<ConfigItem>("configs");
    }

    public void AddConfig(ConfigItem configItem)
    {
        _configCollection
            .Insert(Guid.NewGuid(), configItem);
    }

    public void RemoveConfig(Guid id)
    {
        _configCollection
            .Delete(id);
    }

    public List<ConfigItem> FetchAllConfigs()
    {
        return _configCollection
            .Find(Query.All())
            .OrderByDescending(item => item.CreationAt)
            .ToList();
    }

    public void RemoveAllConfigs()
    {
        _configCollection
            .DeleteAll();
    }
}