using System.Windows;

namespace SimpleLauncher.Presentation.Dialogs;

public partial class PinDialog : Window
{
    public PinDialog()
    {
        InitializeComponent();
        Loaded += (_, _) => PinBox.Focus();
    }

    public string Pin => PinBox.Password;

    private void OnOk(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
    }
}
