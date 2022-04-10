namespace ITHock.XarfReportGenerator.Plugin;

public interface IPlugin
{
    public string Name { get; }
    
    public string Author { get; }
    
    void Initialize();
}