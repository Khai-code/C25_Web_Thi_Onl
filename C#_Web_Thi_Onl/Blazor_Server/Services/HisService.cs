namespace Blazor_Server.Services
{
    public class HisService
    {
        private readonly HttpClient _httpClient;

        public HisService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        
    }
}
