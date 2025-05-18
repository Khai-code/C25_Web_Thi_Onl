using Data_Base.Models.C;
using Data_Base.Models.E;
using Data_Base.Models.P;
using Data_Base.Models.S;
using Data_Base.Models.T;
using Data_Base.Models.U;
using static Blazor_Server.Services.ExammanagementService;
using static Blazor_Server.Services.Package_Test_ERP;

namespace Blazor_Server.Services
{
    public class PackageManager
    {
        private readonly HttpClient _httpClient;

        public PackageManager(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<PackageInactive>> GetPackageInactive()
        {
            var lstPackage = await _httpClient.GetFromJsonAsync<List<Package>>("https://localhost:7187/api/Package/Get");
            if (lstPackage == null || lstPackage.Count == 0)
                return new List<PackageInactive>();

            var lstClass = await _httpClient.GetFromJsonAsync<List<Class>>("https://localhost:7187/api/Class/Get");
            var lstSubject = await _httpClient.GetFromJsonAsync<List<Subject>>("https://localhost:7187/api/Subject/Get");
            var lstExamRoomPackage = await _httpClient.GetFromJsonAsync<List<Exam_Room_Package>>("https://localhost:7187/api/Exam_Room_Package/Get");
            var lstExamRoom = await _httpClient.GetFromJsonAsync<List<Exam_Room>>("https://localhost:7187/api/Exam_Room/Get");
            var lstTeacherClass = await _httpClient.GetFromJsonAsync<List<Teacher_Class>>("https://localhost:7187/api/Teacher_Class/Get");
            var lstExamRoomTeacher = await _httpClient.GetFromJsonAsync<List<Exam_Room_Teacher>>("https://localhost:7187/api/Exam_Room_Teacher/Get");
            var lstTeacher = await _httpClient.GetFromJsonAsync<List<Teacher>>("https://localhost:7187/api/Teacher/Get");
            var lstUser = await _httpClient.GetFromJsonAsync<List<User>>("https://localhost:7187/api/User/Get");

            // Dictionary
            var subjectDict = lstSubject.ToDictionary(s => s.Id);
            var classDict = lstClass.ToDictionary(c => c.Id);
            var examRoomDict = lstExamRoom.ToDictionary(e => e.Id);
            var teacherDict = lstTeacher.ToDictionary(t => t.Id);   
            var userDict = lstUser.ToDictionary(u => u.Id);

            var result = lstPackage
                .Where(p => p.Status == 0)
                .Select(p =>
                {
                    var examRoomPackage = lstExamRoomPackage.FirstOrDefault(x => x.Package_Id == p.Id);
                    examRoomDict.TryGetValue(examRoomPackage?.Exam_Room_Id ?? 0, out var examRoom);

                    // Lấy giáo viên phụ trách lớp
                    var teacherClassId = lstTeacherClass.FirstOrDefault(tc => tc.Class_Id == p.Class_Id)?.Teacher_Id;
                    var teacherClass = teacherDict.TryGetValue(teacherClassId ?? 0, out var tClass) ? tClass : null;
                    var teacherClassName = teacherClass != null && userDict.TryGetValue(teacherClass.User_Id, out var userClass) ? userClass.Full_Name : "N/A";

                    // Lấy giáo viên phụ trách phòng thi
                    var examRoomTeacherId = lstExamRoomTeacher.FirstOrDefault(et => et.Exam_Room_Id == examRoom?.Id)?.Teacher_Id;
                    var teacherExam = teacherDict.TryGetValue(examRoomTeacherId ?? 0, out var tExam) ? tExam : null;
                    var teacherExamRoomName = teacherExam != null && userDict.TryGetValue(teacherExam.User_Id, out var userExam) ? userExam.Full_Name : "N/A";

                    return new PackageInactive
                    {
                        Id = p.Id,
                        Name = p.Package_Name,
                        Code = p.Package_Code,
                        SubjectName = subjectDict.TryGetValue(p.Subject_Id, out var sub) ? sub.Subject_Name : "N/A",
                        ClassName = classDict.TryGetValue(p.Class_Id, out var cl) ? cl.Class_Name : "N/A",
                        ClassNub = classDict.TryGetValue(p.Class_Id, out var cln) ? cln.Number : 0,
                        StartTime = examRoom?.Start_Time ?? 0,
                        EndTime = examRoom?.End_Time ?? 0,
                        TeacherClass = teacherClassName,
                        TeacherExamRoom = teacherExamRoomName
                    };
                })
                .ToList();

            return result;
        }
        public async Task<List<TeacherViewModel>> GetTeacher()
        {
            var lstTea = await _httpClient.GetFromJsonAsync<List<Teacher>>("https://localhost:7187/api/Teacher/Get");
            if (lstTea == null || lstTea.Count == 0)
                return new List<TeacherViewModel>();

            var lstUser = await _httpClient.GetFromJsonAsync<List<User>>("https://localhost:7187/api/User/Get");
            if (lstUser == null || lstUser.Count == 0)
                return new List<TeacherViewModel>();

            var UserDict = lstUser.Where(u => u.Status != 1).ToDictionary(s => s.Id);

            var teacher = lstTea.Select(t =>
            {
                return new TeacherViewModel
                {
                    Teacher_Id = t.Id,
                    Teacher_Name = UserDict.TryGetValue(t.User_Id, out var use) ? use.Full_Name : "N/A"
                };
            }).ToList();

            return teacher;
        }

        public class TeacherViewModel
        {
            public int Teacher_Id { get; set; }
            public string Teacher_Name { get; set; }
        }

        public class PackageInactive
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public int Code { get; set; }
            public string SubjectName { get; set; }
            public string ClassName { get; set; }
            public string TeacherClass { get; set; }
            public string TeacherExamRoom { get; set; }
            public int ClassNub { get; set; }
            public long StartTime { get; set; }
            public long EndTime { get; set; }
        }
    }
}
