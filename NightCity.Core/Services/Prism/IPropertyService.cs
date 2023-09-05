namespace NightCity.Core.Services.Prism
{
    public interface IPropertyService
    {
        object GetProperty(string key);
        void SetProperty(string key, object value);
    }
}
