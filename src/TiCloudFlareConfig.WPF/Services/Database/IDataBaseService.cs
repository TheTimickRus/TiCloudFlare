using System;
using System.Collections.Generic;
using TiCloudFlareConfig.WPF.Models.Pages.Configs;

namespace TiCloudFlareConfig.WPF.Services.Database;

public interface IDataBaseService
{
    void AddConfig(Config config);
    void RemoveConfig(Guid id);

    List<Config> FetchAllConfigs();
    void RemoveAllConfigs();
}