using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Windows.Media;

namespace NightCity.Views
{
    /// <summary>
    /// MessageBox.xaml 的交互逻辑
    /// </summary>
    public partial class MessageBox
    {
        public MessageBox(string content, string title, MessageBoxButton messageBoxButton = MessageBoxButton.OK, MessageBoxImage messageBoxImage = MessageBoxImage.Information)
        {
            InitializeComponent();
            ShowInTaskbar = false;
            Bitmap favicon = Properties.Resources.favicon;
            using (var memory = new MemoryStream())
            {
                favicon.Save(memory, ImageFormat.Png);
                memory.Position = 0;
                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();
                Favicon.Source = bitmapImage;
            }          
            Content.Text = content;
            Title.Text = title;
            switch (messageBoxButton)
            {
                case MessageBoxButton.OK:
                    YesButton.Visibility = Visibility.Visible;
                    break;
                case MessageBoxButton.YesNo:
                    YesButton.Visibility = Visibility.Visible;
                    NoButton.Visibility = Visibility.Visible;
                    break;
                case MessageBoxButton.YesNoCancel:
                    YesButton.Visibility = Visibility.Visible;
                    NoButton.Visibility = Visibility.Visible;
                    CancelButton.Visibility = Visibility.Visible;
                    break;
                case MessageBoxButton.OKCancel:
                    YesButton.Visibility = Visibility.Visible;
                    CancelButton.Visibility = Visibility.Visible;
                    break;
                default:
                    break;
            }
            switch (messageBoxImage)
            {
                case MessageBoxImage.Information:
                    Icon.Kind = MaterialDesignThemes.Wpf.PackIconKind.Information;
                    Icon.Foreground = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#1E88E5"));
                    break;
                case MessageBoxImage.Warning:
                    Icon.Kind = MaterialDesignThemes.Wpf.PackIconKind.Alert;
                    Icon.Foreground = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#FBC02D"));
                    break;
                case MessageBoxImage.Error:
                    Icon.Kind = MaterialDesignThemes.Wpf.PackIconKind.CloseCircle;
                    Icon.Foreground = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#D32F2F"));
                    break;
                case MessageBoxImage.Question:
                    Icon.Kind = MaterialDesignThemes.Wpf.PackIconKind.HelpCircle;
                    Icon.Foreground = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#00897B"));
                    break;
                default:
                    break;
            }
        }
        private void Window_Move(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DragMove();
        }
        private void YesButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
        private void NoButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = null;
        }
    }
}
