using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace QR_Code
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void QR_Button_Click(object sender, RoutedEventArgs e)
        {
            var input = UrlTextBox.Text?.Trim();
            if (string.IsNullOrEmpty(input))
            {
                MessageBox.Show("Please enter a valid URL.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Generate QR code logic will go here
            using var qrGenerator = new QRCoder.QRCodeGenerator();
            var qrCodeData = qrGenerator.CreateQrCode(input, QRCoder.QRCodeGenerator.ECCLevel.Q);
            var pngQR = new QRCoder.PngByteQRCode(qrCodeData);
            byte[] pngBytes = pngQR.GetGraphic(20);

            // Convert byte array to BitmapImage
            var bitmapImage = new BitmapImage();
            using (var stream = new System.IO.MemoryStream(pngBytes))
            {
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = stream;
                bitmapImage.EndInit();
            }
            QrCodeImage.Source = bitmapImage;
        }

        private void Clear_Button_Click(object sender, RoutedEventArgs e)
        {
            UrlTextBox.Clear();
            QrCodeImage.Source = null;
        }

        private void Save_Button_Click(object sender, RoutedEventArgs e)
        {
            if (!(QrCodeImage.Source != null && QrCodeImage.Source is BitmapSource bitmapSource))
            {
                MessageBox.Show("No QR code to save. Please generate a QR code first.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var saveFileDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "PNG Image (*.png)|*.png",
                DefaultExt = ".png",
                Title = "Save QR Code"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
                using (var stream = new System.IO.FileStream(saveFileDialog.FileName, System.IO.FileMode.Create))
                {
                    encoder.Save(stream);
                }
            }
        }   




    }
}