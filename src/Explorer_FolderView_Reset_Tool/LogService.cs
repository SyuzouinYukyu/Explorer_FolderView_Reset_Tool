using System.Text;

namespace Explorer_FolderView_Reset_Tool;

public enum LogLevel
{
    Info,
    Success,
    Warning,
    Error,
    Verbose
}

public sealed class LogService
{
    private readonly object _gate = new();
    private readonly StringBuilder _buffer = new();

    public event Action<string, LogLevel>? LineAdded;

    public string Text
    {
        get
        {
            lock (_gate)
            {
                return _buffer.ToString();
            }
        }
    }

    public void Clear()
    {
        lock (_gate)
        {
            _buffer.Clear();
        }
    }

    public void Info(string message) => Write(LogLevel.Info, message);

    public void Success(string message) => Write(LogLevel.Success, message);

    public void Warning(string message) => Write(LogLevel.Warning, message);

    public void Error(string message) => Write(LogLevel.Error, message);

    public void Verbose(string message) => Write(LogLevel.Verbose, message);

    public void Exception(Exception ex, string context)
    {
        Error($"{context}: {ex.GetType().Name}: {ex.Message}");
    }

    public void SaveToFile(string path)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(path) ?? AppContext.BaseDirectory);
        File.WriteAllText(path, Text, new UTF8Encoding(encoderShouldEmitUTF8Identifier: true));
    }

    private void Write(LogLevel level, string message)
    {
        var prefix = level switch
        {
            LogLevel.Success => "成功",
            LogLevel.Warning => "警告",
            LogLevel.Error => "エラー",
            LogLevel.Verbose => "詳細",
            _ => "情報"
        };
        var line = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{prefix}] {message}";

        lock (_gate)
        {
            _buffer.AppendLine(line);
        }

        LineAdded?.Invoke(line, level);
    }
}
