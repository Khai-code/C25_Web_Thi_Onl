using Data_Base.Models.C;
using Data_Base.Models.E;
using Data_Base.Models.P;
using Data_Base.Models.R;
using Data_Base.Models.S;
using Data_Base.Models.T;
using Data_Base.Models.U;
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
        public async Task<List<ListExam>> Listexam(int id)
        {
            var getExams = await _httpClient.GetFromJsonAsync<List<Exam>>("/api/Exam/Get") ?? new();
            var allExamRooms = await _httpClient.GetFromJsonAsync<List<Exam_Room>>("/api/Exam_Room/Get") ?? new();
            var allExamTeachers = await _httpClient.GetFromJsonAsync<List<Exam_Room_Teacher>>("/api/Exam_Room_Teacher/Get") ?? new();
            var allTeachers = await _httpClient.GetFromJsonAsync<List<Teacher>>("/api/Teacher/Get") ?? new();
            var allUsers = await _httpClient.GetFromJsonAsync<List<User>>("/api/User/Get") ?? new();
            var allPackages = await _httpClient.GetFromJsonAsync<List<Package>>("/api/Package/Get") ?? new();
            var allClasses = await _httpClient.GetFromJsonAsync<List<Class>>("/api/Class/Get") ?? new();
            var filteredExamRooms = allExamRooms.Where(x => x.Room_Id == id).ToList();

            var result = (from room in filteredExamRooms
                          let exam = getExams.FirstOrDefault(e => e.Id == room.Exam_Id)
                          let examTeachers = allExamTeachers.Where(t => t.Exam_Room == room.Id).ToList()
                          let teachers = allTeachers.Where(t => examTeachers.Any(et => et.Teacher_Id == t.Id)).ToList()
                          let teacherNames = teachers
                              .Select(t => allUsers.FirstOrDefault(u => u.Id == t.User_Id)?.Full_Name ?? "Không có giáo viên")
                              .ToList()
                          let teacherName1 = teacherNames.ElementAtOrDefault(0) ?? "Không có giáo viên"
                          let teacherName2 = teacherNames.ElementAtOrDefault(1) ?? "Không có giáo viên"
                          let nameClass = teachers
                              .Select(t => allClasses.FirstOrDefault(c => c.Teacher_Id == t.Id)?.Class_Name ?? "Không có lớp")
                              .FirstOrDefault() ?? "Không có lớp"
                          let package = allClasses.Select(t => allPackages.FirstOrDefault(p => p.Class_Id == t.Id)).FirstOrDefault()
                          select new ListExam
                          {
                              Id = package?.Id?? 0 ,
                              Name = exam?.Exam_Name ?? "Không có bài thi",
                              NameTeacher1 = teacherName1,
                              NameTeacher2 = teacherName2,
                              nameclass = nameClass,
                              StartTime = FormatTime(room.Start_Time),
                              EndTime = FormatTime(room.End_Time),
                              Status = package?.Status??0
                          }).ToList();

            return result;
        }
        public async Task<List<ListExam>> GetlistStudent(int id)
        {
            var allUsers = await _httpClient.GetFromJsonAsync<List<User>>("/api/User/Get") ?? new();
            var allstudent = await _httpClient.GetFromJsonAsync<List<Student>>("/api/Student/Get") ?? new();
            var allexamromStudent = await _httpClient.GetFromJsonAsync<List<Exam_Room_Student>>("/api/Exam_Room_Student/Get") ?? new();
            var allstudentClass = await _httpClient.GetFromJsonAsync<List<Student_Class>>("/api/Student_Class/Get") ?? new();
            var allClasses = await _httpClient.GetFromJsonAsync<List<Class>>("/api/Class/Get") ?? new();
            var allPackages = await _httpClient.GetFromJsonAsync<List<Package>>("/api/Package/Get") ?? new();
            var filteredPackage = allPackages.Where(x => x.Id == id).ToList();
            var result = (from package in filteredPackage
                          let classes = allClasses.FirstOrDefault(x => x.Id == package.Class_Id)
                          let stduent_class = allstudentClass.FirstOrDefault(x => x.Class_Id == classes.Id)
                          let student = allstudent.FirstOrDefault(x => x.Id == stduent_class.Student_Id)
                          let examromStd = allexamromStudent?.FirstOrDefault(x => x.Student_Id == student.Id)
                          let user = allUsers.FirstOrDefault(x => x.Id == student.User_Id)
                          select new ListExam
                          {
                              codeStudent= student.Student_Code,
                              Namestudent = user.Full_Name,
                              checktime = examromStd?.Check_Time != null ? FormatTime(examromStd.Check_Time):"00:00:00",
                              ExamStatus = examromStd != null ? "Đã vào thi" : "Chưa vào thi"
                          }).ToList();
            return result;
        }
        private string FormatTime(long timeValue)
        {
            string timeStr = timeValue.ToString("D4");
            return $"{timeStr.Substring(0, 2)}:{timeStr.Substring(2, 2)}"; 
        }


        public class ListExam
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Namestudent { get; set; }
            public string codeStudent { get; set; }
            public string NameTeacher1 { get; set; }
            public string NameTeacher2 { get; set; }
            public string nameclass { get; set; }
            public string StartTime { get; set; }
            public string checktime { get; set; }
            public string EndTime { get; set; }
            public int Status { get; set; }
            public string ExamStatus { get; set; }
        }
    }
}
