using Prism.Mvvm;
using System;

namespace NightCity.Core.Models.Standard
{
    public class MqttMessage : BindableBase
    {
        public bool IsMastermind { get; set; }
        public string Address { get; set; }
        public string Sender { get; set; }
        public string Content { get; set; }
        public DateTime Time { get; set; }

        private bool readed;
        public bool Readed
        {
            get => readed;
            set
            {
                SetProperty(ref readed, value);
                ReadedChanged?.Invoke();
            }
        }

        public delegate void ReadedChangedDelegate();
        public event ReadedChangedDelegate ReadedChanged;
    }
}
