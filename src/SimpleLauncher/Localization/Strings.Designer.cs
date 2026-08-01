#nullable enable

namespace SimpleLauncher.Localization;

[global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
[global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
[global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
internal class Strings
{
    private static global::System.Resources.ResourceManager? resourceMan;
    private static global::System.Globalization.CultureInfo? resourceCulture;

    [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
    internal Strings()
    {
    }

    internal static global::System.Resources.ResourceManager ResourceManager
    {
        get
        {
            if (resourceMan is null)
            {
                resourceMan = new global::System.Resources.ResourceManager("SimpleLauncher.Localization.Strings", typeof(Strings).Assembly);
            }
            return resourceMan;
        }
    }

    internal static global::System.Globalization.CultureInfo Culture
    {
        get => resourceCulture ?? global::System.Globalization.CultureInfo.CurrentUICulture;
        set => resourceCulture = value;
    }

    internal static string AppTitle => ResourceManager.GetString(nameof(AppTitle), Culture) ?? nameof(AppTitle);
    internal static string Settings => ResourceManager.GetString(nameof(Settings), Culture) ?? nameof(Settings);
    internal static string Applications => ResourceManager.GetString(nameof(Applications), Culture) ?? nameof(Applications);
    internal static string SelectedApplication => ResourceManager.GetString(nameof(SelectedApplication), Culture) ?? nameof(SelectedApplication);
    internal static string Launcher => ResourceManager.GetString(nameof(Launcher), Culture) ?? nameof(Launcher);
    internal static string IconSize => ResourceManager.GetString(nameof(IconSize), Culture) ?? nameof(IconSize);
    internal static string ProtectSettings => ResourceManager.GetString(nameof(ProtectSettings), Culture) ?? nameof(ProtectSettings);
    internal static string HideTaskbar => ResourceManager.GetString(nameof(HideTaskbar), Culture) ?? nameof(HideTaskbar);
    internal static string ChangePin => ResourceManager.GetString(nameof(ChangePin), Culture) ?? nameof(ChangePin);
    internal static string CurrentPin => ResourceManager.GetString(nameof(CurrentPin), Culture) ?? nameof(CurrentPin);
    internal static string NewPin => ResourceManager.GetString(nameof(NewPin), Culture) ?? nameof(NewPin);
    internal static string ConfirmPin => ResourceManager.GetString(nameof(ConfirmPin), Culture) ?? nameof(ConfirmPin);
    internal static string Backup => ResourceManager.GetString(nameof(Backup), Culture) ?? nameof(Backup);
    internal static string Import => ResourceManager.GetString(nameof(Import), Culture) ?? nameof(Import);
    internal static string Export => ResourceManager.GetString(nameof(Export), Culture) ?? nameof(Export);
    internal static string Cancel => ResourceManager.GetString(nameof(Cancel), Culture) ?? nameof(Cancel);
    internal static string Save => ResourceManager.GetString(nameof(Save), Culture) ?? nameof(Save);
    internal static string Add => ResourceManager.GetString(nameof(Add), Culture) ?? nameof(Add);
    internal static string Exit => ResourceManager.GetString(nameof(Exit), Culture) ?? nameof(Exit);
    internal static string SearchPlaceholder => ResourceManager.GetString(nameof(SearchPlaceholder), Culture) ?? nameof(SearchPlaceholder);
    internal static string BackgroundImage => ResourceManager.GetString(nameof(BackgroundImage), Culture) ?? nameof(BackgroundImage);
    internal static string ChooseImage => ResourceManager.GetString(nameof(ChooseImage), Culture) ?? nameof(ChooseImage);
    internal static string RecoveryTitle => ResourceManager.GetString(nameof(RecoveryTitle), Culture) ?? nameof(RecoveryTitle);
    internal static string RecoveryReturn => ResourceManager.GetString(nameof(RecoveryReturn), Culture) ?? nameof(RecoveryReturn);
    internal static string RecoveryDesktop => ResourceManager.GetString(nameof(RecoveryDesktop), Culture) ?? nameof(RecoveryDesktop);
    internal static string RecoveryPinPrompt => ResourceManager.GetString(nameof(RecoveryPinPrompt), Culture) ?? nameof(RecoveryPinPrompt);
    internal static string RecoveryPinError => ResourceManager.GetString(nameof(RecoveryPinError), Culture) ?? nameof(RecoveryPinError);
    internal static string PinRejected => ResourceManager.GetString(nameof(PinRejected), Culture) ?? nameof(PinRejected);
    internal static string SettingsProtected => ResourceManager.GetString(nameof(SettingsProtected), Culture) ?? nameof(SettingsProtected);
    internal static string AddApplicationTitle => ResourceManager.GetString(nameof(AddApplicationTitle), Culture) ?? nameof(AddApplicationTitle);
    internal static string AddApplicationFilter => ResourceManager.GetString(nameof(AddApplicationFilter), Culture) ?? nameof(AddApplicationFilter);
    internal static string ChooseBackgroundTitle => ResourceManager.GetString(nameof(ChooseBackgroundTitle), Culture) ?? nameof(ChooseBackgroundTitle);
    internal static string ChooseBackgroundFilter => ResourceManager.GetString(nameof(ChooseBackgroundFilter), Culture) ?? nameof(ChooseBackgroundFilter);
    internal static string BackgroundImageUpdated => ResourceManager.GetString(nameof(BackgroundImageUpdated), Culture) ?? nameof(BackgroundImageUpdated);
    internal static string Apply => ResourceManager.GetString(nameof(Apply), Culture) ?? nameof(Apply);
    internal static string Remove => ResourceManager.GetString(nameof(Remove), Culture) ?? nameof(Remove);
    internal static string Language => ResourceManager.GetString(nameof(Language), Culture) ?? nameof(Language);
    internal static string DeepLKey => ResourceManager.GetString(nameof(DeepLKey), Culture) ?? nameof(DeepLKey);
    internal static string EnableDeepL => ResourceManager.GetString(nameof(EnableDeepL), Culture) ?? nameof(EnableDeepL);
    internal static string ChangePinButton => ResourceManager.GetString(nameof(ChangePinButton), Culture) ?? nameof(ChangePinButton);
    internal static string PinDialogTitle => ResourceManager.GetString(nameof(PinDialogTitle), Culture) ?? nameof(PinDialogTitle);
    internal static string PinPrompt => ResourceManager.GetString(nameof(PinPrompt), Culture) ?? nameof(PinPrompt);
    internal static string Ok => ResourceManager.GetString(nameof(Ok), Culture) ?? nameof(Ok);
}
