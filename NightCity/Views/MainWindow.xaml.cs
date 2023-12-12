using NightCity.Core.Events;
using Prism.Events;
using System;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using System.Windows;

namespace NightCity.Views
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow
    {
        //具名管道
        private static readonly NamedPipeServerStream PipeServer = new NamedPipeServerStream("NightCityPipe", PipeDirection.InOut, 1, PipeTransmissionMode.Message, PipeOptions.Asynchronous);
        //通知图标
        public System.Windows.Forms.NotifyIcon notifyIcon = new System.Windows.Forms.NotifyIcon()
        {
            Text = "NightCity",
            Icon = Properties.Resources.favicon1
        };
        private static string PipeCommand = string.Empty;
        public MainWindow(IEventAggregator eventAggregator)
        {
            InitializeComponent();
            notifyIcon.DoubleClick += NotifyIcon_DoubleClick;
            eventAggregator.GetEvent<MqttMessageReceivedEvent>().Subscribe((Message) =>
            {
                Connection.MessageScrollToEnd();
            }, ThreadOption.UIThread);
            ThreadPool.QueueUserWorkItem(delegate
            {
                void callback(IAsyncResult o)
                {
                    NamedPipeServerStream mServer = (NamedPipeServerStream)o.AsyncState;
                    mServer.EndWaitForConnection(o);
                    StreamReader mSR = new StreamReader(mServer);
                    StreamWriter mSW = new StreamWriter(mServer);
                    string mResult = null;
                    while (true)
                    {
                        mResult = mSR.ReadLine();
                        if (mResult == null)
                        {
                            break;
                        }
                        else
                        {
                            PipeCommand = mResult;
                        }
                    }
                    PipeServer.Disconnect();
                    PipeServer.BeginWaitForConnection(callback, PipeServer);
                    if (PipeCommand == "NightCity Exit")
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            DockedWidthOrHeight = 0;
                            Environment.Exit(0);
                        });
                    }
                }
                PipeServer.BeginWaitForConnection(callback, PipeServer);
            });
        }

        private void NotifyIcon_DoubleClick(object sender, EventArgs e)
        {
            DockedWidthOrHeight = 40;
            Visibility = Visibility.Visible;
            notifyIcon.Visible = false;
        }

        private void ColorZone_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DockedWidthOrHeight = 0;
            Visibility = Visibility.Collapsed;
            notifyIcon.Visible = true;
            notifyIcon.BalloonTipText = "NightCity has been minimized to the tray display. To restore, please double-click this icon.";
            notifyIcon.ShowBalloonTip(3000);
        }
    }
}
