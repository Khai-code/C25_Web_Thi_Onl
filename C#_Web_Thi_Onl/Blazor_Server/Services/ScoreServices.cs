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

            var student = students.FirstOrDefault(s => s.Student_Code == studentCode);
            if (student == null) return new();

            // Chỉ lấy các đầu điểm cố định, không cần check Summary_Id của PointType nữa
            var validPointTypeIds = pointTypes.Select(pt => pt.Id).ToList();

            // Lọc điểm theo kỳ
            var studentScores = scores
                .Where(s => s.Student_Id == student.Id && s.Summary_Id == currentSummaryId && validPointTypeIds.Contains(s.Point_Type_Id))
                .ToList();

            var studentScoreDetails = new List<StudentScoreDetail>();

            foreach (var score in studentScores)
            {
                var subject = subjects.FirstOrDefault(su => su.Id == score.Subject_Id);
                var pointType = pointTypes.FirstOrDefault(pt => pt.Id == score.Point_Type_Id);

                if (subject != null && pointType != null)
                {
                    studentScoreDetails.Add(new StudentScoreDetail
                    {
                        SubjectName = subject.Subject_Name,
                        PointType = pointType.Point_Type_Name,
                        Point = score.Point,
                        SummaryId = score.Summary_Id,
                        PointTypeId = score.Point_Type_Id
                    });
                }
            }

            return studentScoreDetails;
        }

        public async Task<Dictionary<int, List<StudentScoreDetail>>> GetAnnualStudentScoresAsync(string studentCode)
        {
            var scores = await _client.GetFromJsonAsync<List<Score>>("/api/Score/Get") ?? new();
            var students = await _client.GetFromJsonAsync<List<Student>>("/api/Student/Get") ?? new();
            var pointTypes = await _client.GetFromJsonAsync<List<Point_Type>>("/api/Point_Type/Get") ?? new();
            var subjects = await _client.GetFromJsonAsync<List<Subject>>("/api/Subject/Get") ?? new();

            var student = students.FirstOrDefault(s => s.Student_Code == studentCode);
            if (student == null) return new();

            // Các PointTypeId cố định
            var validPointTypeIds = pointTypes.Select(pt => pt.Id).ToList();

            // Nhóm điểm theo từng kỳ học (Summary_Id)
            var groupedBySummary = scores
                .Where(s => s.Student_Id == student.Id && validPointTypeIds.Contains(s.Point_Type_Id))
                .GroupBy(s => s.Summary_Id)
                .ToDictionary(g => g.Key, g => g.ToList());

            var result = new Dictionary<int, List<StudentScoreDetail>>();

            foreach (var kvp in groupedBySummary)
            {
                var summaryId = kvp.Key;
                var scoreList = kvp.Value;
                var details = new List<StudentScoreDetail>();

                foreach (var score in scoreList)
                {
                    var subject = subjects.FirstOrDefault(su => su.Id == score.Subject_Id);
                    var pointType = pointTypes.FirstOrDefault(pt => pt.Id == score.Point_Type_Id);

                    if (subject != null && pointType != null)
                    {
                        details.Add(new StudentScoreDetail
                        {
                            SubjectName = subject.Subject_Name,
                            PointType = pointType.Point_Type_Name,
                            Point = score.Point,
                            SummaryId = score.Summary_Id,
                            PointTypeId = score.Point_Type_Id
                        });
                    }
                }

                if (details.Any())
                    result[summaryId] = details;
            }

            return result;
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
            var today = DateTime.Now.Date;

            var currentSummary = summaries.FirstOrDefault(s =>
                today >= ConvertLong.ConvertLongToDateTime(s.Start_Time).Date &&
                today <= ConvertLong.ConvertLongToDateTime(s.End_Time).Date);
            
            return currentSummary?.Id ?? 0;
        }
    }

    public class StudentScoreDetail
    {
        public string SubjectName { get; set; }
        public int PointTypeId { get; set; }
        public string PointType { get; set; }
        public double Point { get; set; }
        public int SummaryId { get; set; }
    }


}


