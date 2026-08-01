using System.Resources;
using System.Text.Json;
using SimpleLauncher.Infrastructure.Configuration;
using SimpleLauncher.Localization;
using SimpleLauncher.Logging;
using SimpleLauncher.Models;

namespace SimpleLauncher.Services;

/// <summary>
/// Provides localizable strings backed by English resources and optional DeepL translation caching.
/// </summary>
public sealed class LocalizationService(AppPaths paths, FileLogger logger)
{
    private static readonly JsonSerializerOptions SerializerOptions = new() { WriteIndented = true };
    private readonly Dictionary<string, string> cache = new(StringComparer.OrdinalIgnoreCase);

    public string GetString(string key, LauncherConfiguration configuration)
    {
        var language = string.IsNullOrWhiteSpace(configuration.Theme.Language) ? "en-US" : configuration.Theme.Language;
        if (string.Equals(language, "en-US", StringComparison.OrdinalIgnoreCase))
        {
            return GetEnglish(key);
        }

        if (cache.TryGetValue($"{language}:{key}", out var translated))
        {
            return translated;
        }

        if (!configuration.Theme.EnableDeepLTranslations || string.IsNullOrWhiteSpace(configuration.Theme.DeepLApiKey))
        {
            return GetEnglish(key);
        }

        try
        {
            var translatedText = TranslateWithDeepL(GetEnglish(key), language, configuration.Theme.DeepLApiKey);
            cache[$"{language}:{key}"] = translatedText;
            PersistCache();
            return translatedText;
        }
        catch (Exception ex)
        {
            logger.Error($"Translation failed for '{key}'.", ex);
            return GetEnglish(key);
        }
    }

    public void LoadCache()
    {
        if (!File.Exists(paths.TranslationCacheFilePath))
        {
            return;
        }

        try
        {
            var json = File.ReadAllText(paths.TranslationCacheFilePath);
            var entries = JsonSerializer.Deserialize<Dictionary<string, string>>(json, SerializerOptions);
            if (entries is not null)
            {
                foreach (var entry in entries)
                {
                    cache[entry.Key] = entry.Value;
                }
            }
        }
        catch (Exception ex)
        {
            logger.Error("Failed to load localization cache.", ex);
        }
    }

    private void PersistCache()
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(paths.TranslationCacheFilePath) ?? AppContext.BaseDirectory);
            File.WriteAllText(paths.TranslationCacheFilePath, JsonSerializer.Serialize(cache, SerializerOptions));
        }
        catch (Exception ex)
        {
            logger.Error("Failed to persist localization cache.", ex);
        }
    }

    private static string TranslateWithDeepL(string text, string targetLanguage, string apiKey)
    {
        using var client = new System.Net.Http.HttpClient();
        client.DefaultRequestHeaders.Add("Authorization", $"DeepL-Auth-Key {apiKey}");
        var payload = new System.Net.Http.StringContent($"text={Uri.EscapeDataString(text)}&target_lang={targetLanguage.ToUpperInvariant().Split('-')[0]}", System.Text.Encoding.UTF8, "application/x-www-form-urlencoded");
        var response = client.PostAsync("https://api-free.deepl.com/v2/translate", payload).GetAwaiter().GetResult();
        response.EnsureSuccessStatusCode();
        var body = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
        using var doc = JsonDocument.Parse(body);
        return doc.RootElement.GetProperty("translations")[0].GetProperty("text").GetString() ?? text;
    }

    private static string GetEnglish(string key) => key switch
    {
        "AppTitle" => Strings.AppTitle,
        "Settings" => Strings.Settings,
        "Applications" => Strings.Applications,
        "SelectedApplication" => Strings.SelectedApplication,
        "Launcher" => Strings.Launcher,
        "IconSize" => Strings.IconSize,
        "ProtectSettings" => Strings.ProtectSettings,
        "HideTaskbar" => Strings.HideTaskbar,
        "ChangePin" => Strings.ChangePin,
        "CurrentPin" => Strings.CurrentPin,
        "NewPin" => Strings.NewPin,
        "ConfirmPin" => Strings.ConfirmPin,
        "Backup" => Strings.Backup,
        "Import" => Strings.Import,
        "Export" => Strings.Export,
        "Cancel" => Strings.Cancel,
        "Save" => Strings.Save,
        "Add" => Strings.Add,
        "Exit" => Strings.Exit,
        "SearchPlaceholder" => Strings.SearchPlaceholder,
        "BackgroundImage" => Strings.BackgroundImage,
        "ChooseImage" => Strings.ChooseImage,
        "RecoveryTitle" => Strings.RecoveryTitle,
        "RecoveryReturn" => Strings.RecoveryReturn,
        "RecoveryDesktop" => Strings.RecoveryDesktop,
        "RecoveryPinPrompt" => Strings.RecoveryPinPrompt,
        "PinDialogTitle" => Strings.PinDialogTitle,
        "PinPrompt" => Strings.PinPrompt,
        "Ok" => Strings.Ok,
        "RecoveryPinError" => Strings.RecoveryPinError,
        "PinRejected" => Strings.PinRejected,
        "SettingsProtected" => Strings.SettingsProtected,
        "AddApplicationTitle" => Strings.AddApplicationTitle,
        "AddApplicationFilter" => Strings.AddApplicationFilter,
        "ChooseBackgroundTitle" => Strings.ChooseBackgroundTitle,
        "ChooseBackgroundFilter" => Strings.ChooseBackgroundFilter,
        "BackgroundImageUpdated" => Strings.BackgroundImageUpdated,
        "Apply" => Strings.Apply,
        "Remove" => Strings.Remove,
        "Language" => Strings.Language,
        "DeepLKey" => Strings.DeepLKey,
        "EnableDeepL" => Strings.EnableDeepL,
        "ChangePinButton" => Strings.ChangePinButton,
        _ => key
    };
}
