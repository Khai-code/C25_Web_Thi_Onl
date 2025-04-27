using Data_Base.GenericRepositories;
using Data_Base.Models.P;
using Data_Base.Models.S;
using Data_Base.Models.U;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.IdentityModel.Tokens.Jwt;

namespace Blazor_Server.Services
{
    public class ScoreServices
    {
        private readonly HttpClient _client;
        private readonly ProtectedSessionStorage _sessionStorage;

        public ScoreServices(HttpClient client, ProtectedSessionStorage sessionStorage)
        {
            _client = client;
            _sessionStorage = sessionStorage;
        }

        public async Task<List<StudentScoreDetail>> GetStudentScoresAsync(string studentCode, int currentSummaryId)
        {
            var scores = await _client.GetFromJsonAsync<List<Score>>("/api/Score/Get") ?? new();
            var students = await _client.GetFromJsonAsync<List<Student>>("/api/Student/Get") ?? new();
            var pointTypes = await _client.GetFromJsonAsync<List<Point_Type>>("/api/Point_Type/Get") ?? new();
            var subjects = await _client.GetFromJsonAsync<List<Subject>>("/api/Subject/Get") ?? new();
            var pointTypeSubjects = await _client.GetFromJsonAsync<List<Point_Type_Subject>>("/api/Point_Type_Subject/Get") ?? new();

            // 🔥 Lọc ra các PointType đúng kỳ học hiện tại
            var pointTypesForCurrentSummary = pointTypes
                .Where(pt => pt.Summary_Id == currentSummaryId)
                .ToList();

            var validPointTypeIds = pointTypesForCurrentSummary.Select(pt => pt.Id).ToList();

            var student = students.FirstOrDefault(s => s.Student_Code == studentCode);
            if (student == null)
            {
                return new List<StudentScoreDetail>();
            }

            // 🔥 Lọc Score: Student_Id đúng + PointType đúng kỳ
            var studentScores = scores
                .Where(s => s.Student_Id == student.Id && validPointTypeIds.Contains(s.Point_Type_Id))
                .ToList();

            // 🔥 Map dữ liệu điểm
            var studentScoreDetails = (from score in studentScores
                                       join subject in subjects on score.Subject_Id equals subject.Id
                                       join pointType in pointTypesForCurrentSummary on score.Point_Type_Id equals pointType.Id
                                       select new StudentScoreDetail
                                       {
                                           SubjectName = subject.Subject_Name,
                                           PointType = pointType.Point_Type_Name,
                                           Point = score.Point
                                       }).ToList();

            return studentScoreDetails;
        }



        // 🔥 Hàm lấy tất cả học sinh
        public async Task<List<Student>> GetAllStudentsAsync()
        {
            var students = await _client.GetFromJsonAsync<List<Student>>("/api/Student/Get");
            return students ?? new List<Student>();
        }

        // Hàm lấy ID của kỳ học hiện tại sử dụng hàm ConvertLong để so sánh
        public async Task<int> GetCurrentSummaryId()
        {
            var summaries = await _client.GetFromJsonAsync<List<Summary>>("/api/Summary/Get") ?? new();
            var currentTime = DateTime.Now;

            // Tìm kỳ học mà thời gian hiện tại nằm trong phạm vi
            var currentSummary = summaries.FirstOrDefault(s => currentTime >= ConvertLong.ConvertLongToDateTime(s.Start_Time) && currentTime <= ConvertLong.ConvertLongToDateTime(s.End_Time));
            return currentSummary?.Id ?? 0;
        }
    }

    public class StudentScoreDetail
    {
        public string SubjectName { get; set; }
        public string PointType { get; set; }
        public double Point { get; set; }
    }


}


