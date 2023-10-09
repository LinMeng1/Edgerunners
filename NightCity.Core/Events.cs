using NightCity.Core.Models;
using NightCity.Core.Models.Standard;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NightCity.Core.Events
{
    public class MqttConnectedEvent : PubSubEvent<bool> { }
    public class MqttMessageReceivedEvent : PubSubEvent<MqttMessage> { }
    public class MqttNoReadMessageCountChangedEvent : PubSubEvent<int> { }
    public class MqttMessageSendingEvent : PubSubEvent<Tuple<List<string>, MqttMessage>> { }
    public class BannerMessagesChangedEvent : PubSubEvent<Tuple<BannerMessage, int>> { }
    public class BannerMessageRemovingEvent : PubSubEvent<BannerMessage> { }
    public class BannerMessageTryLinkingEvent : PubSubEvent<Tuple<string, string>> { }
    public class BannerMessageSyncingEvent : PubSubEvent { }
    public class BannerMessageLinkingEvent : PubSubEvent<string> { }
    public class ModulesChangedEvent : PubSubEvent<ObservableCollection<ModuleInfo>> { }
    public class TemplateClosingEvent : PubSubEvent<string> { }
    public class TemplateReOpeningEvent : PubSubEvent<string> { }
    public class TemplateShowingEvent : PubSubEvent<string> { }
    public class AuthorizationInfoChangedEvent : PubSubEvent<Tuple<string, string>> { }
    public class IsConnectionFixChangedEvent : PubSubEvent<bool> { }
    public class ClustersSyncedEvent : PubSubEvent<List<Connection_GetClusters_Result>> { }
    public class ErrorMessageShowingEvent : PubSubEvent<Tuple<string, string>> { }
}
