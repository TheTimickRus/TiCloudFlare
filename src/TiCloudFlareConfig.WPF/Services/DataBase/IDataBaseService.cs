using System;
using System.Collections.Generic;
using TiCloudFlareConfig.WPF.Models.Pages.Configs;

namespace TiCloudFlareConfig.WPF.Services.Database;

public interface IDataBaseService
{
    /// <summary>
    /// Добавить новую конфигурацию в БД
    /// </summary>
    /// <param name="configItem">Элемент</param>
    void AddConfig(ConfigItem configItem);
    /// <summary>
    /// Удалить существующую конфигурацию из БД
    /// </summary>
    /// <param name="id">ID Конфигурации</param>
    void RemoveConfig(Guid id);
    /// <summary>
    /// Получить все конфигурации
    /// </summary>
    /// <returns>Список конфигураций</returns>
    List<ConfigItem> FetchAllConfigs();
    /// <summary>
    /// Удалить все конфигурации
    /// </summary>
    void RemoveAllConfigs();
}