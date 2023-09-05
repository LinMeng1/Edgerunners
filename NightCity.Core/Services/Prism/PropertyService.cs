using System.Collections.Concurrent;

namespace NightCity.Core.Services.Prism
{
    public class PropertyService : IPropertyService
    {
        private ConcurrentDictionary<string, object> properties = new ConcurrentDictionary<string, object>();
        public object GetProperty(string name)
        {
            properties.TryGetValue(name, out object value);
            return value;
        }
        public void SetProperty(string key, object value)
        {
            properties.AddOrUpdate(key, value, (xkey, xvalue) => value);            
        }
    }
}
