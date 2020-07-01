using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System.Text.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Threading.Tasks;
using WorkplaceAdministrator.Common.Clients;
using WorkplaceAdministrator.Common.Dtos;
using WorkplaceAdministrator.Common;
using Newtonsoft.Json;

namespace WorkplaceAdministrator.Blazor
{ 
    public class WorkplaceAuthStateProvider : AuthenticationStateProvider
    {
        private readonly HttpClient _client;
        private readonly ILocalStorageService _localStorage;

        public WorkplaceAuthStateProvider(ILocalStorageService localStorage, HttpClient client)
        {
            _client = client;
            _localStorage = localStorage;
        }

        public async override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            if (await _localStorage.ContainKeyAsync("authToken"))
            {

                var token = await _localStorage.GetItemAsync<string>("authToken");
                var claims = ParseClaimsFromJwt(token);
                var identity = new ClaimsIdentity(claims, "bearerToken");
                var user = new ClaimsPrincipal(identity);
                var state = new AuthenticationState(user);
                NotifyAuthenticationStateChanged(Task.FromResult(state));

                return state;
            }

            return new AuthenticationState(new ClaimsPrincipal());
        }

        public async Task<AuthenticationState> LoginAsync(LoginDto loginDto)
        {
            var httpResponse = await _client.PostAsJsonAsync("https://localhost:44335/api/auth/login", loginDto);
            var responseJson = await httpResponse.Content.ReadAsStringAsync();
            var responseDto = JsonConvert.DeserializeObject<AuthResponse>(responseJson);

            if (responseDto.IsSuccess)
            {
                await _localStorage.SetItemAsync("authToken", responseDto.Message);

                return await GetAuthenticationStateAsync();
            }

            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        public async Task LogoutAsync()
        {
            await _localStorage.RemoveItemAsync("authToken");
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(new ClaimsPrincipal())));
        }

        private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var claims = new List<Claim>();
            var payload = jwt.Split('.')[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);
            var keyValuePairs = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

            keyValuePairs.TryGetValue(ClaimTypes.Role, out object roles);

            if (roles != null)
            {
                if (roles.ToString().Trim().StartsWith("["))
                {
                    var parsedRoles = System.Text.Json.JsonSerializer.Deserialize<string[]>(roles.ToString());

                    foreach (var parsedRole in parsedRoles)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, parsedRole));
                    }
                }
                else
                {
                    claims.Add(new Claim(ClaimTypes.Role, roles.ToString()));
                }

                keyValuePairs.Remove(ClaimTypes.Role);
            }

            claims.AddRange(keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString())));

            return claims;
        }

        private byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            return Convert.FromBase64String(base64);
        }
    }
}
