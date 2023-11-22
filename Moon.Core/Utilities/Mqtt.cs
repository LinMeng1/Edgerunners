using Moon.Core.Models._Imaginary;
using MQTTnet;
using MQTTnet.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moon.Core.Utilities
{
    public static class Mqtt
    {
        public static async void Publish(string topic, _MqttMessage message)
        {
            IMqttClient mqttClient = new MqttFactory().CreateMqttClient();
            MqttClientOptions mqttClientOptions = new MqttClientOptionsBuilder()
               .WithClientId("Moon")
               .WithTcpServer("10.114.113.101", 1883)
               .WithKeepAlivePeriod(TimeSpan.FromSeconds(10))
               .WithCredentials("NightCity", "I Really Want to Stay At Your House")
               .WithCleanSession()
               .Build();
            await mqttClient.ConnectAsync(mqttClientOptions);
            var mqttMessage = new MqttApplicationMessageBuilder()
               .WithTopic($"NightCity/{topic}")
               .WithPayload(JsonConvert.SerializeObject(message))
               .WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce)
               .Build();
            if (mqttClient.IsConnected)
                await mqttClient.PublishAsync(mqttMessage);
            await mqttClient.DisconnectAsync();
            mqttClient.Dispose();
        }
    }
}
