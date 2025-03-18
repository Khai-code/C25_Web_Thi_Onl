using Microsoft.JSInterop;

namespace Blazor_Server.Services
{
    public class Notification
    {
        private IJSRuntime _jsrutime;
        public Notification(IJSRuntime jSRuntime)
        {
            _jsrutime = jSRuntime;
        }
        public async Task ShowToast(string message,string type = "success")
        {
            await _jsrutime.InvokeVoidAsync("showToast", message, type);
        }
        public async Task ShowSweetAlert(string message,string type= "success")
        {
            await _jsrutime.InvokeVoidAsync("showSweetAlert", message, type);
        }
    }
}
