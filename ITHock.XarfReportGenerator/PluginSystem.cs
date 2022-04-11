using System.Reflection;
using ITHock.XarfReportGenerator.Plugin;
using SimpleLogger;

namespace ITHock.XarfReportGenerator;

public class PluginSystem
{
    public class Plugin
    {
        public string Name => PluginInstance.Name;

        public IPlugin PluginInstance { get; }
        public IReportCollector? ReportCollector { get; }
        public IReportProcessor? ReportProcessor { get; }

        public Plugin(IPlugin pluginInstance, IReportCollector? reportCollector = null,
            IReportProcessor? reportProcessor = null)
        {
            PluginInstance = pluginInstance;
            ReportCollector = reportCollector;
            ReportProcessor = reportProcessor;
        }
    }

    private readonly List<Plugin> _loadedPlugins = new();

    public PluginSystem(string pluginPath)
    {
        var pluginFiles = Directory.GetFiles(pluginPath, "*.Plugin.*.dll", SearchOption.AllDirectories);
        foreach (var pluginFile in pluginFiles)
        {
            try
            {
                var loadContext = new PluginLoadContext(pluginFile);
                var assembly =
                    loadContext.LoadFromAssemblyName(new AssemblyName(Path.GetFileNameWithoutExtension(pluginFile)));

                var pluginType = assembly.GetTypes().First(t => t.GetInterfaces().Contains(typeof(IPlugin)));

                var pluginInstance = (IPlugin?)Activator.CreateInstance(pluginType);
                if (pluginInstance == null) continue;

                pluginInstance.Initialize();

                var reportCollectorType =
                    assembly.GetTypes().Where(t => t.GetInterfaces().Contains(typeof(IReportCollector)));
                var reportProcessorType =
                    assembly.GetTypes().Where(t => t.GetInterfaces().Contains(typeof(IReportProcessor)));
                if (!reportCollectorType.Any() && !reportProcessorType.Any())
                    throw new NotImplementedException("does not implement IReportCollector or IReportProcessor");

                IReportCollector? reportCollectorInstance = null;
                if (reportCollectorType.Any())
                {
                    // Allows for parametersless constructors or constructor with IPlugin parameter
                    if(reportCollectorType.First().GetConstructor(new []{typeof(IPlugin)}) != null)
                        reportCollectorInstance = (IReportCollector?)Activator.CreateInstance(reportCollectorType.First(), pluginInstance);
                    else
                        reportCollectorInstance = (IReportCollector?)Activator.CreateInstance(reportCollectorType.First());
                }

                IReportProcessor? reportProcessorInstance = null;
                if (reportProcessorType.Any())
                {
                    // Allows for parametersless constructors or constructor with IPlugin parameter
                    if(reportProcessorType.First().GetConstructor(new []{typeof(IPlugin)}) != null)
                        reportProcessorInstance = (IReportProcessor?)Activator.CreateInstance(reportProcessorType.First(), pluginInstance);
                    else
                        reportProcessorInstance = (IReportProcessor?)Activator.CreateInstance(reportProcessorType.First());
                }

                var plugin = new Plugin(pluginInstance, reportCollectorInstance, reportProcessorInstance);
                _loadedPlugins.Add(plugin);
                Logger.Log(Logger.Level.Info, $"Loaded plugin {plugin.Name}");
            }
            catch (NotImplementedException e)
            {
                Logger.Log(Logger.Level.Warning, $"Plugin {Path.GetFileName(pluginFile)} {e.Message}");
            }
            catch (Exception e)
            {
                // ignored
                Logger.Log(Logger.Level.Warning, $"Failed to load plugin {pluginFile}");
#if DEBUG
                Logger.Log(e);
#endif
            }
        }
    }

    /// <summary>
    /// Returns the plugin with the given name. Null if not found.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public Plugin? GetPlugin(string name)
    {
        return _loadedPlugins.FirstOrDefault(p => p.Name == name);
    }

    public IEnumerable<Plugin> GetReportProcessors()
    {
        return _loadedPlugins.Where(p => p.ReportProcessor != null).Select(p => p);
    }

    public IEnumerable<Plugin> GetReportCollectors()
    {
        return _loadedPlugins.Where(p => p.ReportCollector != null).Select(p => p);
    }
}