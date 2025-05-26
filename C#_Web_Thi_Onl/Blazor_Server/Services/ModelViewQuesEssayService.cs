using Data_Base.Models.S;
using static Blazor_Server.Services.Package_Test_ERP;
using System.Net.Http;
using Data_Base.Models.Q;

namespace Blazor_Server.Services
{
    public class ModelViewQuesEssayService
    {
        private readonly HttpClient _httpClient;

        public ModelViewQuesEssayService(HttpClient client)
        {
            _httpClient = client;
        }

        
    }
}
