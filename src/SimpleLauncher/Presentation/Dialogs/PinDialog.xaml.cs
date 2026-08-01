using System.Windows;
using SimpleLauncher.Models;
using SimpleLauncher.Services;

namespace SimpleLauncher.Presentation.Dialogs;

public partial class PinDialog : Window
{
    private readonly LocalizationService? localizationService;
    private readonly LauncherConfiguration? configuration;

    public PinDialog(LocalizationService? localizationService = null, LauncherConfiguration? configuration = null)
    {
        this.localizationService = localizationService;
        this.configuration = configuration;
        InitializeComponent();
        ApplyLocalization();
        Loaded += (_, _) => PinBox.Focus();
    }

    public string Pin => PinBox.Password;

    private void OnOk(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
    }

    private void ApplyLocalization()
    {
        if (localizationService is null || configuration is null)
        {
            return;
        }

        Title = localizationService.GetString("PinDialogTitle", configuration);
        PromptText.Text = localizationService.GetString("PinPrompt", configuration);
        CancelButton.Content = localizationService.GetString("Cancel", configuration);
        OkButton.Content = localizationService.GetString("Ok", configuration);
    }
}
