using System.Runtime.InteropServices;
using System;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Interop;
using System.Windows;
using System.Windows.Input;
using NightCity.ViewModels;

namespace NightCity.Views
{
    /// <summary>
    /// Connection.xaml 的交互逻辑
    /// </summary>
    public partial class Connection : UserControl
    {
        public Connection()
        {
            InitializeComponent();
        }
        public void MessageScrollToEnd()
        {
            MessageScroll.ScrollToEnd();
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
