namespace StorageService.Services;

public class FileLogWriter : ILogWriter
{
    private readonly string _filePath;

    public FileLogWriter(IConfiguration configuration)
    {
        _filePath = configuration["FilePath"] ?? throw new ArgumentException("FilePath is not defined");

        if (!File.Exists(_filePath))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_filePath));
            using (var sw = File.CreateText(_filePath)) { }
        }
    }

    public void Write(string log)
    {
        using var w = File.AppendText(_filePath);
        w.WriteLine(log);
    }
}