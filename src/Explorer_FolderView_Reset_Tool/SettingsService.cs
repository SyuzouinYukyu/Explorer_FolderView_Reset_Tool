using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text;
using System.Text.Unicode;

namespace Explorer_FolderView_Reset_Tool;

public sealed class SettingsService
{
    private readonly string _settingsPath;
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true,
        Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
    };

    public SettingsService()
    {
        _settingsPath = Path.Combine(AppContext.BaseDirectory, "Explorer_FolderView_Reset_Tool_v1.0.0.settings.json");
    }

    public string SettingsPath => _settingsPath;

    public AppSettings Load()
    {
        try
        {
            if (!File.Exists(_settingsPath))
            {
                return new AppSettings();
            }

            var json = File.ReadAllText(_settingsPath);
            return JsonSerializer.Deserialize<AppSettings>(json, _jsonOptions) ?? new AppSettings();
        }
        catch
        {
            return new AppSettings();
        }
    }

    public void Save(AppSettings settings)
    {
        var json = JsonSerializer.Serialize(settings, _jsonOptions);
        File.WriteAllText(_settingsPath, json, new UTF8Encoding(encoderShouldEmitUTF8Identifier: true));
    }
}
