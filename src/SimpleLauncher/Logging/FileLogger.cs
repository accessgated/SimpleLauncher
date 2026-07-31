namespace SimpleLauncher.Logging;

/// <summary>
/// Small file logger used before a richer logging package is warranted.
/// </summary>
public sealed class FileLogger : IDisposable
{
    private readonly object syncRoot = new();
    private readonly StreamWriter writer;

    public FileLogger(string path)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(path) ?? AppContext.BaseDirectory);
        writer = new StreamWriter(new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.Read)) { AutoFlush = true };
    }

    public void Info(string message) => Write("INFO", message, null);

    public void Error(string message, Exception exception) => Write("ERROR", message, exception);

    public void Dispose() => writer.Dispose();

    private void Write(string level, string message, Exception? exception)
    {
        lock (syncRoot)
        {
            writer.WriteLine($"{DateTimeOffset.UtcNow:O} [{level}] {message}");
            if (exception is not null)
            {
                writer.WriteLine(exception);
            }
        }
    }
}
