using Blazored.LocalStorage;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using WorkplaceAdministrator.Common;

namespace WorkplaceAdministrator.Blazor
{
    public interface IHttpService
    {
        Task<OperationResponse<T>> Get<T>(string uri);
        Task<OperationResponse<T>> Post<T>(string uri, object data);
    }

    public class HttpService : IHttpService
    {
        public readonly HttpClient _client;
        public readonly ILocalStorageService _localStorage;

        public HttpService(HttpClient client, ILocalStorageService localStorage)
        {
            _client = client;
            _localStorage = localStorage;
        }

        public async Task<OperationResponse<T>> Get<T>(string uri)
        {
            await SetAuthorizationHeadersAsync();

            var httpResponse = await _client.GetAsync(uri);
            var jsonResponse = await httpResponse.Content.ReadAsStringAsync();
            var objectResponse = JsonConvert.DeserializeObject<OperationResponse<T>>(jsonResponse);

            return objectResponse;
        }

        public async Task<OperationResponse<T>> Post<T>(string uri, object data)
        {
            await SetAuthorizationHeadersAsync();

            var jsonData = JsonConvert.SerializeObject(data);
            var stringContent = new StringContent(jsonData, Encoding.UTF8, "application/json");
            var httpResponse = await _client.PostAsync(uri, stringContent);
            var jsonResponse = await httpResponse.Content.ReadAsStringAsync();
            var objectResponse = JsonConvert.DeserializeObject<OperationResponse<T>>(jsonResponse);

            return objectResponse;
        }

        private async Task SetAuthorizationHeadersAsync()
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }
}
