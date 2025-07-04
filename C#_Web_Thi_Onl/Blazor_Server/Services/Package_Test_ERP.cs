﻿using Data_Base.Filters;
using Data_Base.Models.C;
using Data_Base.Models.E;
using Data_Base.Models.P;
using Data_Base.Models.Q;
using Data_Base.Models.R;
using Data_Base.Models.S;
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

        public async Task<bool> CheckPackage(PackageTestADO model)
        {
            bool success = true;
            try
            {
                var filterRequest = new CommonFilterRequest
                {
                    Filters = new Dictionary<string, string>
                    {
                        { "Room_ID", model.Exam_Room.Room_Id.ToString() },
                        { "Start_Time",  model.Package.Create_Time.ToString()}
                    },
                };
                   
                var ExamRoomResponse = await _httpClient.PostAsJsonAsync("https://localhost:7187/api/Exam_Room/common/get", filterRequest);
                if (!ExamRoomResponse.IsSuccessStatusCode)
                    return false;

                var lstExamRoom = await ExamRoomResponse.Content.ReadFromJsonAsync<List<Exam_Room>>();

                if(lstExamRoom == null && lstExamRoom.Count <= 0)
                    return false;

                if (lstExamRoom != null && lstExamRoom.Count > 0)
                {
                    foreach (var item in lstExamRoom)
                    {
                        if ((model.Exam_Room.Start_Time < item.Start_Time && model.Exam_Room.End_Time < item.Start_Time)
                            || model.Exam_Room.Start_Time > item.Start_Time && model.Exam_Room.End_Time > item.Start_Time)
                        {
                            success = true;

                        }
                        else
                        {
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

            bool check = await CheckPackage(model);

            if (!check)
                return null;

            var model_Package = await _httpClient.PostAsJsonAsync("https://localhost:7187/api/Package/Post", model.Package);
            if (!model_Package.IsSuccessStatusCode)
            {
                var errorContent = await model_Package.Content.ReadAsStringAsync();
                Console.WriteLine("Lỗi khi gọi API Package/Post:");
                Console.WriteLine(errorContent);
                return null;
            }

            var addedPackage = await model_Package.Content.ReadFromJsonAsync<Package>();
            if (addedPackage == null) return null;

            var model_Exam_Room = await _httpClient.PostAsJsonAsync("https://localhost:7187/api/Exam_Room/Post", model.Exam_Room);
            if (!model_Exam_Room.IsSuccessStatusCode)
                return null;

            var addedExam_Room = await model_Exam_Room.Content.ReadFromJsonAsync<Exam_Room>();
            if (addedExam_Room == null) return null;

            model.Exam_Room_Package.Exam_Room_Id = addedExam_Room.Id;
            model.Exam_Room_Package.Package_Id = addedPackage.Id;

            model.Exam_Room_Teacher.Exam_Room_Id = addedExam_Room.Id;

            var model_Exam_Room_Package = await _httpClient.PostAsJsonAsync("https://localhost:7187/api/Exam_Room_Package/Post", model.Exam_Room_Package);

            if (!model_Exam_Room_Package.IsSuccessStatusCode)
                return null;

            var model_Exam_Room_Teacher = await _httpClient.PostAsJsonAsync("https://localhost:7187/api/Exam_Room_Teacher/Post", model.Exam_Room_Teacher);
            if (!model_Exam_Room_Teacher.IsSuccessStatusCode)
            {
                var errorContent = await model_Exam_Room_Teacher.Content.ReadAsStringAsync();
                Console.WriteLine("Lỗi khi gọi API Package/Post:");
                Console.WriteLine(errorContent);
                return null;
            }

            return new PackageTestADO
            {
                Package = addedPackage,
                Exam_Room = addedExam_Room,
                Exam_Room_Package = model.Exam_Room_Package,
                Exam_Room_Teacher = model.Exam_Room_Teacher,
            };
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

        public async Task<List<ClassViewModel>> GetClasses()
        {
            var lstclass = await _httpClient.GetFromJsonAsync<List<Class>>("https://localhost:7187/api/Class/Get");

            var classes = (from cla in lstclass
                           select new ClassViewModel
                           {
                               Class_Id = cla.Id,
                               Class_Name = cla.Class_Name
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
            var lstExam = await _httpClient.GetFromJsonAsync<List<Exam>>("https://localhost:7187/api/Exam/Get");

            var exams = (from exam in lstExam
                             select new ExamsViewModel
                             {
                                 Exams_Id = exam.Id,
                                 Exams_Name = exam.Exam_Name
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
            public Package Package { get; set; }
            public Exam_Room Exam_Room { get; set; }
            public Exam_Room_Package Exam_Room_Package { get; set; }
            public Exam_Room_Teacher Exam_Room_Teacher { get; set; }

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
        }

        public class RoomViewModel
        {
            public int Room_Id { get; set; }
            public string Room_Name { get; set; }
        }
    }
}
