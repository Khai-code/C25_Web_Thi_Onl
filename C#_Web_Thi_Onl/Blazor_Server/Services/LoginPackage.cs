using Data_Base.GenericRepositories;
using Data_Base.Models.C;
using Data_Base.Models.E;
using Data_Base.Models.P;
using Data_Base.Models.S;
using Microsoft.AspNetCore.Mvc;
using static Blazor_Server.Services.Package_Test_ERP;

namespace Blazor_Server.Services
{
    public class LoginPackge
    {
        private readonly HttpClient _httpClient;
        public LoginPackge(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<bool> CheckPackage(string Studentcode, int packagecode)
        {
            var lstStudent = await _httpClient.GetFromJsonAsync<List<Student>>("https://localhost:7187/api/Subject/Get");

            if (lstStudent == null || lstStudent.Count == 0)
            {
                Console.WriteLine("Không tìm thấy sinh viên.");
                return false;
            }

            var student = lstStudent.FirstOrDefault(o => o.Student_Code == Studentcode);
            if (student == null)
            {
                Console.WriteLine("Không tìm thấy sinh viên.");
                return false;
            }

            int studentId = student.Id;
            var lstStudentClass = await _httpClient.GetFromJsonAsync<List<Student_Class>>("https://localhost:7187/api/Student_Class/Get");

            if (lstStudentClass == null || lstStudentClass.Count == 0)
            {
                Console.WriteLine("Không tìm thấy lớp của sinh viên.");
                return false;
            }

            var StudentClass = lstStudentClass.FirstOrDefault(o => o.Student_Id == studentId);
            if (StudentClass == null)
            {
                Console.WriteLine("Không tìm thấy lớp của sinh viên.");
                return false;
            }

            int ClassId = StudentClass.Class_Id;

            var Packages = await _httpClient.GetFromJsonAsync<List<Package>>("https://localhost:7187/api/Student_Class/Get");
            if (Packages == null || Packages.Count == 0)
            {
                Console.WriteLine("Không tìm thấy gói.");
                return false;
            }

            var Package = Packages.FirstOrDefault(o => o.Class_Id == ClassId && o.Package_Code == packagecode);
            if (Package == null)
            {
                Console.WriteLine("Không tìm thấy gói.");
                return false;
            }

            var ExamRoomPackages = await _httpClient.GetFromJsonAsync<List<Exam_Room_Package>>("https://localhost:7187/api/Student_Class/Get");
            if (ExamRoomPackages == null || ExamRoomPackages.Count == 0)
            {
                Console.WriteLine("Không tìm thấy gói phòng thi.");
                return false;
            }

            var ExamRoomPackage = ExamRoomPackages.FirstOrDefault(o => o.Package_Id == Package.Id);
            if (ExamRoomPackage == null)
            {
                Console.WriteLine("Không tìm thấy phòng thi cho gói.");
                return false;
            }

            var examrooms = await _httpClient.GetFromJsonAsync<List<Exam_Room>>("https://localhost:7187/api/Student_Class/Get");

            if (examrooms == null || examrooms.Count == 0)
            {
                Console.WriteLine("Không tìm thấy phòng thi.");
                return false;
            }

            DateTime currentTime = DateTime.Now;
            long time = ConvertLong.ConvertDateTimeToLong(currentTime);
            var examroom = examrooms.FirstOrDefault(o => o.Id == ExamRoomPackage.Exam_Room_Id);

            if (examroom.Start_Time <= time)
            {
                Console.WriteLine("Chưa đến thời gian làm bài thi.");
                return false;
            }
            else if (examroom.Start_Time >= time && (examroom.Start_Time - time) > 1500 && examroom.End_Time > time)
            {
                Console.WriteLine("Đã quá thời gian vào thi.");
                return false;
            }
            else if (examroom.Start_Time >= time && (examroom.Start_Time - time) > 1500 && examroom.End_Time < time)
            {
                Console.WriteLine("Đã hết thời gian thi.");
                return false;
            }

            return true;
        }
    }
}