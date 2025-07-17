using Blazor_Server.Pages;
using Data_Base.Filters;
using Data_Base.GenericRepositories;
using Data_Base.Models.A;
using Data_Base.Models.C;
using Data_Base.Models.E;
using Data_Base.Models.P;
using Data_Base.Models.Q;
using Data_Base.Models.S;
using Data_Base.Models.T;
using Data_Base.Models.U;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using static Blazor_Server.Services.ExammanagementService;
using static Blazor_Server.Services.HistoriesExam;
using static Blazor_Server.Services.Package_Test_ERP;
using static Blazor_Server.Services.PackageManager;
using static Blazor_Server.Services.Test;
using static System.Net.WebRequestMethods;
using Question = Data_Base.Models.Q.Question;

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
            var lstPackage = await _httpClient.GetFromJsonAsync<List<Data_Base.Models.P.Package>>("https://localhost:7187/api/Package/Get");
            if (lstPackage == null || lstPackage.Count == 0)
                return new List<PackageInactive>();

            var lstPackageType = await _httpClient.GetFromJsonAsync<List<Package_Type>>("https://localhost:7187/api/Package_Type/Get");
            var lstClass = await _httpClient.GetFromJsonAsync<List<Class>>("https://localhost:7187/api/Class/Get");
            var lstSubject = await _httpClient.GetFromJsonAsync<List<Subject>>("https://localhost:7187/api/Subject/Get");
            var lstExamRoomPackage = await _httpClient.GetFromJsonAsync<List<Exam_Room_Package>>("https://localhost:7187/api/Exam_Room_Package/Get");
            var lstExamRoom = await _httpClient.GetFromJsonAsync<List<Exam_Room>>("https://localhost:7187/api/Exam_Room/Get");
            var lstExamRoomTeacher = await _httpClient.GetFromJsonAsync<List<Exam_Room_Teacher>>("https://localhost:7187/api/Exam_Room_Teacher/Get");
            var lstTeacher = await _httpClient.GetFromJsonAsync<List<Teacher>>("https://localhost:7187/api/Teacher/Get");
            var lstUser = await _httpClient.GetFromJsonAsync<List<User>>("https://localhost:7187/api/User/Get");

            // Dictionary
            var subjectDict = lstSubject.ToDictionary(s => s.Id);
            var classDict = lstClass.ToDictionary(c => c.Id);
            var PackageTypeDict = lstPackageType.ToDictionary(c => c.Id);
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
                    var teacherClassId = lstClass.FirstOrDefault(tc => tc.Id == p.Class_Id)?.Teacher_Id;
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
                        PackageTypeId = p.Package_Type_Id,
                        SubjectId = p.Subject_Id,
                        ClassId = p.Class_Id,
                        PackageTypeName = PackageTypeDict.TryGetValue(p.Package_Type_Id, out var pt) ? pt.Package_Type_Name : "N/A",
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
        public async Task<List<HistDTO>> GetQuesTL(int subjectId, int packageTypeId, int classId)
        {
            try
            {
                var Class = await _httpClient.GetFromJsonAsync<Data_Base.Models.C.Class>($"https://localhost:7187/api/Class/GetBy/{classId}");
                var lstClass = await _httpClient.GetFromJsonAsync<List<Data_Base.Models.C.Class>>("https://localhost:7187/api/Class/Get");
                lstClass = lstClass.Where(o => o.Grade_Id == Class.Grade_Id).ToList();
                var filterRequest = new CommonFilterRequest
                {
                    Filters = new Dictionary<string, string>
                    {
                        { "Subject_Id", subjectId.ToString() },
                        { "Package_Type_Id", packageTypeId.ToString() }  
                    },
                };

                var packageGetResponse = await _httpClient.PostAsJsonAsync("https://localhost:7187/api/Package/common/get", filterRequest);
                if (!packageGetResponse.IsSuccessStatusCode)
                    return null;

                var lstpackage = await packageGetResponse.Content.ReadFromJsonAsync<List<Data_Base.Models.P.Package>>();
                var lstpackageId = lstpackage.Where(o => lstClass.Select(o => o.Id).ToList().Contains(o.Class_Id)).Select(o => o.Id).ToList();

                var filterRequestQues = new CommonFilterRequest
                {
                    Filters = new Dictionary<string, string>
                    {
                        { "Package_Id", string.Join(",", lstpackageId) },
                    },
                };

                var questionGetResponse = await _httpClient.PostAsJsonAsync("https://localhost:7187/api/Question/common/get", filterRequestQues);
                if (!questionGetResponse.IsSuccessStatusCode)
                    return null;

                var questionList = await questionGetResponse.Content.ReadFromJsonAsync<List<Data_Base.Models.Q.Question>>();

                var result = new List<HistDTO>();

                if (packageTypeId == 2)
                {
                    List<int> quesId = questionList.Select(o => o.Id).ToList();

                    var filterAns = new CommonFilterRequest
                    {
                        Filters = new Dictionary<string, string>
                        {
                            { "Question_Id", string.Join(",", quesId) },
                        },
                    };

                    var ansGetResponse = await _httpClient.PostAsJsonAsync("https://localhost:7187/api/Answers/common/get", filterAns);
                    if (!ansGetResponse.IsSuccessStatusCode)
                        return null;

                    var ansList = await ansGetResponse.Content.ReadFromJsonAsync<List<Data_Base.Models.A.Answers>>();

                    foreach (var item in lstpackage)
                    {
                        var questions = questionList.Where(q => q.Package_Id == item.Id).Select(q => new ListQuesAns
                        {
                            QuestionId = q.Id,
                            PackageId = q.Package_Id,
                            QuestionName = q.Question_Name,
                            QuestionTypeId = q.Question_Type_Id,
                            Leva = q.Question_Level_Id,
                            Answers = ansList.Where(o => o.Question_Id == q.Id).Select(ans => new Answer
                            {
                                AnswerId = ans.Id,
                                AnswersName = ans.Answers_Name,
                                Right_Answer = ans.Right_Answer,
                                QuestionId = q.Id
                            } ).ToList()
                        }).ToList();

                        if (questions.Any())
                        {
                            var hist = new HistDTO
                            {
                                PackageId = item.Id,
                                Package_Name = item.Package_Name,
                                Create_Time = item.Create_Time,
                                PackageTypeId = item.Package_Type_Id,
                                PointTypeId = item.Point_Type_Id,
                                Questions = questions
                            };

                            result.Add(hist);
                        }
                    }
                }
                else if(packageTypeId == 1)
                {
                    foreach (var item in lstpackage)
                    {
                        var questions = questionList.Where(q => q.Package_Id == item.Id).Select(q => new ListQuesAns
                        {
                            QuestionId = q.Id,
                            PackageId = q.Package_Id,
                            QuestionName = q.Question_Name,
                            QuestionTypeId = q.Question_Type_Id,
                            Leva = q.Question_Level_Id
                        }).ToList();

                        if (questions.Any())
                        {
                            var hist = new HistDTO
                            {
                                PackageId = item.Id,
                                Package_Name = item.Package_Name,
                                Create_Time = item.Create_Time,
                                PackageTypeId = item.Package_Type_Id,
                                PointTypeId = item.Point_Type_Id,
                                Questions = questions
                            };

                            result.Add(hist);
                        }
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<bool> CreateQuestionTL(QuestionAdo model)
        {
            try
            {
                var questionCreate = await _httpClient.PostAsJsonAsync("https://localhost:7187/api/Question/Post", model.question);
                if (!questionCreate.IsSuccessStatusCode)
                {
                    var errorContent = await questionCreate.Content.ReadAsStringAsync();
                    Console.WriteLine("Lỗi khi gọi API Package/Post:");
                    Console.WriteLine(errorContent);
                    return false;
                }
                    

                var addQuestion = await questionCreate.Content.ReadFromJsonAsync<Data_Base.Models.Q.Question>();

                var filterRequest = new CommonFilterRequest
                {
                    Filters = new Dictionary<string, string>
                    {
                        { "Package_Id", model.question.Package_Id.ToString() }
                    },
                    Entity = model.question
                };

                var questionGetResponse = await _httpClient.PostAsJsonAsync("https://localhost:7187/api/Question/common/get", filterRequest);

                if (questionGetResponse.IsSuccessStatusCode)
                {
                    var questionList = await questionGetResponse.Content.ReadFromJsonAsync<List<Data_Base.Models.Q.Question>>();

                    if (questionList != null && questionList.Any())
                    {
                        var totalMaxScore = questionList.Sum(q => q.Maximum_Score);
                        if (totalMaxScore > 10) 
                        {
                            if (addQuestion != null)
                            {
                                var deleteResponse = await _httpClient.DeleteAsync($"https://localhost:7187/api/Question/Delete/{addQuestion.Id}");
                                if (deleteResponse.IsSuccessStatusCode)
                                {
                                    Console.WriteLine($"Không thể tạo câu hỏi : {addQuestion.Question_Name} vì tổng điểm vượt quá 10");
                                    return false;
                                }
                            }
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public async Task<bool> CreateListQuestionTL(List<int> QuesId, int packageId, int packageTypeId)
        {
            try
            {
                if (QuesId == null || QuesId.Count == 0)
                    return false;

                // Bước 1: Lấy danh sách câu hỏi theo ID
                var filter = new CommonFilterRequest
                {
                    Filters = new Dictionary<string, string> { { "Id", string.Join(",", QuesId) } }
                };

                var questionGetResponse = await _httpClient.PostAsJsonAsync("https://localhost:7187/api/Question/common/get", filter);
                if (!questionGetResponse.IsSuccessStatusCode) return false;

                var questionList = await questionGetResponse.Content.ReadFromJsonAsync<List<Data_Base.Models.Q.Question>>();
                var lstquestions = new List<Question>();

                // Bước 2: Tạo danh sách câu hỏi mới (bản sao)
                foreach (var item in questionList)
                {
                    lstquestions.Add(new Question
                    {
                        Question_Level_Id = item.Question_Level_Id,
                        Question_Type_Id = item.Question_Type_Id,
                        Question_Name = item.Question_Name,
                        Maximum_Score = item.Maximum_Score ?? 0,
                        Package_Id = packageId
                    });
                }

                // Bước 3: Gửi danh sách câu hỏi mới lên server
                var repQues = await _httpClient.PostAsJsonAsync("https://localhost:7187/api/Question/PostList", lstquestions);
                if (!repQues.IsSuccessStatusCode) return false;

                // Bước 4: Đọc danh sách câu hỏi mới đã lưu
                var lstQuesNew = await repQues.Content.ReadFromJsonAsync<List<Data_Base.Models.Q.Question>>();

                // Bước 5: Nếu là loại có đáp án (packageTypeId == 2) thì xử lý thêm đáp án
                if (packageTypeId == 2 && lstQuesNew != null)
                {
                    // Lấy danh sách đáp án của các câu hỏi cũ
                    var answerFilter = new CommonFilterRequest
                    {
                        Filters = new Dictionary<string, string> { { "Id", string.Join(",", QuesId) } }
                    };

                    var ansGetResponse = await _httpClient.PostAsJsonAsync("https://localhost:7187/api/Answers/common/get", answerFilter);
                    if (!ansGetResponse.IsSuccessStatusCode) return false;

                    var lstOldAnswers = await ansGetResponse.Content.ReadFromJsonAsync<List<Data_Base.Models.A.Answers>>();

                    // Ánh xạ câu hỏi cũ - mới theo thứ tự
                    for (int i = 0; i < questionList.Count; i++)
                    {
                        var oldQuestion = questionList[i];
                        var newQuestion = lstQuesNew[i];

                        // Lấy đáp án theo ID câu hỏi cũ
                        var answersForOldQ = lstOldAnswers.Where(a => a.Question_Id == oldQuestion.Id).ToList();

                        var newAnswers = answersForOldQ.Select(a => new Answers
                        {
                            Question_Id = newQuestion.Id,
                            Answers_Name = a.Answers_Name,
                            Right_Answer = a.Right_Answer
                        }).ToList();

                        // Gửi danh sách đáp án mới lên server
                        var postAnsResponse = await _httpClient.PostAsJsonAsync("https://localhost:7187/api/Answers/PostList", newAnswers);
                        if (!postAnsResponse.IsSuccessStatusCode) return false;
                    }
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<bool> CreatequesTN(QuestionAdo quesAdo, List<AnsAdo> ansAdo)
        {
            try
            {
                List<Data_Base.Models.A.Answers> lstAnswers = new List<Data_Base.Models.A.Answers>();
                if (quesAdo != null && ansAdo != null)
                {
                    var questionCreate = await _httpClient.PostAsJsonAsync("https://localhost:7187/api/Question/Post", quesAdo.question);
                    if (!questionCreate.IsSuccessStatusCode)
                    {
                        var errorContent = await questionCreate.Content.ReadAsStringAsync();
                        Console.WriteLine("Lỗi khi gọi API Package/Post:");
                        Console.WriteLine(errorContent);
                        return false;
                    }

                    var addQuestion = await questionCreate.Content.ReadFromJsonAsync<Data_Base.Models.Q.Question>();

                    
                    foreach (var item in ansAdo)
                    {
                        Data_Base.Models.A.Answers answers = new Data_Base.Models.A.Answers();
                        answers.Answers_Name = item.Name;
                        answers.Right_Answer = item.Right ? 1 : 0;
                        answers.Question_Id = addQuestion.Id;

                        lstAnswers.Add(answers);
                    }

                    if (lstAnswers == null && lstAnswers.Count <= 0)
                        return false;

                    await _httpClient.PostAsJsonAsync("https://localhost:7187/api/Answers/PostList", lstAnswers);

                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public async Task<List<QuestionTypeViewModel>> GetQuestionType(int idPackageType)
        {
            var lstquestionType = await _httpClient.GetFromJsonAsync<List<Question_Type>>("https://localhost:7187/api/Question_Type/Get");

            lstquestionType = lstquestionType.Where(o => o.Package_Type_Id == idPackageType).ToList();

            var questionType = (from qt in lstquestionType
                                select new QuestionTypeViewModel
                                {
                                    Question_Type_Id = qt.Id,
                                    Question_Type_Name = qt.Question_Type_Name
                                }).ToList();

            return questionType;
        }
        public async Task<List<QuestionlevelViewModel>> GetQuestionLevel()
        {
            var lstquestionType = await _httpClient.GetFromJsonAsync<List<Question_Level>>("https://localhost:7187/api/Question_Level/Get");

            var questionLevel = (from qt in lstquestionType
                                 select new QuestionlevelViewModel
                                 {
                                     Question_Level_Id = qt.Id,
                                     Question_Level_Name = qt.Question_Level_Name
                                 }).ToList();

            return questionLevel;
        }
        public async Task<bool> CreateExcelEssay(MultipartFormDataContent content, int packageId)
        {
            try
            {
                var response = await _httpClient.PostAsync($"/api/Excel_/exceltuluan?packageId={packageId}", content);

                if (response.IsSuccessStatusCode)
                {
                    var msg = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("✅ Thành công: " + msg);
                    return true;
                }
                else
                {
                    var err = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("❌ Thất bại: " + err);
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public async Task<bool> CreateExcel(MultipartFormDataContent content, int packageId)
        {
            try
            {
                var response = await _httpClient.PostAsync($"https://localhost:7187/api/Excel_/excel?packageId={packageId}", content);

                if (response.IsSuccessStatusCode)
                {
                    var msg = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("✅ Thành công: " + msg);
                    return true;
                }
                else
                {
                    var err = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("❌ Thất bại: " + err);
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public async Task <List<ListQuesAns>> GetQuestyonNew(int packageId, int packageTypeId)
        {
            try
            {
                if (packageId <= 0) 
                {
                    return null;
                }

                var filterRequestQues = new CommonFilterRequest
                {
                    Filters = new Dictionary<string, string>
                    {
                        { "Package_Id", packageId.ToString() },
                    },
                };

                var quesrepo = await _httpClient.PostAsJsonAsync("https://localhost:7187/api/Question/common/get", filterRequestQues);

                if (!quesrepo.IsSuccessStatusCode)
                {
                    return null;
                }

                var lstQuestyon = await quesrepo.Content.ReadFromJsonAsync<List<Data_Base.Models.Q.Question>>();

                var result = new List<ListQuesAns>();

                if (packageTypeId == 2)
                {
                    List<int> lstQuestyonId = lstQuestyon.Select(o => o.Id).ToList();

                    var filterAns = new CommonFilterRequest
                    {
                        Filters = new Dictionary<string, string>
                        {
                            { "Question_Id", string.Join(",", lstQuestyonId) },
                        },
                    };

                    var ansGetResponse = await _httpClient.PostAsJsonAsync("https://localhost:7187/api/Answers/common/get", filterAns);
                    if (!ansGetResponse.IsSuccessStatusCode)
                        return null;

                    var ansList = await ansGetResponse.Content.ReadFromJsonAsync<List<Data_Base.Models.A.Answers>>();

                    foreach (var item in lstQuestyon)
                    {
                        ListQuesAns que = new ListQuesAns();
                        que.QuestionId = item.Id;
                        que.QuestionTypeId = item.Question_Type_Id;
                        que.QuestionName = item.Question_Name;
                        que.PackageId = item.Package_Id;
                        //que.Type = item.Question_Type_Id;
                        que.Leva = item.Question_Level_Id;
                        que.Answers = ansList.Where(o => o.Question_Id == item.Id).Select(ans => new Answer
                        {
                            AnswerId = ans.Id,
                            AnswersName = ans.Answers_Name,
                            Right_Answer = ans.Right_Answer,
                            QuestionId = item.Id

                        }).ToList();

                        result.Add(que);
                    }
                }
                else if (packageTypeId == 1)
                {
                    foreach (var item in lstQuestyon)
                    {
                        ListQuesAns que = new ListQuesAns();
                        que.QuestionId = item.Id;
                        que.QuestionName = item.Question_Name;
                        que.PackageId = item.Package_Id;
                        que.QuestionTypeId = item.Question_Type_Id;
                        que.Leva = item.Question_Level_Id;

                        result.Add(que);
                    }
                }
                return result;

            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<bool> DeleteQuestion(int questionId, int? questionTypeId)
        {
            try
            {
                if (questionId <= 0 && questionTypeId <= 0)
                {
                    return false;
                }

                if (questionTypeId == 1 || questionTypeId == 2 || questionTypeId == 3)
                {
                    var filter = new CommonFilterRequest
                    {
                        Filters = new Dictionary<string, string> { { "Question_Id", questionId.ToString() } }
                    };

                    var ansRep = await _httpClient.PostAsJsonAsync("https://localhost:7187/api/Answers/common/get", filter);

                    if (!ansRep.IsSuccessStatusCode)
                    {
                        return false;
                    }

                    List<int> lstAnsId = (await ansRep.Content.ReadFromJsonAsync<List<Data_Base.Models.A.Answers>>()).Select(o => o.Id).ToList();

                    var request = new HttpRequestMessage
                    {
                        Method = HttpMethod.Delete,
                        RequestUri = new Uri("https://localhost:7187/api/Answers/DeleteLst"),
                        Content = new StringContent(JsonSerializer.Serialize(lstAnsId), Encoding.UTF8, "application/json")
                    };

                    var response = await _httpClient.SendAsync(request);

                    if (!response.IsSuccessStatusCode)
                    {
                        return false;
                    }
                }
                
                var ques = await _httpClient.DeleteAsync($"https://localhost:7187/api/Question/Delete/{questionId}");
                if (!ques.IsSuccessStatusCode)
                {
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> UpdateQuestionAns(ListQuesAns ado)
        {
            try
            {
                if (ado == null)
                {
                    return false;
                }

                var questionToUpdate = new Question
                {
                    Id = ado.QuestionId,
                    Question_Name = ado.QuestionName,
                    Question_Type_Id = ado.QuestionTypeId ?? 0,
                    Question_Level_Id = ado.Leva,
                    Package_Id = ado.PackageId,
                };

                var questionResponse = await _httpClient.PutAsJsonAsync($"https://localhost:7187/api/Question/Pus/{ado.QuestionId}", questionToUpdate);

                if (questionResponse.IsSuccessStatusCode)
                {
                    // Update từng answer
                    foreach (var answer in ado.Answers)
                    {
                        var answerToUpdate = new Data_Base.Models.A.Answers
                        {
                            Id = answer.AnswerId,
                            Answers_Name = answer.AnswersName,
                            Right_Answer = answer.Right_Answer,
                            Question_Id = ado.QuestionId // Đảm bảo có QuestionId nếu cần
                        };

                        var answerResponse = await _httpClient.PutAsJsonAsync($"https://localhost:7187/api/Answers/Pus/{answer.AnswerId}", answerToUpdate);

                        if (!answerResponse.IsSuccessStatusCode)
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public class AnsAdo
        {
            public string Name { get; set; }
            public bool Right { get; set; }
        }
        public class HistDTO
        {
            public int PackageId { get; set; }
            public string Package_Name { get; set; }
            public int PackageTypeId { get; set; }
            public int PointTypeId { get; set; }
            public long Create_Time { get; set; }
            public List<ListQuesAns> Questions { get; set; }
        }
        public class ListQuesAns
        {
            public int QuestionId { get; set; }
            public int? QuestionTypeId { get; set; }
            public int PackageId { get; set; }
            public string QuestionName { get; set; }
            public double? MaximumScore { get; set; }
            public int Leva { get; set; }
            public List<Answer>? Answers { get; set; }
        }
        public class Answer
        {
            public int AnswerId { get; set; }
            public string AnswersName { get; set; }
            public int? Right_Answer { get; set; }
            public int? QuestionId { get; set; }
            public bool IsCorrect
            {
                get => Right_Answer == 1;
                set => Right_Answer = value ? 1 : 0;
            }
        }
        public class QuestionAdo
        {
            public Data_Base.Models.Q.Question question { get; set; }
        }
        public class QuestionTypeViewModel
        {
            public int Question_Type_Id { get; set; }
            public string Question_Type_Name { get; set; }
        }
        public class QuestionlevelViewModel
        {
            public int Question_Level_Id { get; set; }
            public string Question_Level_Name { get; set; }
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
            public int PackageTypeId { get; set; }
            public string PackageTypeName { get; set; }
            public int Code { get; set; }
            public int SubjectId { get; set; }
            public string SubjectName { get; set; }
            public int ClassId { get; set; }
            public string ClassName { get; set; }
            public string TeacherClass { get; set; }
            public string TeacherExamRoom { get; set; }
            public int ClassNub { get; set; }
            public long StartTime { get; set; }
            public long EndTime { get; set; }
        }
    }
}
