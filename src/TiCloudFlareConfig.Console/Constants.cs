using System.Diagnostics.CodeAnalysis;
using Spectre.Console;

namespace TiCloudFlareConfig.Console;

public static class Constants
{
    public static class Titles
    {
        /// <summary>
        /// *Название программы* (*Версия* (*дата*)) by *Разработчик*
        /// </summary>
        public const string FullTitle = $"TiCloudFlare.Console ({Version} (20.12.2022)) by TheTimickRus";
        /// <summary>
        /// *Название программы* by *Разработчик*
        /// </summary>
        public const string ShortTitle = "TiCloudFlare by TheTimickRus";
        /// <summary>
        /// *Название программы*
        /// </summary>
        public const string VeryShortTitle = "TiCloudFlare";

        public const string ExecutableName = "TiCloudFlare.Console.exe";
        /// <summary>
        /// Версия программы
        /// </summary>
        public const string Version = "v.1.2";
        /// <summary>
        /// Имя лог-файла
        /// </summary>
        public const string LogFileName = $"{VeryShortTitle}.log";
    }

    [SuppressMessage("Usage", "CA2211:Поля, не являющиеся константами, не должны быть видимыми")]
    public static class Colors
    {
        /// <summary>
        /// Основной цвет
        /// </summary>
        public static Color MainColor = Color.SteelBlue;
        /// <summary>
        /// Цвет успеха
        /// </summary>
        public static Color SuccessColor = Color.SeaGreen1;
        /// <summary>
        /// Цвет ошибки
        /// </summary>
        public static Color ErrorColor = Color.Red; 
    }
}