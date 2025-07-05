using Data_Base.Filters;
using Data_Base.Models.S;
using Data_Base.V_Model;

namespace Blazor_Server.Services
{
    public class HisService
    {
        private readonly HttpClient _httpClient;

        public HisService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Data_Base.V_Model.V_Test> GetVTest(int testId)
        {
            V_Test vTesst = new V_Test(); 
            try
            {
                if (testId <= 0)
                {
                    return null;
                }

                var filter = new CommonFilterRequest
                {
                    Filters = new Dictionary<string, string>
                    {
                             { "Test_Id", testId.ToString()}
                    }
                };

                var reques = await _httpClient.PostAsJsonAsync("https://localhost:7187/api/Exam_Room_Student/common/get", filter);

                vTesst = (await reques.Content.ReadFromJsonAsync<List<V_Test>>()).SingleOrDefault();

                return vTesst;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
