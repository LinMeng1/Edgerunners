using NightCity.Core.Events;
using Prism.Events;
using System.Windows;

namespace NightCity.Views
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow(IEventAggregator eventAggregator)
        {
            InitializeComponent();
            eventAggregator.GetEvent<MqttMessageReceivedEvent>().Subscribe((Message) =>
            {
                Connection.MessageScrollToEnd();
            }, ThreadOption.UIThread);
        }
    }
}
