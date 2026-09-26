using System.Text;

namespace FoodExpress.Infrastructure.Services;

public class FileManager
{
    private readonly string _dataFolderPath;

    public FileManager()
    {
        var projectPath =
            Directory.GetParent(
                AppContext.BaseDirectory)!
            .Parent!
            .Parent!
            .Parent!
            .Parent!
            .FullName;

        _dataFolderPath = Path.Combine(
            projectPath,
            "FoodExpress.Infrastructure",
            "Data");

        Directory.CreateDirectory(_dataFolderPath);
    }

    public async Task<List<string>> ReadAllLinesAsync(string fileName)
    {
        try
        {
            var filePath = GetFilePath(fileName);

            if (!File.Exists(filePath))
                return new List<string>();

            var lines = await File.ReadAllLinesAsync(
                filePath,
                Encoding.UTF8);

            return lines.ToList();
        }
        catch (IOException ex)
        {
            throw new IOException(
                $"Error reading file '{fileName}'.",
                ex);
        }
        catch (UnauthorizedAccessException ex)
        {
            throw new UnauthorizedAccessException(
                $"Access denied for file '{fileName}'.",
                ex);
        }
    }

    public async Task WriteAllLinesAsync(
        string fileName,
        IEnumerable<string> lines)
    {
        try
        {
            var filePath = GetFilePath(fileName);

            await File.WriteAllLinesAsync(
                filePath,
                lines,
                Encoding.UTF8);
        }
        catch (IOException ex)
        {
            throw new IOException(
                $"Error writing file '{fileName}'.",
                ex);
        }
        catch (UnauthorizedAccessException ex)
        {
            throw new UnauthorizedAccessException(
                $"Access denied for file '{fileName}'.",
                ex);
        }
    }

    public async Task AppendLineAsync(
        string fileName,
        string line)
    {
        try
        {
            var filePath = GetFilePath(fileName);

            await File.AppendAllTextAsync(
                filePath,
                line + Environment.NewLine,
                Encoding.UTF8);
        }
        catch (IOException ex)
        {
            throw new IOException(
                $"Error appending to file '{fileName}'.",
                ex);
        }
        catch (UnauthorizedAccessException ex)
        {
            throw new UnauthorizedAccessException(
                $"Access denied for file '{fileName}'.",
                ex);
        }
    }

    public bool FileExists(string fileName)
    {
        var filePath = GetFilePath(fileName);

        return File.Exists(filePath);
    }

    public string GetFilePath(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentException(
                "File name is required.",
                nameof(fileName));

        return Path.Combine(
            _dataFolderPath,
            fileName);
    }
}
