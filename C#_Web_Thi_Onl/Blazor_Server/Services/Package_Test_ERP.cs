using Blazor_Server.Pages;
using Data_Base.Filters;
using Data_Base.Models.C;
using Data_Base.Models.E;
using Data_Base.Models.P;
using Data_Base.Models.Q;
using Data_Base.Models.R;
using Data_Base.Models.S;
using Data_Base.Models.T;
using Data_Base.V_Model;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.IdentityModel.Tokens;
using static Blazor_Server.Services.ExamService;

namespace Blazor_Server.Services
{
    public class Package_Test_ERP
    {
        private readonly HttpClient _httpClient;

        public Package_Test_ERP(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public string ErrorMes { get; set; }

        public async Task<bool> CheckTeacher(int TeacherId, long StartTime, long EndTime)
        {
            bool success = true;
            try
            {
                var filterRequest = new CommonFilterRequest
                {
                    Filters = new Dictionary<string, string>
                    {
                        { "Teacher_Id", TeacherId.ToString() }
                    },
                };

                var ExamRoomTeacherResponse = await _httpClient.PostAsJsonAsync("https://localhost:7187/api/Exam_Room_Teacher/common/get", filterRequest);
                if (!ExamRoomTeacherResponse.IsSuccessStatusCode)
                {
                    ErrorMes = "Gọi API kiểm tra phòng thi thất bại";
                    return false;
                }

                List<int> lstExamRoomId = (await ExamRoomTeacherResponse.Content.ReadFromJsonAsync<List<Exam_Room_Teacher>>()).Select(o => o.Exam_Room_Id).ToList();

                if (lstExamRoomId != null && lstExamRoomId.Count > 0)
                {
                    var filterRequestER = new CommonFilterRequest
                    {
                        Filters = new Dictionary<string, string>
                    {
                        { "Exam_Room_Id", string.Join(",", lstExamRoomId) }
                    },
                    };

                    var ExamRoomResponse = await _httpClient.PostAsJsonAsync("https://localhost:7187/api/Exam_Room/common/get", filterRequestER);
                    if (!ExamRoomResponse.IsSuccessStatusCode)
                    {
                        ErrorMes = "Gọi API kiểm tra phòng thi thất bại";
                        return false;
                    }

                    var lstExamRoom = await ExamRoomResponse.Content.ReadFromJsonAsync<List<Exam_Room>>();

                    foreach (var item in lstExamRoom)
                    {
                        if ((item.Start_Time <= StartTime && item.End_Time >= StartTime) || (item.End_Time >= EndTime && item.Start_Time <= EndTime)
                            || (StartTime.ToString().Substring(0, 8) == item.Start_Time.ToString().Substring(0, 8) && StartTime <= item.Start_Time && EndTime >= item.End_Time))
                        {
                            ErrorMes = string.Format("trong khoảng thời gian từ {0} đến {1} đã được phân công coi thi.", StartTime, EndTime);
                            return false;
                        }
                    }
                }
                
                return success;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public async Task<bool> CheckPackage(PackageTestADO model)
        {
            bool success = true;
            try
            {
                var filterRequest = new CommonFilterRequest
                {
                    Filters = new Dictionary<string, string>
                    {
                        { "Room_Id", model.Exam_Room.Room_Id.ToString() }
                    },
                };
                   
                var ExamRoomResponse = await _httpClient.PostAsJsonAsync("https://localhost:7187/api/Exam_Room/common/get", filterRequest);
                if (!ExamRoomResponse.IsSuccessStatusCode)
                {
                    ErrorMes = "Gọi API kiểm tra phòng thi thất bại";
                    return false;
                }    

                var lstExamRoom = await ExamRoomResponse.Content.ReadFromJsonAsync<List<Exam_Room>>();

                //if(lstExamRoom == null && lstExamRoom.Count <= 0)
                //    return false;

                if (lstExamRoom != null && lstExamRoom.Count > 0)
                {
                    foreach (var item in lstExamRoom)
                    {
                        if (
                            (model.Exam_Room.Start_Time < item.Start_Time && model.Exam_Room.End_Time < item.Start_Time)
                            || (model.Exam_Room.Start_Time > item.End_Time && model.Exam_Room.End_Time > item.End_Time))
                        {
                            success = true;
                        }
                        else
                        {
                            ErrorMes = string.Format("Trong khoảng thời gian từ (0) đến (1) tại phòng (2) đang có ca thi", model.Exam_Room.Start_Time, model.Exam_Room.End_Time, model.Exam_Room.Room_Id);
                            return success = false;
                        }
                    }
                }
                return success;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<PackageTestADO> AddPackageTestERP(PackageTestADO model)
        {

            try
            {
                bool check = await CheckPackage(model);

                if (!check)
                    return null;

                var model_Package = await _httpClient.PostAsJsonAsync("https://localhost:7187/api/Package/Post", model.Package);
                if (!model_Package.IsSuccessStatusCode)
                {
                    var errorContent = await model_Package.Content.ReadAsStringAsync();
                    Console.WriteLine("Lỗi khi gọi API Package/Post:");
                    Console.WriteLine(errorContent);
                    ErrorMes = "Gọi API tạo gói đề thất bại";
                    return null;
                }

                var addedPackage = await model_Package.Content.ReadFromJsonAsync<Data_Base.Models.P.Package>();

                var model_Exam_Room = await _httpClient.PostAsJsonAsync("https://localhost:7187/api/Exam_Room/Post", model.Exam_Room);
                if (!model_Exam_Room.IsSuccessStatusCode)
                {
                    ErrorMes = "Gọi API tạo ca thi thất bại";
                    return null;
                }    

                var addedExam_Room = await model_Exam_Room.Content.ReadFromJsonAsync<Exam_Room>();

                model.Exam_Room_Package.Exam_Room_Id = addedExam_Room.Id;
                model.Exam_Room_Package.Package_Id = addedPackage.Id;
                foreach (var item in model.lstTeacherId)
                {
                    Exam_Room_Teacher tea = new Exam_Room_Teacher();
                    tea.Exam_Room_Id = addedExam_Room.Id;
                    tea.Teacher_Id = item;

                    model.lstExamRoomTeacher.Add(tea);
                }
                

                var model_Exam_Room_Package = await _httpClient.PostAsJsonAsync("https://localhost:7187/api/Exam_Room_Package/Post", model.Exam_Room_Package);

                if (!model_Exam_Room_Package.IsSuccessStatusCode)
                {
                    ErrorMes = "Gọi API tạo ca thi thất bại";
                    return null;
                }   

                var model_Exam_Room_Teacher = await _httpClient.PostAsJsonAsync("https://localhost:7187/api/Exam_Room_Teacher/PostList", model.lstExamRoomTeacher);
                if (!model_Exam_Room_Teacher.IsSuccessStatusCode)
                {
                    var errorContent = await model_Exam_Room_Teacher.Content.ReadAsStringAsync();
                    Console.WriteLine("Lỗi khi gọi API Exam_Room_Teacher/PostList:");
                    Console.WriteLine(errorContent);
                    ErrorMes = "Gọi API khiểm tra giảng viên coi thi thất bại";
                    return null;
                }

                return new PackageTestADO
                {
                    Package = addedPackage,
                    Exam_Room = addedExam_Room,
                    Exam_Room_Package = model.Exam_Room_Package,
                    lstExamRoomTeacher = model.lstExamRoomTeacher,
                    lstTeacherId = model.lstTeacherId
                };
            }
            catch (Exception ex)
            {
                ErrorMes = ex.Message;
                return null;
            }
        }

        public async Task<V_Package> FillPackage(int packageId)
        {
            try
            {
                if (packageId == null || packageId <= 0)
                {
                    return null;
                }

                var filterRequest = new CommonFilterRequest
                {
                    Filters = new Dictionary<string, string>
                    {
                        { "Id", packageId.ToString() }
                    },
                };

                var packageGetResponse = await _httpClient.PostAsJsonAsync("https://localhost:7187/api/V_Package/common/get", filterRequest);
                if (!packageGetResponse.IsSuccessStatusCode)
                    return null;

                var package = (await packageGetResponse.Content.ReadFromJsonAsync<List<V_Package>>()).SingleOrDefault();


                return package;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<bool> UpdatePackage(V_Package packageTestADO)
        {
            try
            {
                if (packageTestADO == null)
                {
                    return false;
                }

                Data_Base.Models.P.Package packageModel = new Data_Base.Models.P.Package();
                packageModel.Id = packageTestADO.Id;
                packageModel.Package_Code = packageTestADO.Package_Code;
                packageModel.Package_Name = packageTestADO.Package_Name;
                packageModel.Create_Time = packageTestADO.Create_Time;
                packageModel.Number_Of_Questions = packageTestADO.Number_Of_Questions;
                packageModel.ExecutionTime = packageTestADO.ExecutionTime;
                packageModel.Status = packageTestADO.Status;
                packageModel.Subject_Id = packageTestADO.Subject_Id;
                packageModel.Class_Id = packageTestADO.Class_Id;
                packageModel.Package_Type_Id = packageTestADO.Package_Type_Id;
                packageModel.Teacher_Id = packageTestADO.TeacherPackage_Id;
                packageModel.Um_Lock = 0;
                packageModel.Point_Type_Id = packageTestADO.Point_Type_Id;
                packageModel.Teacher_Mark_Id = packageTestADO.TeacherMark_Id;

                var package = await _httpClient.PutAsJsonAsync($"https://localhost:7187/api/Package/Pus/{packageTestADO.Id}", packageModel);

                if (!package.IsSuccessStatusCode)
                {
                    ErrorMes = "Cập nhập gói đề thất bại";
                    return false;
                }

                Data_Base.Models.E.Exam_Room examRoomModel = new Exam_Room();
                examRoomModel.Id = packageTestADO.Exam_Room_Id;
                examRoomModel.Start_Time = packageTestADO.Start_Time;
                examRoomModel.End_Time = packageTestADO.End_Time;
                examRoomModel.Room_Id = packageTestADO.Room_Id;
                examRoomModel.Exam_Id = packageTestADO.Exam_Id;

                var examroom = await _httpClient.PutAsJsonAsync($"https://localhost:7187/api/Exam_Room/Pus/{packageTestADO.Exam_Room_Id}", examRoomModel);

                if (!examroom.IsSuccessStatusCode)
                {
                    ErrorMes = "Cập nhập phòng thi thất bại";
                    return false;
                }

                Data_Base.Models.E.Exam_Room_Package examRoomPackageModel = new Exam_Room_Package();
                examRoomPackageModel.Id = packageTestADO.Exam_Room_Package_Id;
                examRoomPackageModel.Package_Id = packageTestADO.Id;
                examRoomPackageModel.Exam_Room_Id = packageTestADO.Exam_Room_Id;

                var examRoomPackage = await _httpClient.PutAsJsonAsync($"https://localhost:7187/api/Exam_Room_Package/Pus/{packageTestADO.Exam_Room_Package_Id}", examRoomPackageModel);

                if (!examRoomPackage.IsSuccessStatusCode)
                {
                    ErrorMes = "Cập nhập gói đề thất bại";
                    return false;
                }

                List<Data_Base.Models.E.Exam_Room_Teacher> CurrExamRoomTeacher = new List<Exam_Room_Teacher>();

                Data_Base.Models.E.Exam_Room_Teacher examRoomTeacherModel1 = new Exam_Room_Teacher();
                examRoomTeacherModel1.Id = packageTestADO.Exam_Room_Teacher1_Id;
                examRoomTeacherModel1.Teacher_Id = packageTestADO.GV1_Id;
                examRoomTeacherModel1.Exam_Room_Id = packageTestADO.Exam_Room_Id;
                CurrExamRoomTeacher.Add(examRoomTeacherModel1);

                Data_Base.Models.E.Exam_Room_Teacher examRoomTeacherModel2 = new Exam_Room_Teacher();
                examRoomTeacherModel2.Id = packageTestADO.Exam_Room_Teacher2_Id;
                examRoomTeacherModel2.Teacher_Id = packageTestADO.GV2_Id;
                examRoomTeacherModel2.Exam_Room_Id = packageTestADO.Exam_Room_Id;
                CurrExamRoomTeacher.Add(examRoomTeacherModel2);

                var examRoomTeacher = await _httpClient.PutAsJsonAsync("https://localhost:7187/api/Exam_Room_Teacher/PusList", CurrExamRoomTeacher);
                if (!examRoomTeacher.IsSuccessStatusCode)
                {
                    ErrorMes = "Cập nhập giảng viên coi thi thất bại";
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                ErrorMes = ex.Message;
                return false;
            }
        }

        public async Task<List<PackageTypeViewModel>> GetPackageType()
        {
            var lstPackageType = await _httpClient.GetFromJsonAsync<List<Package_Type>>("https://localhost:7187/api/Package_Type/Get");

            var PackageType = (from pt in lstPackageType
                            select new PackageTypeViewModel
                            {
                                Package_Type_Id = pt.Id,
                                Package_Type_Name = pt.Package_Type_Name,
                            }).ToList();

            return PackageType;
        }

        public async Task<List<SubjectViewModel>> GetSubject() 
        {
            var lstsubjects = await _httpClient.GetFromJsonAsync<List<Subject>>("https://localhost:7187/api/Subject/Get");

            var subjects = (from sub in lstsubjects
                            select new SubjectViewModel
                            {
                                Subject_Id = sub.Id,
                                Subject_Name = sub.Subject_Name
                            }).ToList();

            return subjects;
        }

        public SubjectViewModel GetSubjectOfExam(int subjectId)
        {
            var lstsubjects = _httpClient.GetFromJsonAsync<List<Subject>>("https://localhost:7187/api/Subject/Get").GetAwaiter().GetResult();

            var subjects = (from sub in lstsubjects
                            where sub.Id == subjectId
                            select new SubjectViewModel
                            {
                                Subject_Id = sub.Id,
                                Subject_Name = sub.Subject_Name
                            }).ToList().SingleOrDefault();

            return subjects;
        }

        public async Task<List<ClassViewModel>> GetClasses()
        {
            var lstclass = await _httpClient.GetFromJsonAsync<List<Class>>("https://localhost:7187/api/Class/Get");

            var classes = (from cla in lstclass
                           select new ClassViewModel
                           {
                               Class_Id = cla.Id,
                               Class_Name = cla.Class_Name,
                               Homeroom_Teacher = cla.Teacher_Id
                           }).ToList();

            return classes;
        }

        public async Task<List<PointTypeViewModel>> GetPointType()
        {
            var lstPointType = await _httpClient.GetFromJsonAsync<List<Point_Type>>("https://localhost:7187/api/Point_Type/Get");

            var PointType = (from lpt in lstPointType
                             select new PointTypeViewModel
                             {
                                 PointType_Id = lpt.Id,
                                 PointType_Name = lpt.Point_Type_Name
                             }).ToList();

            return PointType;

        }

        public async Task<List<ExamsViewModel>> GetExam()
        {
            var lstExam = await _httpClient.GetFromJsonAsync<List<Data_Base.Models.E.Exam>>("https://localhost:7187/api/Exam/Get");

            var exams = (from exam in lstExam
                             select new ExamsViewModel
                             {
                                 Exams_Id = exam.Id,
                                 Exams_Name = exam.Exam_Name,
                                 Subject_Id = exam.Subject_Id,
                             }).ToList();

            return exams;
        }

        public async Task<List<RoomViewModel>> GetRoom()
        {
            var lstRoom = await _httpClient.GetFromJsonAsync<List<Room>>("https://localhost:7187/api/Room/Get");

            var room = (from r in lstRoom
                         select new RoomViewModel
                         {
                             Room_Id = r.ID,
                             Room_Name = r.Room_Name
                         }).ToList();

            return room;
        }

        public class PackageTestADO
        {
            public Data_Base.Models.P.Package Package { get; set; }
            public Exam_Room Exam_Room { get; set; }
            public Exam_Room_Package Exam_Room_Package { get; set; }
            public List<Exam_Room_Teacher> lstExamRoomTeacher { get; set; }
            public List<int> lstTeacherId { get; set; }

        }

        public class PackageTypeViewModel
        {
            public int Package_Type_Id { get; set; }
            public string Package_Type_Name { get; set; }
        }

        public class SubjectViewModel
        {
            public int Subject_Id { get; set; }
            public string Subject_Name { get; set; }
        }

        public class ClassViewModel
        {
            public int Class_Id { get; set; }
            public string Class_Name { get; set; }
            public int Homeroom_Teacher { get; set; }
        }
        
        public class PointTypeViewModel
        {
            public int PointType_Id { get; set; }
            public string PointType_Name { get; set; }
        }
        
        public class ExamsViewModel
        {
            public int Exams_Id { get; set; }
            public string Exams_Name { get; set; }
            public int Subject_Id { get; set; }
        }

        public class RoomViewModel
        {
            public int Room_Id { get; set; }
            public string Room_Name { get; set; }
        }
    }
}
