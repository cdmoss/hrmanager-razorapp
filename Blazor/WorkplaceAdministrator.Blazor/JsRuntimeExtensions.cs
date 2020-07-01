using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WorkplaceAdministrator.Blazor
{
    public static class IJsRuntimeExtensions
    {
        public static ValueTask SetinLocalStorage(this IJSRuntime js, string key, string content)
        {
            return js.InvokeVoidAsync("localStorage.setItem", key, content);
        }

        public static ValueTask<string> GetFromLocalStorage(this IJSRuntime js, string key)
        {
            return js.InvokeAsync<string>("localStorage.getItem", key);
        }

        public static ValueTask RemoveFromLocalStorage(this IJSRuntime js, string key)
        {
            return js.InvokeVoidAsync("localStorage.removeItem", key);
        }
    }
}
