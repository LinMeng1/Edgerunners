using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace NightCity.Core.Services
{
    public class HttpService
    {
        private HttpClient httpClient;
        public HttpService()
        {
            httpClient = new HttpClient();
        }
        public void AddToken(string token)
        {
            httpClient.DefaultRequestHeaders.Remove("Authorization");
            httpClient.DefaultRequestHeaders.Add("Authorization", $"bearer {token}");
        }
        public async Task<string> Get(string url)
        {
            HttpResponseMessage response = await httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            return responseBody;
        }
        public async Task<string> Post(string url, object parameter)
        {
            HttpContent content = new StringContent(JsonConvert.SerializeObject(parameter));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = await httpClient.PostAsync(url, content);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            return responseBody;
        }
    }
}
