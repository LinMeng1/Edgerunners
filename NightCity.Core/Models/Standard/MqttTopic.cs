using Prism.Mvvm;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace NightCity.Core.Models.Standard
{
    public class MqttTopic : BindableBase
    {
        public MqttTopic()
        {
            Messages.CollectionChanged += Messages_CollectionChanged;
        }
        private void Messages_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
                MqttMessage_ReadedChanged();
            MessagesChanged?.Invoke();
        }
        private void MqttMessage_ReadedChanged()
        {
            NoReadMessageCount = Messages.Where(it => !it.Readed).Count();
        }
        public string Topic { get; set; }
        public string Origin { get; set; }
        public string Category { get; set; }

        private int noReadMessageCount;
        public int NoReadMessageCount
        {
            get => noReadMessageCount;
            set
            {
                SetProperty(ref noReadMessageCount, value);
            }
        }

        public ObservableCollection<MqttMessage> messages = new ObservableCollection<MqttMessage>();
        public ObservableCollection<MqttMessage> Messages
        {
            get => messages;
            set
            {
                SetProperty(ref messages, value);
            }
        }
        public void ClearMessages()
        {
            Messages.Clear();
            MqttMessage_ReadedChanged();
            MessagesChanged?.Invoke();
        }
        public void ReadAllMessages()
        {
            foreach (var message in Messages)
            {
                message.Readed = true;
            }
            MqttMessage_ReadedChanged();
            MessagesChanged?.Invoke();
        }

        public delegate void MessagesChangedDelegate();
        public event MessagesChangedDelegate MessagesChanged;
    }
}
