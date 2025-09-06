using System.Reflection;

namespace WindowsPathCheckerAva.Services;

public static class EmbeddedResourceExtractor
{
    public static async Task<string> ExtractEmbeddedExecutableAsync(string resourceName, string fileName)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var tempFilePath = Path.Combine(Path.GetTempPath(), fileName);

        using var stream = assembly.GetManifestResourceStream(resourceName);
        if (stream == null)
        {
            throw new FileNotFoundException($"Embedded resource '{resourceName}' not found.");
        }

        using var fileStream = File.Create(tempFilePath);
        await stream.CopyToAsync(fileStream);

        // Make the file executable (on Windows this is usually not needed, but good practice)
        if (OperatingSystem.IsWindows())
        {
            File.SetAttributes(tempFilePath, FileAttributes.Normal);
        }

        return tempFilePath;
    }

    public static void CleanupTempFile(string filePath)
    {
        try
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
        catch
        {
            // Ignore cleanup errors
        }
    }
}
