using System.Reflection;
using Newtonsoft.Json;

namespace ITHock.XarfReportGenerator.Plugin.Utils;

public static class PluginUtilities
{
    public static string? GetPluginDirectory(Type pluginType)
    {
        var mainAssembly = Assembly.GetAssembly(pluginType);
        if (mainAssembly == null)
            return null;

        var assemblyDirectory = Path.GetDirectoryName(mainAssembly.Location);
        return assemblyDirectory ?? null;
    }

    public static string? GetConfigContent(Type pluginType)
    {
        var pluginDirectory = GetPluginDirectory(pluginType);
        if (pluginDirectory == null)
            return null;

        var configPath = Path.Combine(pluginDirectory, "config.json");
        if (!File.Exists(configPath))
            return null;

        var configContent = File.ReadAllText(configPath);
        return string.IsNullOrEmpty(configContent) ? null : configContent;
    }

    public static void SaveConfigContent(Type pluginType, string configContent)
    {
        var configPath = GetConfigPath(pluginType);
        File.WriteAllText(configPath, configContent);
    }

    public static void SaveConfig<T>(T config)
    {
        SaveConfigContent(typeof(T), JsonConvert.SerializeObject(config, Formatting.Indented));
    }

    public static string GetConfigPath(Type pluginType)
    {
        var pluginDirectory = GetPluginDirectory(pluginType);
        if (pluginDirectory == null)
            return null;

        var configPath = Path.Combine(pluginDirectory, "config.json");
        return configPath;
    }

    public static T? GetConfig<T>()
    {
        var configContent = GetConfigContent(typeof(T));
        return string.IsNullOrEmpty(configContent) ? default : JsonConvert.DeserializeObject<T>(configContent);
    }
}