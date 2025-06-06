using Data_Base.GenericRepositories;
using Data_Base.Models.C;
using Data_Base.Models.E;
using Data_Base.Models.P;
using Data_Base.Models.R;
using Data_Base.Models.S;
using Data_Base.Models.U;
using System.Net.WebSockets;
using System.Text.RegularExpressions;
namespace Blazor_Server.Services
{
    public class ExammanagementService
    {
        private readonly HttpClient _httpClient;
        public ExammanagementService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<List<listexam>> SeachExam(DateTime start, DateTime end)
        {
            var result = new List<listexam>();
            var listExam = await _httpClient.GetFromJsonAsync<List<Exam>>("/api/Exam/Get") ?? new List<Exam>();
            var examRooms = await _httpClient.GetFromJsonAsync<List<Exam_Room>>("/api/Exam_Room/Get") ?? new List<Exam_Room>();
            var roomPackages = await _httpClient.GetFromJsonAsync<List<Exam_Room_Package>>("/api/Exam_Room_Package/Get") ?? new List<Exam_Room_Package>();
            var packages = await _httpClient.GetFromJsonAsync<List<Data_Base.Models.P.Package>>("/api/Package/Get") ?? new List<Data_Base.Models.P.Package>();
            foreach (var exam in listExam)
            {
                // Lọc các phòng thi thuộc bài thi hiện tại và có thời gian bắt đầu hoặc kết thúc trong khoảng start-end
                var validRooms = examRooms
                    .Where(x => x.Exam_Id == exam.Id)
                    .Where(x =>
                    {
                        var roomStart = ConvertLong.ConvertLongToDateTime(x.Start_Time);
                        var roomEnd = ConvertLong.ConvertLongToDateTime(x.End_Time);
                        // Chỉnh lại điều kiện: phòng thi có bắt đầu hoặc kết thúc trong khoảng start - end
                        return (roomStart >= start && roomStart <= end) || (roomEnd >= start && roomEnd <= end);
                    })
                    .ToList();

                if (!validRooms.Any()) continue;

                int totalPackage = 0;

                foreach (var room in validRooms)
                {
                    var rpk = roomPackages.Where(x => x.Exam_Room_Id == room.Id);
                    foreach (var pkg in rpk)
                    {
                        if (packages.Any(x => x.Id == pkg.Package_Id))
                        {
                            totalPackage++;
                        }
                    }
                }

                result.Add(new listexam
                {
                    Id = exam.Id,
                    NameExam = exam.Exam_Name,
                    Totalpackage = totalPackage
                });
            }

            return result;
        }

        public async Task<List<listexam>> GetallExam()
        {
            var result = new List<listexam>();
            var ListExam = await _httpClient.GetFromJsonAsync<List<Exam>>("/api/Exam/Get") ?? new List<Exam>();
            foreach(var exam in ListExam)
            {
                var examroom = await _httpClient.GetFromJsonAsync<List<Exam_Room>>("/api/Exam_Room/Get") ?? new List<Exam_Room>();
                var valiTime = examroom.Where(x=>x.Exam_Id==exam.Id).ToList();
                //if (!valiTime.Any()) continue;
                int total = 0;
                foreach(var room in valiTime)
                {
                    var roomPackages = await _httpClient.GetFromJsonAsync<List<Exam_Room_Package>>("/api/Exam_Room_Package/Get") ?? new List<Exam_Room_Package>();
                    var rpk = roomPackages.Where(x => x.Exam_Room_Id == room.Id);
                    foreach(var pakage in rpk)
                    {
                        var package = await _httpClient.GetFromJsonAsync<List<Data_Base.Models.P.Package>>("/api/Package/Get") ?? new List<Data_Base.Models.P.Package>();
                        var list = package.Where(x => x.Id == pakage.Package_Id);
                        if (list != null) total++;
                    }
                }
                result.Add(new listexam
                {
                    Id = exam.Id,
                    NameExam = exam.Exam_Name,
                    Totalpackage = total
                });
            }
            return result;
        }
        public async Task<List<listpackage>> GetallPackage(int id)
        {
            var listPackage = await _httpClient.GetFromJsonAsync<List<Data_Base.Models.P.Package>>("/api/Package/Get") ?? new List<Data_Base.Models.P.Package>();

            var tasks = listPackage.Select(async package =>
            {
                var listExamRoomPackage = await _httpClient.GetFromJsonAsync<List<Exam_Room_Package>>("/api/Exam_Room_Package/Get") ?? new List<Exam_Room_Package>();
                var exrPackage = listExamRoomPackage.FirstOrDefault(x => x.Package_Id == package.Id);
                if (exrPackage == null) return null;
                var listExamRoom = await _httpClient.GetFromJsonAsync<List<Exam_Room>>("/api/Exam_Room/Get")?? new List<Exam_Room>();
                var examRoom = listExamRoom.FirstOrDefault(x =>x.Id == exrPackage.Exam_Room_Id &&x.Exam_Id == id && IsOverlapCurrentWeek(x.Start_Time, x.End_Time));
                if (examRoom == null) return null;
                var listRoom = await _httpClient.GetFromJsonAsync<List<Room>>("/api/Room/Get")?? new List<Room>();
                var room = listRoom.FirstOrDefault(x => x.ID == examRoom.Room_Id);
                if (room == null) return null;
                return new listpackage
                {
                    Id = package.Id,
                    Idexam = examRoom.Id,
                    NamePackage = package.Package_Name,
                    StartTime = ConvertLong.ConvertLongToDateTime(examRoom.Start_Time),
                    EndTime = ConvertLong.ConvertLongToDateTime(examRoom.End_Time),
                    RoomName = room.Room_Name
                };
            });

            var results = await Task.WhenAll(tasks);
            return results.Where(x => x != null).Distinct().ToList();

        }

        public async Task<List<listStudent>> GetAllStudent(int Id)
        {
            var Listpackage = await _httpClient.GetFromJsonAsync<List<Data_Base.Models.P.Package>>("/api/Package/Get") ?? new List<Data_Base.Models.P.Package>();
            var package = Listpackage.FirstOrDefault(x => x.Id == Id);
            if (package == null) return new List<listStudent>();
            var Lisclass = await _httpClient.GetFromJsonAsync<List<Class>>("/api/Class/Get") ?? new List<Class>();
            var classpackage = Lisclass.FirstOrDefault(x => x.Id == package.Class_Id);
            if (classpackage == null) return new List<listStudent>();
            var Liststdclass = await _httpClient.GetFromJsonAsync<List<Student_Class>>("/api/Student_Class/Get") ?? new List<Student_Class>();
            var studentClasses = Liststdclass.Where(x => x.Class_Id == classpackage.Id).ToList();
            if (!studentClasses.Any()) return new List<listStudent>();
            var Liststudent = await _httpClient.GetFromJsonAsync<List<Student>>("/api/Student/Get") ?? new List<Student>();
            var Listuser = await _httpClient.GetFromJsonAsync<List<User>>("/api/User/Get") ?? new List<User>();
            var Listexroompk = await _httpClient.GetFromJsonAsync<List<Exam_Room_Package>>("/api/Exam_Room_Package/Get") ?? new List<Exam_Room_Package>();
            var expk = Listexroompk.FirstOrDefault(x => x.Package_Id == package.Id);
            if (expk == null) return new List<listStudent>();
            var Listexamroom = await _httpClient.GetFromJsonAsync<List<Exam_Room>>("/api/Exam_Room/Get") ?? new List<Exam_Room>();
            var exroom = Listexamroom.FirstOrDefault(x => x.Id == expk.Exam_Room_Id);
            if (exroom == null) return new List<listStudent>();
            var Listexroomstudent = await _httpClient.GetFromJsonAsync<List<Exam_Room_Student>>("/api/Exam_Room_Student/Get") ?? new List<Exam_Room_Student>();
            var Listexhistories = await _httpClient.GetFromJsonAsync<List<Exam_HisTory>>("/api/Exam_HisTory/Get") ?? new List<Exam_HisTory>();
            var result = new List<listStudent>();
            foreach (var studentClass in studentClasses)
            {
                var student = Liststudent.FirstOrDefault(s => s.Id == studentClass.Student_Id);
                if (student == null) continue;

                var user = Listuser.FirstOrDefault(u => u.Id == student.User_Id);
                if (user == null) continue;

                var exstd = Listexroomstudent.FirstOrDefault(x => x.Exam_Room_Package_Id == expk.Id && x.Student_Id == studentClass.Student_Id);

                string status = "Chưa thi";

                if (exstd != null)
                {
                    var startTime = ConvertLong.ConvertLongToDateTime(exroom.Start_Time);
                    var endTime = ConvertLong.ConvertLongToDateTime(exroom.End_Time);

                    if (startTime > DateTime.Now)
                    {
                        status = "Chưa thi";
                    }
                    else if (ConvertLong.ConvertLongToDateTime(exstd.Check_Time) >= startTime &&
                             ConvertLong.ConvertLongToDateTime(exstd.Check_Time) < endTime)
                    {
                        status = "Đang thi";
                    }
                    else if (Listexhistories.Any(h => h.Exam_Room_Student_Id == exstd.Id &&
                                                      ConvertLong.ConvertLongToDateTime(h.Create_Time) >= startTime &&
                                                      ConvertLong.ConvertLongToDateTime(h.Create_Time) <= endTime))
                    {
                        status = "Đã hoàn thành bài thi";
                    }
                    else
                    {
                        status = "Đã thi";
                    }
                }

                result.Add(new listStudent
                {
                    Id = student.Id,
                    NameStudent = user.Full_Name,
                    status = status
                });
            }

            return result.ToList();
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
       
        private bool IsOverlapCurrentWeek(long startTime, long endTime)
        {
            DateTime today = DateTime.Today;
            int diff = today.DayOfWeek == DayOfWeek.Sunday ? -6 : DayOfWeek.Monday - today.DayOfWeek;
            DateTime startOfWeek = today.AddDays(diff).Date;
            DateTime endOfWeek = startOfWeek.AddDays(7).AddTicks(-1);

            DateTime roomStartTime = ConvertLong.ConvertLongToDateTime(startTime);
            DateTime roomEndTime = ConvertLong.ConvertLongToDateTime(endTime);
            if (roomStartTime < startOfWeek || roomStartTime > endOfWeek)
            {
                return false;
            }

            // Chỉ cần thời gian thi có GIAO với tuần hiện tại là tính
            return roomStartTime <= endOfWeek && roomEndTime >= startOfWeek;
           
        }


        public async Task<Exam> AddExam(Exam exam)
        {
            try
            {
                var addexam = await _httpClient.PostAsJsonAsync("/api/Exam/Post", exam);

                if (!addexam.IsSuccessStatusCode)
                    return null;

                // Giải mã nội dung từ phản hồi thành đối tượng Exam
                var examResult = await addexam.Content.ReadFromJsonAsync<Exam>();

                return examResult; // Trả về đối tượng Exam đã giải mã
            }
            catch (Exception ex)
            {
                // Có thể ghi log exception nếu cần
                throw new ApplicationException("Có lỗi xảy ra khi thêm kỳ thi.", ex);
            }
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
