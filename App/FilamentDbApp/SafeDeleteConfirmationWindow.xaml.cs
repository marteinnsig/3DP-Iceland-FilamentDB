using System.Windows;
using System.Windows.Input;

namespace FilamentDbApp;

public partial class SafeDeleteConfirmationWindow : Window
{
    public SafeDeleteConfirmationWindow(string caption, string message)
    {
        InitializeComponent();
        Title = caption;
        ConfirmationMessage.Text = message;
        Loaded += (_, _) => CancelDeleteButton.Focus();
    }

    private void ConfirmDeleteButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
    }

    private void CancelDeleteButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }

    private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Escape) return;
        e.Handled = true;
        DialogResult = false;
    }
}
