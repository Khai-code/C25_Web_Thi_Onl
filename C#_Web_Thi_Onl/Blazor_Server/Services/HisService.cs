using Data_Base.Filters;
using Data_Base.GenericRepositories;
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

        public async Task<Data_Base.V_Model.V_Test> GetVTest(int testId, int PackageTypeId)
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

                if (vTesst != null)
                {
                    if (PackageTypeId != 1)
                    {
                        var lstSummary = await _httpClient.GetFromJsonAsync<List<Summary>>("https://localhost:7187/api/Summary/Get");
                        DateTime dateTime = DateTime.Now;
                        Summary summary = lstSummary.Where(x => ConvertLong.ConvertDateTimeToLong(dateTime) >= x.Start_Time && ConvertLong.ConvertDateTimeToLong(dateTime) <= x.End_Time).SingleOrDefault();
                        Data_Base.Models.S.Score score = new Score();
                        score.Student_Id = vTesst.Student_Id;
                        score.Subject_Id = vTesst.Subject_Id;
                        score.Point_Type_Id = vTesst.Point_Type_Id;
                        score.Point = vTesst.Score;
                        score.Summary_Id = summary.Id;

                        var checkScore = await _httpClient.PostAsJsonAsync("https://localhost:7187/api/Score/Post", score);

                        if (!checkScore.IsSuccessStatusCode)
                        {
                            return null;
                        }
                    }
                }
                return vTesst;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
