using System.Reflection;
using ITHock.XarfReportGenerator.Plugin;

namespace ITHock.XarfReportGenerator;

public class PluginSystem
{
    public class Plugin
    {
        public string Name => PluginInstance.Name;

        public IPlugin PluginInstance { get; }
        public IReportCollector? ReportCollector { get; }
        public IIpProcessor? IpProcessor { get; }

        public Plugin(IPlugin pluginInstance, IReportCollector? reportCollector = null,
            IIpProcessor? ipProcessor = null)
        {
            PluginInstance = pluginInstance;
            ReportCollector = reportCollector;
            IpProcessor = ipProcessor;
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
                var ipProcessorType = assembly.GetTypes().Where(t => t.GetInterfaces().Contains(typeof(IIpProcessor)));
                if (!reportCollectorType.Any() && !ipProcessorType.Any())
                    throw new NotImplementedException("Plugin does not implement IReportCollector or IIpProcessor");

                IReportCollector? reportCollectorInstance = null;
                if (reportCollectorType.Any())
                    reportCollectorInstance = (IReportCollector?)Activator.CreateInstance(reportCollectorType.First());

                IIpProcessor? ipProcessorInstance = null;
                if (ipProcessorType.Any())
                    ipProcessorInstance = (IIpProcessor?)Activator.CreateInstance(ipProcessorType.First());

                var plugin = new Plugin(pluginInstance, reportCollectorInstance, ipProcessorInstance);
                _loadedPlugins.Add(plugin);
                Console.WriteLine($"Loaded plugin {plugin.Name}");
            }
            catch (NotImplementedException e)
            {
                Console.WriteLine(e);
            }
            catch (Exception)
            {
                // ignored
                Console.WriteLine($"Failed to load plugin {pluginFile}");
#if DEBUG
                throw;
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

    public IEnumerable<IIpProcessor> GetIpProcessors()
    {
        return _loadedPlugins.Where(p => p.IpProcessor != null).Select(p => p.IpProcessor)!;
    }

    public IEnumerable<IReportCollector> GetReportCollectors()
    {
        return _loadedPlugins.Where(p => p.ReportCollector != null).Select(p => p.ReportCollector)!;
    }
}