using Microsoft.WindowsAPICodePack.Dialogs;
using PdfMark.Library;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace PdfMark.WpfApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string VERSION = "1.0.13";

        public MainWindow()
        {
            InitializeComponent();
            try { System.Diagnostics.Process.Start("update.exe").WaitForExit(); } catch { }

            Title += $" {VERSION}";

            // Properties.Settings.Default.Reset();
            txtFilePath.Text = Properties.Settings.Default.LastPdfDirectory;
            cboWatermark.ItemsSource = Properties.Settings.Default.WatermarkHistory.Split(';');
            cboWatermark.Text = Properties.Settings.Default.LastWatermark.EmptyAsNull() ?? "Work In Process";
        }

        private void BtnBrowse_Click(object sender, RoutedEventArgs e)
        {
            var d = new CommonOpenFileDialog() { Title = "Open Pdf Folder", IsFolderPicker = true };
            if (d.ShowDialog() != CommonFileDialogResult.Ok) { return; }

            txtFilePath.Text = d.FileName;
        }

        private void BtnStart_Click(object sender, RoutedEventArgs e) => Run();
        private void Run()
        {
            var path = txtFilePath.Text;
            var watermark = cboWatermark.Text;
            var destinationPath = Path.Combine(path, "watermarked");

            Properties.Settings.Default.LastPdfDirectory = txtFilePath.Text;
            Properties.Settings.Default.LastWatermark = watermark;
            Properties.Settings.Default.WatermarkHistory = Properties.Settings.Default.WatermarkHistory.Split(';').Concat(new[] { watermark }).Distinct().OrderBy(x => x).ConcatString(";");
            Properties.Settings.Default.Save();

            Task.Run(() =>
            {
                Dispatcher.Invoke(() =>
                {
                    txtMessage.Text = "Starting";
                    progressBar.Visibility = Visibility.Visible;
                    progressBar.Value = 0;
                });

                new PdfWatermarker().AddWatermark_Batch(path, watermark, destinationPath, (p, message) => Dispatcher.Invoke(() =>
                {
                    txtMessage.Text = message;
                    progressBar.Value = p * 100;
                }));

                Dispatcher.Invoke(() =>
                {
                    txtMessage.Text = "";
                    progressBar.Visibility = Visibility.Collapsed;
                    progressBar.Value = 100;
                });
            });
        }

        private void BtnOpenFolder_Click(object sender, RoutedEventArgs e)
        {
            var destinationPath = Path.Combine(txtFilePath.Text, "watermarked");
            System.Diagnostics.Process.Start(destinationPath);
        }
    }
}
