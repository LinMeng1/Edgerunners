using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace TestEngAuthorization.Views
{
    /// <summary>
    /// Main.xaml 的交互逻辑
    /// </summary>
    public partial class Main : UserControl
    {      
        public Main()
        {
            InitializeComponent();
            Bitmap avatar = Properties.Resources.Avatar;
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
            Focus();
        }
    }
}
