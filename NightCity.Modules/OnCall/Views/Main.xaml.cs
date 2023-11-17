using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;

namespace OnCall.Views
{
    /// <summary>
    /// Main.xaml 的交互逻辑
    /// </summary>
    public partial class Main : UserControl
    {
        public Main()
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
