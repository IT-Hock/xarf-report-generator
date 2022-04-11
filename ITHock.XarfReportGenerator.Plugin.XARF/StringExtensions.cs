namespace ITHock.XarfReportGenerator.Plugin.XARF;

public class StringExtensions
{
    public static string GetCleanFileName(string fileName)
    {
        var invalidChars = Path.GetInvalidFileNameChars();
        return new string(fileName.Where(m => !invalidChars.Contains(m)).ToArray());
    }
}