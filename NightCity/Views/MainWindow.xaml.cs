using Itp.WpfAppBar;
using NightCity.Core.Events;
using Prism.Events;
using System;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using System.Timers;
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
        private static string PipeCommand = string.Empty;
        public MainWindow(IEventAggregator eventAggregator)
        {
            InitializeComponent();
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
    }
}
