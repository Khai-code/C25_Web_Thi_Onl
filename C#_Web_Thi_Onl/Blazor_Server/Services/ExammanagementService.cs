using Data_Base.Models.R;
using Data_Base.Models.S;
using Microsoft.AspNetCore.Components.Server;
using System.Net.Http;
using System.Net.WebSockets;

namespace Blazor_Server.Services
{
    public class ExammanagementService
    {
        private readonly HttpClient _httpClient;
        public ExammanagementService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<List<Room>> GetallRoom()
        {
            var Listroom = await _httpClient.GetFromJsonAsync<List<Room>>("/api/Room/Get");
            return Listroom ?? new List<Room>();
        }

    }
}
