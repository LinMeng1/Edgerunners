using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Server;
using Newtonsoft.Json;
using NightCity.Core.Models.Standard;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace NightCity.Core.Services
{
    public class MqttService : BindableBase
    {
        private MqttClientOptions mqttClientOptions;
        private IMqttClient mqttClient;
        private MqttTopicCollection topicCollection = new MqttTopicCollection();
        public MqttTopicCollection TopicCollection
        {
            get => topicCollection;
            private set
            {
                SetProperty(ref topicCollection, value);
            }
        }
        public MqttService(string clientId, string server, int port)
        {
            TopicCollection.Topics.Add(new MqttTopic()
            {
                Topic = clientId,
                Origin = "System",
            });
            mqttClient = new MqttFactory().CreateMqttClient();
            mqttClientOptions = new MqttClientOptionsBuilder()
               .WithClientId(clientId)
               .WithTcpServer(server, port)
               .WithKeepAlivePeriod(TimeSpan.FromSeconds(10))
               .WithCredentials("NightCity", "I Really Want to Stay At Your House")
               .WithCleanSession()
               .Build();
            mqttClient.ConnectedAsync += ConnectedAsyncTask;
            mqttClient.DisconnectedAsync += DisconnectedAsyncTask;
            mqttClient.ApplicationMessageReceivedAsync += ApplicationMessageReceivedAsyncTask;
            Connect();
        }
        public async Task AddTopic(string topic, string category = null)
        {
            try
            {               
                var distTopic = TopicCollection.Topics.FirstOrDefault(it => it.Topic == topic && it.Category == category);
                if (distTopic != null) return;
                while (!mqttClient.IsConnected)
                {
                    await Task.Delay(200);
                }
                Application.Current.Dispatcher.Invoke(() =>
                {
                    TopicCollection.Topics.Add(new MqttTopic()
                    {
                        Topic = topic,
                        Category = category
                    });
                });
                if (category == null)
                    await mqttClient.SubscribeAsync($"NightCity/{topic}");
                else
                    await mqttClient.SubscribeAsync($"NightCity/{category}/{topic}");
            }
            catch (Exception e)
            {
                Global.Log($"[MqttService]:[AddTopic]:{e.Message}", true);
            }
        }
        public async Task RemoveAllClusterTopic()
        {
            try
            {
                for (int i = TopicCollection.Topics.Count - 1; i >= 0; i--)
                {
                    var topic = TopicCollection.Topics[i];
                    if (topic.Origin != null) continue;
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        TopicCollection.Topics.Remove(topic);
                    });                  
                    await mqttClient.UnsubscribeAsync($"NightCity/{topic.Topic}");
                }
            }
            catch (Exception e)
            {
                Global.Log($"[MqttService]:[RemoveAllClusterTopic]:{e.Message}", true);
            }            
        }
        public void ClearTopic(string topic)
        {
            var distTopic = TopicCollection.Topics.FirstOrDefault(it => it.Topic == topic);
            if (distTopic != null)
                distTopic.ClearMessages();
        }
        public void ReadTopic(string topic)
        {
            var distTopic = TopicCollection.Topics.FirstOrDefault(it => it.Topic == topic);
            if (distTopic != null)
                distTopic.ReadAllMessages();
        }
        private void Connect()
        {
            mqttClient.ConnectAsync(mqttClientOptions);
        }
        public delegate void ConnectionChangedDelegate(bool isConnected);
        public event ConnectionChangedDelegate ConnectionChanged;
        private async Task ConnectedAsyncTask(MqttClientConnectedEventArgs e)
        {
            Global.Log($"[MqttService]:[ConnectedAsyncTask]:connected to server");
            ConnectionChanged?.Invoke(true);
            foreach (string topic in TopicCollection.Topics.Select(it => it.Topic))
            {
                await mqttClient.SubscribeAsync($"NightCity/{topic}");
            }
        }
        private Task DisconnectedAsyncTask(MqttClientDisconnectedEventArgs e)
        {
            Global.Log($"[MqttService]:[DisconnectedAsyncTask]:disconnected from server");
            ConnectionChanged?.Invoke(false);
            Global.Log($"[MqttService]:[DisconnectedAsyncTask]:attempting to reconnect");
            Thread.Sleep(2000);
            Connect();
            return Task.CompletedTask;
        }
        public delegate void ApplicationMessageReceivedDelegate(MqttMessage message);
        public event ApplicationMessageReceivedDelegate ApplicationMessageReceived;
        private Task ApplicationMessageReceivedAsyncTask(MqttApplicationMessageReceivedEventArgs e)
        {
            string topic = e.ApplicationMessage.Topic;
            var payloadSegment = e.ApplicationMessage.PayloadSegment;
            string message = Encoding.UTF8.GetString(payloadSegment.Array, payloadSegment.Offset, payloadSegment.Count);
            MqttMessage mqttMessage = JsonConvert.DeserializeObject<MqttMessage>(message);
            mqttMessage.Time = DateTime.Now;
            if (mqttMessage.Address == mqttClient.Options.ClientId)
            {
                mqttMessage.Address = string.Empty;
                mqttMessage.Readed = true;
            }
            Application.Current.Dispatcher.Invoke(() =>
            {
                var distTopic = TopicCollection.Topics.FirstOrDefault(it => $"NightCity/{it.Topic}" == topic || $"NightCity/{it.Category}/{it.Topic}" == topic);
                if (distTopic != null)
                    distTopic.Messages.Add(mqttMessage);
            });
            Global.Log($"[MqttService]:[ApplicationMessageReceived]:{message}");
            ApplicationMessageReceived(mqttMessage);
            return Task.CompletedTask;
        }
        public async Task Publish(bool isMasterMind, string topic, string sender, string content)
        {
            MqttMessage message = new MqttMessage()
            {
                IsMastermind = isMasterMind,
                Sender = sender,
                Address = mqttClient.Options.ClientId,
                Content = content
            };
            var mqttMessage = new MqttApplicationMessageBuilder()
                .WithTopic($"NightCity/{topic}")
                .WithPayload(JsonConvert.SerializeObject(message))
                .WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce)
                .Build();
            if (mqttClient.IsConnected)
                await mqttClient.PublishAsync(mqttMessage);
        }
    }
}
