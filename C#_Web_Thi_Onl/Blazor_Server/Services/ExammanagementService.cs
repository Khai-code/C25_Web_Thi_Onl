using Data_Base.GenericRepositories;
using Data_Base.Models.C;
using Data_Base.Models.E;
using Data_Base.Models.P;
using Data_Base.Models.R;
using Data_Base.Models.S;
using Data_Base.Models.U;
namespace Blazor_Server.Services
{
    public class ExammanagementService
    {
        private readonly HttpClient _httpClient;
        public ExammanagementService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<listexam>> GetallExam()
        {
            var ListExam = await _httpClient.GetFromJsonAsync<List<Exam>>("/api/Exam/Get") ?? new List<Exam>();
            var Listexamroom = await _httpClient.GetFromJsonAsync<List<Exam_Room>>("/api/Exam_Room/Get") ?? new List<Exam_Room>();
            var Listexamroompk = await _httpClient.GetFromJsonAsync<List<Exam_Room_Package>>("/api/Exam_Room_Package/Get") ?? new List<Exam_Room_Package>();
            var Listpackage = await _httpClient.GetFromJsonAsync<List<Package>>("/api/Package/Get") ?? new List<Package>();

            var result = from exam in ListExam
                         join room in Listexamroom on exam.Id equals room.Exam_Id into roomGroup
                         from room in roomGroup.DefaultIfEmpty()
                         join roomPk in Listexamroompk on room?.Id equals roomPk.Exam_Room_Id into roomPkGroup
                         from roomPk in roomPkGroup.DefaultIfEmpty()
                         join package in Listpackage on roomPk?.Package_Id equals package.Id into packageGroup
                         from package in packageGroup.DefaultIfEmpty()
                         where room == null || IsWithinCurrentWeek(room.Start_Time, room.End_Time)
                         group package by new { exam.Id, exam.Exam_Name } into grouped
                         select new listexam
                         {
                             Id = grouped.Key.Id,
                             NameExam = grouped.Key.Exam_Name,
                             Totalpackage = grouped.Count(p => p != null) 
                         };

            return result.ToList();
        }
        public async Task<List<listpackage>> GetallPackage(int id)
        {
            var Listpackage = await _httpClient.GetFromJsonAsync<List<Package>>("/api/Package/Get");
            var Listexamroompk = await _httpClient.GetFromJsonAsync<List<Exam_Room_Package>>("/api/Exam_Room_Package/Get");
            var Listexamroom = await _httpClient.GetFromJsonAsync<List<Exam_Room>>("/api/Exam_Room/Get");
            var Listroom = await _httpClient.GetFromJsonAsync<List<Room>>("/api/Room/Get");
            var result = from package in Listpackage
                         join exroompk in Listexamroompk on package.Id equals exroompk.Package_Id
                         join examroom in Listexamroom on exroompk.Exam_Room_Id equals examroom.Id
                         join room in Listroom on examroom.Room_Id equals room.ID
                         where IsWithinCurrentWeek(examroom.Start_Time, examroom.End_Time)&&examroom.Exam_Id==id
                         group package by new { package.Id, package.Package_Name, ExamRoomId = examroom.Id, examroom.Start_Time, examroom.End_Time, room.Room_Name } into grouped
                         select new listpackage
                         {
                             Id = grouped.Key.Id,
                             Idexam = grouped.Key.ExamRoomId,
                             NamePackage = grouped.Key.Package_Name,
                             StartTime = ConvertLong.ConvertLongToDateTime(grouped.Key.Start_Time),
                             EndTime = ConvertLong.ConvertLongToDateTime(grouped.Key.End_Time),
                             RoomName = grouped.Key.Room_Name,
                         };
            return result.ToList();
        }
        public async Task<List<listStudent>> GetAllStudent(int Id)
        {
            var Listpackage = await _httpClient.GetFromJsonAsync<List<Package>>("/api/Package/Get");
            var Lisclass = await _httpClient.GetFromJsonAsync<List<Class>>("/api/Class/Get");
            var Liststdclass = await _httpClient.GetFromJsonAsync<List<Student_Class>>("/api/Student_Class/Get");
            var Liststudent = await _httpClient.GetFromJsonAsync<List<Student>>("/api/Student/Get");
            var Listuser = await _httpClient.GetFromJsonAsync<List<User>>("/api/User/Get");
            var Listexroompk = await _httpClient.GetFromJsonAsync<List<Exam_Room_Package>>("/api/Exam_Room_Package/Get");
            var Listexroomstudent = await _httpClient.GetFromJsonAsync<List<Exam_Room_Student>>("/api/Exam_Room_Student/Get");
            var Listexhistories = await _httpClient.GetFromJsonAsync<List<Exam_HisTory>>("/api/Exam_HisTory/Get");
            var Listexamroom = await _httpClient.GetFromJsonAsync<List<Exam_Room>>("/api/Exam_Room/Get");

            var result = (from package in Listpackage
                          join classs in Lisclass on package.Class_Id equals classs.Id
                          join stdclass in Liststdclass on classs.Id equals stdclass.Class_Id
                          join student in Liststudent on stdclass.Student_Id equals student.Id
                          join user in Listuser on student.User_Id equals user.Id
                          join exroompk in Listexroompk on package.Id equals exroompk.Package_Id
                          join examroom in Listexamroom on exroompk.Exam_Room_Id equals examroom.Id
                          join exroomstd in Listexroomstudent on exroompk.Id equals exroomstd.Exam_Room_Package_Id into studentExamGroup
                          from studentExam in studentExamGroup.DefaultIfEmpty()
                          join exhistories in Listexhistories on studentExam?.Id equals exhistories.Exam_Room_Student_Id into historyGroup
                          from history in historyGroup.DefaultIfEmpty()
                          where package.Id == Id
                          select new listStudent
                          {
                              Id = student.Id,
                              NameStudent = user.Full_Name,
                              status = !Listexroomstudent.Any(x => x.Student_Id == stdclass.Student_Id) ? "Chưa thi"
                          : ConvertLong.ConvertLongToDateTime(examroom.Start_Time) > DateTime.Now ? "Chưa thi"
                          : (ConvertLong.ConvertLongToDateTime(studentExam.Check_Time) >= ConvertLong.ConvertLongToDateTime(examroom.Start_Time) &&
                             ConvertLong.ConvertLongToDateTime(studentExam.Check_Time) < ConvertLong.ConvertLongToDateTime(examroom.End_Time))
                          ? "Đang thi"
                          : (Listexhistories.Any(x => x.Exam_Room_Student_Id == studentExam?.Id &&
                                                      ConvertLong.ConvertLongToDateTime(x.Create_Time) >= ConvertLong.ConvertLongToDateTime(examroom.Start_Time) &&
                                                      ConvertLong.ConvertLongToDateTime(x.Create_Time) <= ConvertLong.ConvertLongToDateTime(examroom.End_Time))
                             ? "Đã hoàn thành bài thi"
                             : "Đã thi")
                          }).DistinctBy(s => s.Id).ToList();
            return result;
        }
        public async Task<bool> UpdateExamRoomTime(int id, Exam_Room exam_Room)
        {
            var existingExamRoom = await _httpClient.GetFromJsonAsync<Exam_Room>($"/api/Exam_Room/GetBy/{id}");

            if (existingExamRoom == null)
            {
                Console.WriteLine($" ID {id} không tồn tại.");
                return false;
            }
            existingExamRoom.Start_Time = exam_Room.Start_Time ;
            existingExamRoom.End_Time = exam_Room.End_Time;
            var response = await _httpClient.PutAsJsonAsync($"/api/Exam_Room/Pus/{id}", existingExamRoom);
            return response.IsSuccessStatusCode;
        }

        private bool IsWithinCurrentWeek(long startTime, long endTime)
        {
            DateTime today = DateTime.Today;
            DateTime startOfWeek = today.AddDays(-(int)today.DayOfWeek + (int)DayOfWeek.Monday);
            DateTime endOfWeek = startOfWeek.AddDays(7);
            DateTime roomStartTime = ConvertLong.ConvertLongToDateTime(startTime);
            DateTime roomEndTime = ConvertLong.ConvertLongToDateTime(endTime);
            return roomStartTime >= startOfWeek && roomEndTime <= endOfWeek;
        }

        public class listexam
        {
            public int Id { get; set; }
            public string NameExam { get; set; }
            public int Totalpackage { get; set; }
        }
        public class listpackage 
        {
            public int Id { get; set; }
            public int Idexam { get; set; }
            public string NamePackage { get; set; }
            public DateTime StartTime { get; set; }
            public DateTime EndTime { get; set; }
            public string RoomName {  get; set; }
        }
        public class listStudent
        {
            public int Id { get; set; }
            public string NameStudent { get; set; }
            public string status { get; set; }
        }
    }
}
