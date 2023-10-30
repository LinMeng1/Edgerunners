using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NightCity.Views
{
    /// <summary>
    /// ModuleManager.xaml 的交互逻辑
    /// </summary>
    public partial class ModuleManager : UserControl
    {
        public ModuleManager()
        {
            InitializeComponent();
        }

        [DllImport("User32.dll")]
        private static extern IntPtr SetFocus(IntPtr hWnd);
        private void Textbox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            var source = (HwndSource)PresentationSource.FromVisual(sender as TextBox);
            if (source != null)
            {
                SetFocus(source.Handle);
            }
            textBox.Focus();
        }
    }
}
