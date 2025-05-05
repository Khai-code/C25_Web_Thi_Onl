using Blazor_Server.Pages;
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
            var lstStudent = await _httpClient.GetFromJsonAsync<List<Student>>("https://localhost:7187/api/Student/Get");

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
                Console.WriteLine("Không tìm thấy lớp của sinh viên1.");
                return false;
            }

            var StudentClass = lstStudentClass.FirstOrDefault(o => o.Student_Id == studentId);
            if (StudentClass == null)
            {
                Console.WriteLine("Không tìm thấy lớp của sinh viên2.");
                return false;
            }

            int ClassId = StudentClass.Class_Id;

            var Packages = await _httpClient.GetFromJsonAsync<List<Data_Base.Models.P.Package>>("https://localhost:7187/api/Package/Get");
            if (Packages == null || Packages.Count == 0)
            {
                Console.WriteLine("Không tìm thấy gói1.");
                return false;
            }

            var Package = Packages.FirstOrDefault(o => o.Class_Id == ClassId && o.Package_Code == packagecode);
            if (Package == null)
            {
                Console.WriteLine("Không tìm thấy gói2.");
                return false;
            }

            var ExamRoomPackages = await _httpClient.GetFromJsonAsync<List<Exam_Room_Package>>("https://localhost:7187/api/Exam_Room_Package/Get");
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

            var examrooms = await _httpClient.GetFromJsonAsync<List<Exam_Room>>("https://localhost:7187/api/Exam_Room/Get");

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
                Console.WriteLine("Chưa đến thời gian làm bài thi1.");
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

        public async Task<bool> PostERStudent(int packagecode, string Studentcode)
        {
            try
            {
                var Packages = await _httpClient.GetFromJsonAsync<List<Data_Base.Models.P.Package>>("https://localhost:7187/api/Package/Get");
                if (Packages == null || Packages.Count == 0)
                {
                    Console.WriteLine("Không tìm thấy gói đề.");
                    return false;
                }

                var PackageId = Packages.FirstOrDefault(o => o.Package_Code == packagecode).Id;

                var lstExamRoomPackage = await _httpClient.GetFromJsonAsync<List<Exam_Room_Package>>("https://localhost:7187/api/Exam_Room_Package/Get");

                var ExamRoomPackageId = lstExamRoomPackage.FirstOrDefault(o => o.Package_Id == PackageId).Id;

                var Students = await _httpClient.GetFromJsonAsync<List<Data_Base.Models.S.Student>>("https://localhost:7187/api/Student/Get");

                var studetId = Students.FirstOrDefault(o => o.Student_Code == Studentcode).Id;
                
                DateTime dateTime = DateTime.Now;
                long dataLong = ConvertLong.ConvertDateTimeToLong(dateTime);

                Exam_Room_Student ERStudent = new Exam_Room_Student();

                ERStudent.Exam_Room_Package_Id = ExamRoomPackageId;
                ERStudent.Student_Id = studetId;
                ERStudent.Check_Time = dataLong;

                var Exam_Room_Student = await _httpClient.PostAsJsonAsync("https://localhost:7187/api/Package/Post", ERStudent);
                if (!Exam_Room_Student.IsSuccessStatusCode)
                    return false;

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<Dictionary<int, int>> GetRemainingExamTime(string studentCode, int packageCode)
        {
            var students = await _httpClient.GetFromJsonAsync<List<Student>>("/api/Student/Get") ?? new List<Student>();
            var examRoomStudents = await _httpClient.GetFromJsonAsync<List<Exam_Room_Student>>("/api/Exam_Room_Student/Get") ?? new List<Exam_Room_Student>();
            var examRoomPackages = await _httpClient.GetFromJsonAsync<List<Exam_Room_Package>>("/api/Exam_Room_Package/Get") ?? new List<Exam_Room_Package>();
            var packages = await _httpClient.GetFromJsonAsync<List<Data_Base.Models.P.Package>>("/api/Package/Get") ?? new List<Data_Base.Models.P.Package>();
            var pointTypes = await _httpClient.GetFromJsonAsync<List<Point_Type>>("/api/Point_Type/Get") ?? new List<Point_Type>();
            var examRooms = await _httpClient.GetFromJsonAsync<List<Exam_Room>>("/api/Exam_Room/Get") ?? new List<Exam_Room>();
            var result = from student in students
                         join examRoomStudent in examRoomStudents on student.Id equals examRoomStudent.Student_Id into examRoomStudentGroup
                         from examRoomStudent in examRoomStudentGroup.DefaultIfEmpty()
                         join examRoomPackage in examRoomPackages on examRoomStudent.Exam_Room_Package_Id equals examRoomPackage.Id into examRoomPackageGroup
                         from examRoomPackage in examRoomPackageGroup.DefaultIfEmpty()
                         join package in packages on examRoomPackage.Package_Id equals package.Id into packageGroup
                         from package in packageGroup.DefaultIfEmpty()
                         join pointType in pointTypes on package.Point_Type_Id equals pointType.Id into pointTypeGroup
                         from pointType in pointTypeGroup.DefaultIfEmpty()
                         join examRoom in examRooms on examRoomPackage.Exam_Room_Id equals examRoom.Id into examRoomGroup
                         from examRoom in examRoomGroup.DefaultIfEmpty()
                         where student.Student_Code == studentCode && package.Package_Code == packageCode
                         select new
                         {
                             examRoomStudent,
                             package,
                             pointType,
                             examRoom
                         };

            var data = result.FirstOrDefault();
            if (data == null || data.examRoomStudent?.Check_Time == null || data.package == null || data.pointType == null || data.examRoom == null)
            {
                return new Dictionary<int, int> { { 0, 0 } };
            }

            int examDurationMinutes = data.pointType.Point_Type_Name switch
            {
                "ATT" => 15,
                "T15" => 15,
                "T45" => 45,
                "MID" => 45,
                "FIN" => 60,
                _ => 30
            };

            DateTime checkTime = ConvertLong.ConvertLongToDateTime(data.examRoomStudent.Check_Time);
            DateTime examRoomEndTime = ConvertLong.ConvertLongToDateTime(data.examRoom.End_Time);
            var timeRemaining = examRoomEndTime - checkTime;
            int remainingMinutes = (int)timeRemaining.TotalMinutes;
            int remainingSeconds = timeRemaining.Seconds;

            var resultDictionary = new Dictionary<int, int>
                {
                    { remainingMinutes, remainingSeconds }
                };

            if (remainingMinutes >= examDurationMinutes)
            {
                resultDictionary = new Dictionary<int, int>
                {
                    { examDurationMinutes, 0 }
                };
            }

            return resultDictionary;
        }


    }
}