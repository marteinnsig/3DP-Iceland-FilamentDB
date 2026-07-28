using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace FilamentDbApp;

public partial class DocumentBrandingPreviewWindow : Window
{
    public DocumentBrandingPreviewWindow(ImageSource? image, string status)
    {
        InitializeComponent();
        PreviewImage.Source = image;
        PreviewStatusText.Text = status;
        Loaded += (_, _) => ClosePreviewButton.Focus();
    }

    private void ClosePreviewButton_Click(object sender, RoutedEventArgs e)
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
