using System;
using System.Collections.Generic;
using System.Linq;
using LiteDB;
using TiCloudFlareConfig.WPF.Models.Pages.Configs;

namespace TiCloudFlareConfig.WPF.Services.Database;

public class DataBaseService : IDataBaseService
{
    private readonly ILiteCollection<Config> _configCollection;

    public DataBaseService()
    {
        _configCollection = new LiteDatabase("Resources\\configs.db")
            .GetCollection<Config>("configs");
    }

    public void AddConfig(Config config)
    {
        _configCollection
            .Insert(Guid.NewGuid(), config);
    }

    public void RemoveConfig(Guid id)
    {
        _configCollection
            .Delete(id);
    }

    public List<Config> FetchAllConfigs()
    {
        return _configCollection
            .Find(Query.All())
            .ToList();
    }

    public void RemoveAllConfigs()
    {
        _configCollection
            .DeleteAll();
    }
}