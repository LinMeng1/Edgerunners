using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Moon.Core.Utilities
{
    public static class Http
    {
        public static async Task<string> Post(string url, object parameter, Dictionary<string, string>? Headers = null)
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, errors) => true
            };
            HttpClient httpClient = new(handler);
            HttpContent content = new StringContent(JsonConvert.SerializeObject(parameter));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            if (Headers != null)
                foreach (var header in Headers)
                {
                    httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
                }
            HttpResponseMessage response = await httpClient.PostAsync(url, content);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            return responseBody;
        }

        public static async Task<string> Get(string url, Dictionary<string, string>? Headers = null)
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, errors) => true
            };
            HttpClient httpClient = new(handler);
            if (Headers != null)
                foreach (var header in Headers)
                {
                    httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
                }
            HttpResponseMessage response = await httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            return responseBody;
        }
    }
}
