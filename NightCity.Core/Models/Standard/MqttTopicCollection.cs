using Prism.Mvvm;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace NightCity.Core.Models.Standard
{
    public class MqttTopicCollection : BindableBase
    {
        public MqttTopicCollection()
        {
            Topics.CollectionChanged += Topics_CollectionChanged;
        }
        private void Topics_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                MqttTopicChanged();
                ObservableCollection<MqttTopic> topicCollection = sender as ObservableCollection<MqttTopic>;
                MqttTopic topic = topicCollection.LastOrDefault();
                topic.MessagesChanged += MqttTopicChanged;
            }
        }
        private void MqttTopicChanged()
        {
            int count = 0;
            foreach (var topic in Topics)
            {
                count += topic.NoReadMessageCount;
            }
            NoReadMessageCount = count;
        }

        private int noReadMessageCount;
        public int NoReadMessageCount
        {
            get => noReadMessageCount;
            set
            {
                SetProperty(ref noReadMessageCount, value);
                NoReadMessageCountChanged?.Invoke(value);
            }
        }

        private ObservableCollection<MqttTopic> topics = new ObservableCollection<MqttTopic>();
        public ObservableCollection<MqttTopic> Topics
        {
            get => topics;
            set
            {
                SetProperty(ref topics, value);
            }
        }

        public delegate void NoReadMessageCountChangedDelegate(int noReadMessageCount);
        public event NoReadMessageCountChangedDelegate NoReadMessageCountChanged;

    }
}
