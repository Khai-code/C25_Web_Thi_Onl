using Blazor_Server.Pages;
using Data_Base.Filters;
using Data_Base.GenericRepositories;
using Data_Base.Models.C;
using Data_Base.Models.E;
using Data_Base.Models.P;
using Data_Base.Models.S;
using Data_Base.Models.T;
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
        public string ErrorMes { get; set; }
        public int secondsLeft { get; set; } = 0;
        public async Task<bool> CheckPackage(string Studentcode, int packagecode)
        {
            try
            {
                #region Package
                var filterPackage = new CommonFilterRequest
                {
                    Filters = new Dictionary<string, string>
                    {
                        { "Package_Code", packagecode.ToString() }
                    },
                };

                var lstPackages = await _httpClient.PostAsJsonAsync("https://localhost:7187/api/V_Package/common/get", filterPackage);

                if (!lstPackages.IsSuccessStatusCode)
                {
                    ErrorMes = "Gọi API khiểm tra gói đề thất bại";
                    return false;
                }


                var packages = (await lstPackages.Content.ReadFromJsonAsync<List<Data_Base.V_Model.V_Package>>()).SingleOrDefault();
                if (packages == null)
                {
                    ErrorMes = "Mã gói đề không hợp lệ";
                    return false;
                }

                if (packages.Status == 2)
                {
                    ErrorMes = "Bài thi đã kết thúc";
                    return false;
                }

                if (packages.GV1_Confirm == 0)
                {
                    ErrorMes = string.Format("Giám thị {0} chưa có xác nhận có mặt tại phòng thi", packages.GV1_Name);
                    return false;
                }

                if (packages.GV2_Confirm == 0)
                {
                    ErrorMes = string.Format("Giám thị {0} chưa có xác nhận có mặt tại phòng thi", packages.GV2_Name);
                    return false;
                }

                #endregion

                #region Student

                var filterStudent = new CommonFilterRequest
                {
                    Filters = new Dictionary<string, string>
                    {
                        { "Student_Code", Studentcode.ToString() }
                    },
                };

                var studentResponse = await _httpClient.PostAsJsonAsync("https://localhost:7187/api/Student/common/get", filterStudent);

                if (!studentResponse.IsSuccessStatusCode)
                {
                    ErrorMes = "Gọi API khiểm tra thông tin học sinh thất bại";
                    return false;
                }


                var student = (await studentResponse.Content.ReadFromJsonAsync<List<Data_Base.Models.S.Student>>()).SingleOrDefault();
                #endregion

                #region Student_Class
                var filterStudentClass = new CommonFilterRequest
                {
                    Filters = new Dictionary<string, string>
                    {
                        { "Student_Id", student.Id.ToString() }
                    },
                };

                var lstStudentClass = await _httpClient.PostAsJsonAsync("https://localhost:7187/api/Student_Class/common/get", filterStudentClass);

                if (!lstStudentClass.IsSuccessStatusCode)
                {
                    ErrorMes = "Gọi API khiểm tra thông tin học sinh thất bại";
                    return false;
                }

                var studentClass_ClassId = (await lstStudentClass.Content.ReadFromJsonAsync<List<Data_Base.Models.S.Student_Class>>()).SingleOrDefault().Class_Id;
                #endregion

                #region Exam_Room_Package
                var filterERP = new CommonFilterRequest
                {
                    Filters = new Dictionary<string, string>
                    {
                        { "Package_Id", packages.Id.ToString() }
                    },
                };

                var ExamRoomPackages = await _httpClient.PostAsJsonAsync("https://localhost:7187/api/Exam_Room_Package/common/get", filterERP);

                if (!ExamRoomPackages.IsSuccessStatusCode)
                {
                    ErrorMes = "Gọi API khiểm tra gói đề thất bại";
                    return false;
                }

                var ExamRoomPackage = (await ExamRoomPackages.Content.ReadFromJsonAsync<List<Data_Base.Models.E.Exam_Room_Package>>()).FirstOrDefault();

                #endregion

                #region Exam_Room
                var filterExamRoom = new CommonFilterRequest
                {
                    Filters = new Dictionary<string, string>
                    {
                        { "Id", ExamRoomPackage.Exam_Room_Id.ToString() }
                    },
                };

                var examrooms = await _httpClient.PostAsJsonAsync("https://localhost:7187/api/Exam_Room/common/get", filterExamRoom);

                if (!examrooms.IsSuccessStatusCode)
                {
                    ErrorMes = "Gọi API khiểm tra phòng thi và thời gian thi thất bại";
                    return false;
                }

                var examroom = (await examrooms.Content.ReadFromJsonAsync<List<Data_Base.Models.E.Exam_Room>>()).FirstOrDefault();

                if (examroom == null)
                {
                    ErrorMes = "Gọi API khiểm tra phòng thi và thời gian thi thất bại";
                    return false;
                }

                #endregion

                #region Check thời gian ko hợp lệ
                DateTime currentTime = DateTime.Now;
                long time = ConvertLong.ConvertDateTimeToLong(currentTime);
                TimeSpan duration = currentTime - ConvertLong.ConvertLongToDateTime(examroom.Start_Time);
                if (examroom.Start_Time > time)
                {
                    ErrorMes = "Chưa đến thời gian làm bài thi";
                    return false;
                }
                else if (examroom.End_Time < time)
                {
                    ErrorMes = "Đã hết thời gian thi.";
                    if (packages.Status != 2)
                    {
                        Data_Base.Models.P.Package packageNew = new Data_Base.Models.P.Package();
                        packageNew.Id = packages.Id;
                        packageNew.Package_Code = packages.Package_Code;
                        packageNew.Package_Name = packages.Package_Name;
                        packageNew.Create_Time = packages.Create_Time;
                        packageNew.Number_Of_Questions = packages.Number_Of_Questions;
                        packageNew.ExecutionTime = packages.ExecutionTime;
                        packageNew.Status = 2;
                        packageNew.Subject_Id = packages.Subject_Id;
                        packageNew.Class_Id = packages.Class_Id;
                        packageNew.Package_Type_Id = packages.Package_Type_Id;
                        packageNew.Point_Type_Id = packages.Point_Type_Id;
                        packageNew.Teacher_Id = packages.TeacherPackage_Id;

                        var UpdatePackage = await _httpClient.PutAsJsonAsync($"https://localhost:7187/api/Package/Pus/{packages.Id}", packageNew);
                        if (!UpdatePackage.IsSuccessStatusCode)
                        {
                            ErrorMes += string.Format("Gọi API cậu nhập trạng thái gói đề thất bại ");
                            return false;
                        }
                    }
                    return false;
                }
                else if (examroom.Start_Time <= time
                    && examroom.End_Time > time
                    && (packages.Point_Type_Id == 1 || packages.Point_Type_Id == 2)
                    && duration.TotalSeconds > 300)
                {
                    ErrorMes = "Đã quá thời gian vào thi.";
                    return false;
                }
                else if (examroom.Start_Time <= time
                    && examroom.End_Time > time
                    && (packages.Point_Type_Id == 3 || packages.Point_Type_Id == 4 || packages.Point_Type_Id == 5)
                    && duration.TotalSeconds > 1500)
                {
                    ErrorMes = "Đã quá thời gian vào thi.";
                    return false;
                }
                #endregion

                #region Create Test vs Exam_Room_Student
                var filterEXS = new CommonFilterRequest
                {
                    Filters = new Dictionary<string, string>
                    {
                        { "Exam_Room_Package_Id", ExamRoomPackage.Id.ToString() },
                        { "Student_Id", student.Id.ToString() }
                    },
                };

                var exsReq = await _httpClient.PostAsJsonAsync("https://localhost:7187/api/Exam_Room_Student/common/get", filterEXS);

                var exs = (await exsReq.Content.ReadFromJsonAsync<List<Data_Base.Models.E.Exam_Room_Student>>()).ToList();

                foreach (var item in exs.Where(o => o.Is_Check_Review == 0))
                {
                    if (item.Is_Check_Out == 0)
                    {
                        ErrorMes = string.Format("Học sinh có mã (0) đã vào thi, không thể đăng nhập lại", student.Student_Code);
                        return false;
                    }
                }

                Data_Base.Models.T.Test test = new Data_Base.Models.T.Test();
                test.Test_Code = string.Empty;
                test.Modifi_Time = 0;
                test.Is_Check_Cheat= 0;
                test.Reason = string.Empty;
                test.Package_Id = packages.Id;
                test.Student_Id = student.Id;
                test.Status = 0;

                var testReport = await _httpClient.PostAsJsonAsync("https://localhost:7187/api/Test/Post", test);

                if (!testReport.IsSuccessStatusCode)
                {
                    ErrorMes = "Tạo bài thi thất bại";
                    return false;
                }

                var filterTest = new CommonFilterRequest
                {
                    Filters = new Dictionary<string, string>
                    {
                        { "Package_Id", packages.Id.ToString() }
                    },
                };

                var testRep = await _httpClient.PostAsJsonAsync("https://localhost:7187/api/Test/common/get", filterTest);

                if (!testRep.IsSuccessStatusCode)
                {
                    ErrorMes = "Gọi API kiểm tra bài thi thất bại";
                    return false;
                }

                var lstTest = (await testRep.Content.ReadFromJsonAsync<List<Data_Base.Models.T.Test>>()).ToList();

                if (lstTest != null && lstTest.Count > 0 && lstTest.Count < packages.Number)
                {
                    Data_Base.Models.P.Package packageModel = new Data_Base.Models.P.Package();
                    packageModel.Id = packages.Id;
                    packageModel.Package_Code = packages.Package_Code;
                    packageModel.Package_Name = packages.Package_Name;
                    packageModel.Create_Time = packages.Create_Time;
                    packageModel.Number_Of_Questions = packages.Number_Of_Questions;
                    packageModel.ExecutionTime = packages.ExecutionTime;
                    packageModel.Status = 1;
                    packageModel.Subject_Id = packages.Subject_Id;
                    packageModel.Class_Id = packages.Class_Id;
                    packageModel.Package_Type_Id = packages.Package_Type_Id;
                    packageModel.Point_Type_Id = packages.Point_Type_Id;
                    packageModel.Teacher_Id = packages.TeacherPackage_Id;

                    var UpdatePackage = await _httpClient.PutAsJsonAsync($"https://localhost:7187/api/Package/Pus/{packageModel.Id}", packageModel);
                    if (!UpdatePackage.IsSuccessStatusCode)
                    {
                        return false;
                    }
                }
                else if (lstTest != null && lstTest.Count > 0 && lstTest.Count == packages.Number)
                {
                    Data_Base.Models.P.Package packageModel = new Data_Base.Models.P.Package();
                    packageModel.Id = packages.Id;
                    packageModel.Package_Code = packages.Package_Code;
                    packageModel.Create_Time = packageModel.Create_Time;
                    packageModel.Number_Of_Questions = packages.Number_Of_Questions;
                    packageModel.ExecutionTime = packageModel.ExecutionTime;
                    packageModel.Status = 2;
                    packageModel.Subject_Id = packages.Subject_Id;
                    packageModel.Class_Id = packages.Class_Id;
                    packageModel.Package_Type_Id = packages.Package_Type_Id;
                    packageModel.Point_Type_Id = packages.Point_Type_Id;
                    packageModel.Teacher_Id = packages.TeacherPackage_Id;

                    var UpdatePackage = await _httpClient.PutAsJsonAsync($"https://localhost:7187/api/Package/Pus/{packages.Id}", packageModel);
                    if (!UpdatePackage.IsSuccessStatusCode)
                    {
                        return false;
                    }
                }

                var testId = (await testReport.Content.ReadFromJsonAsync<Data_Base.Models.T.Test>()).Id;

                Exam_Room_Student ERStudent = new Exam_Room_Student();

                ERStudent.Exam_Room_Package_Id = ExamRoomPackage.Id;
                ERStudent.Student_Id = student.Id;
                ERStudent.Is_Check_Out = 0;
                ERStudent.Is_Check_Review = 0;
                ERStudent.Check_Time = time;
                ERStudent.Test_Id = testId;

                var Exam_Room_Student_Post = await _httpClient.PostAsJsonAsync("https://localhost:7187/api/Exam_Room_Student/Post", ERStudent);
                if (!Exam_Room_Student_Post.IsSuccessStatusCode)
                {
                    await _httpClient.DeleteAsync($"https://localhost:7187/api/Test/Delete/{testId}");
                    ErrorMes = "Gọi API xác thực học sinh vào thi thất bại";
                    return false;
                }

                var Exam_Room_Student = (await Exam_Room_Student_Post.Content.ReadFromJsonAsync<Data_Base.Models.E.Exam_Room_Student>());

                if (Exam_Room_Student == null)
                {
                    ErrorMes = "Gọi API xác thực học sinh vào thi thất bại";
                    _httpClient.DeleteAsync($"https://localhost:7187/api/Exam_Room_Student/Delete/{Exam_Room_Student.Id}");
                    _httpClient.DeleteAsync($"https://localhost:7187/api/Test/Delete/{testId}");
                    return false;
                }
                #endregion

                #region Đếm thời gian

                //CountingTime(Exam_Room_Student, examroom);

                int timeout = (int)(ConvertLong.ConvertLongToDateTime(examroom.End_Time) - ConvertLong.ConvertLongToDateTime(Exam_Room_Student.Check_Time)).TotalSeconds;

                secondsLeft = timeout > 0 ? timeout : 0;

                if ((packages.Point_Type_Id == 1 || packages.Point_Type_Id == 2) && secondsLeft > 900) secondsLeft = 900;
                else if (packages.Point_Type_Id == 3 && secondsLeft > 2700) secondsLeft = 2700;
                else if ((packages.Point_Type_Id == 4 || packages.Point_Type_Id == 5) && secondsLeft > 5400) secondsLeft = 5400;
                #endregion
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private long CountingTime(Data_Base.Models.E.Exam_Room_Student exam_Room_Student, Data_Base.Models.E.Exam_Room exam_Room)
        {
            try
            {
                if (exam_Room_Student == null || exam_Room == null)
                {
                    int testId = exam_Room_Student.Test_Id;
                   
                    return 0;
                }    
                    

                int time = (int)(ConvertLong.ConvertLongToDateTime(exam_Room.End_Time) - ConvertLong.ConvertLongToDateTime(exam_Room_Student.Check_Time)).TotalSeconds;

                return secondsLeft = time > 0 ? time : 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi đếm thời gian: {ex.Message}");
                return 0;
            }
        }

        #region ko dùng nữa
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

                if (PackageId <= 0)
                    return false;

                Data_Base.Models.T.Test test = new Data_Base.Models.T.Test();
                test.Package_Id = PackageId;

                var TestReport = await _httpClient.PostAsJsonAsync("https://localhost:7187/api/Test/Post", test);

                if (!TestReport.IsSuccessStatusCode)
                    return false;

                var TestReportModel = await TestReport.Content.ReadFromJsonAsync<List<Data_Base.Models.T.Test>>();

                var TestId = TestReportModel.Select(o => o.Id).FirstOrDefault();

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
                ERStudent.Test_Id = TestId;

                var Exam_Room_Student = await _httpClient.PostAsJsonAsync("https://localhost:7187/api/Exam_Room_Student/Post", ERStudent);
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
        #endregion

    }
}