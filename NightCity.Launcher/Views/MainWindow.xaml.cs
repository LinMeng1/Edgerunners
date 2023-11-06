using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace NightCity.Launcher.Views
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Bitmap favicon = Properties.Resources.favicon;
            Bitmap avatar = Properties.Resources.Avatar;
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
            using (var memory = new MemoryStream())
            {
                avatar.Save(memory, ImageFormat.Png);
                memory.Position = 0;
                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();
                Avatar.Source = bitmapImage;
            }
        }
        private void Close_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                Environment.Exit(0);
            }
        }

        private void Window_Move(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
