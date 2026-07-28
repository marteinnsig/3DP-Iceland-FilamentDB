using FilamentDbApp.Models;
using FilamentDbApp.Services;
using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace FilamentDbApp;

public partial class MainWindow
{
    private const string BuiltInDocumentLogoUri =
        "pack://application:,,,/Assets/3dp-iceland-labs-header-logo.png";

    private void SelectDocumentBrandingLogo_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFileDialog
        {
            Title = "Select Document Logo PNG",
            Filter = "PNG images (*.png)|*.png",
            CheckFileExists = true,
            Multiselect = false
        };
        if (dialog.ShowDialog(this) != true) return;

        try
        {
            var record = new DocumentBrandingService(_database).ImportCustomLogo(dialog.FileName);
            RefreshDocumentBrandingStatus();
            ShowTransientStatus(
                $"Custom document logo saved: {record.PixelWidth} × {record.PixelHeight}px, " +
                $"{record.ByteLength / 1024d:N1} KiB. The source file was unchanged.");
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                this,
                ex.Message,
                "Document Logo Not Accepted",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
            RefreshDocumentBrandingStatus();
        }
    }

    private void PreviewDocumentBrandingLogo_Click(object sender, RoutedEventArgs e)
    {
        RefreshDocumentBrandingStatus();
        var preview = new DocumentBrandingPreviewWindow(
            DocumentBrandingPreviewImage.Source,
            DocumentBrandingStatusText.Text)
        {
            Owner = this
        };
        preview.ShowDialog();
        ShowTransientStatus("Document branding preview closed; no data was changed.");
    }

    private void RestoreDefaultDocumentBranding_Click(object sender, RoutedEventArgs e)
    {
        var confirmation = ShowSafeDeleteConfirmation(
            "Restore Default Document Branding?",
            "Restore built-in document branding?\n\n" +
            "Any saved custom document logo will be removed from this database. Other Settings are unchanged.\n\n" +
            "Choose No, press Escape or close this warning to keep the current selection.");
        if (!confirmation) return;

        RestoreBuiltInDocumentBranding("Built-in document branding restored; other Settings were unchanged.");
    }

    private void RestoreBuiltInDocumentBranding(string status)
    {
        try
        {
            new DocumentBrandingService(_database).RestoreDefault();
            RefreshDocumentBrandingStatus();
            ShowTransientStatus(status);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                this,
                ex.Message,
                "Document Branding Could Not Be Restored",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            RefreshDocumentBrandingStatus();
        }
    }

    private void RefreshDocumentBrandingStatus()
    {
        if (DocumentBrandingPreviewImage is null || DocumentBrandingStatusText is null) return;

        try
        {
            var snapshot = new DocumentBrandingService(_database).ResolveCustomOrFallback();
            DocumentBrandingPreviewImage.Source = snapshot.Provenance == DocumentBrandingProvenance.Custom
                ? LoadDocumentLogo(snapshot.PngBytes)
                : LoadBuiltInDocumentLogo();
            DocumentBrandingStatusText.Text = snapshot.Provenance switch
            {
                DocumentBrandingProvenance.Custom =>
                    $"Custom PNG selected • {snapshot.PixelWidth} × {snapshot.PixelHeight}px • " +
                    $"{snapshot.PngBytes.LongLength / 1024d:N1} KiB • SHA-256 {snapshot.Sha256[..12]}…",
                DocumentBrandingProvenance.Fallback =>
                    "Fallback active: the saved custom PNG is missing or corrupt; built-in branding is shown.",
                _ => "Built-in document branding selected."
            };
        }
        catch
        {
            DocumentBrandingPreviewImage.Source = LoadBuiltInDocumentLogo();
            DocumentBrandingStatusText.Text =
                "Fallback active: document branding state could not be read; built-in branding is shown.";
        }
    }

    private static BitmapImage LoadDocumentLogo(byte[] bytes)
    {
        using var stream = new MemoryStream(bytes, writable: false);
        var image = new BitmapImage();
        image.BeginInit();
        image.CacheOption = BitmapCacheOption.OnLoad;
        image.StreamSource = stream;
        image.EndInit();
        image.Freeze();
        return image;
    }

    private static BitmapImage LoadBuiltInDocumentLogo()
    {
        var image = new BitmapImage();
        image.BeginInit();
        image.CacheOption = BitmapCacheOption.OnLoad;
        image.UriSource = new Uri(BuiltInDocumentLogoUri, UriKind.Absolute);
        image.EndInit();
        image.Freeze();
        return image;
    }
}
