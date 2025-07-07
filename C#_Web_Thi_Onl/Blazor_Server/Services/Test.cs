using Blazor_Server.Pages;
using Data_Base.Filters;
using Data_Base.GenericRepositories;
using Data_Base.Models.A;
using Data_Base.Models.E;
using Data_Base.Models.P;
using Data_Base.Models.Q;
using Data_Base.Models.S;
using Data_Base.Models.T;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Engineering;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using static Blazor_Server.Services.HistoriesExam;

namespace Blazor_Server.Services
{
    public class Test
    {
        private readonly HttpClient _httpClient;

        public Test(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<HistDTO> GetQuestionAnswers(int Package_Code, string Student_Code)
        {
            HistDTO histDTO = new HistDTO();
            bool IsCheckType = true;
            try
            {

                if (Package_Code == null)
                {
                    return null;
                }

                #region vế 1: lấy test
                var filterRequestPackage = new CommonFilterRequest
                {
                    Filters = new Dictionary<string, string>
                    {
                        { "Package_Code", Package_Code.ToString() }
                    },
                };
                var packageGetResponse = await _httpClient.PostAsJsonAsync("https://localhost:7187/api/V_Package/common/get", filterRequestPackage);

                if (!packageGetResponse.IsSuccessStatusCode)
                    return null;

                var Vpackage = (await packageGetResponse.Content.ReadFromJsonAsync<List<Data_Base.V_Model.V_Package>>()).SingleOrDefault();

                if (Vpackage.Package_Type_Id == 2)
                {
                    IsCheckType = true;
                }
                else
                {
                    IsCheckType = false;
                }

                if (Vpackage == null)
                    return null;

                var filterRequestStudent = new CommonFilterRequest
                {
                    Filters = new Dictionary<string, string>
                    {
                        { "Student_Code", Student_Code.ToString() }
                    },
                };
                var studentGetResponse = await _httpClient.PostAsJsonAsync("https://localhost:7187/api/V_Student/common/get", filterRequestStudent);

                if (!studentGetResponse.IsSuccessStatusCode)
                    return null;

                var student = (await studentGetResponse.Content.ReadFromJsonAsync<List<Data_Base.V_Model.V_Student>>()).SingleOrDefault();

                if (student == null)
                    return null;

                var filterRequestTest = new CommonFilterRequest
                {
                    Filters = new Dictionary<string, string>
                    {
                        { "Package_Id", Vpackage.Id.ToString() },
                        { "Student_Id", student.Id.ToString() }
                    },
                };
                var lstTestsReq = await _httpClient.PostAsJsonAsync("https://localhost:7187/api/Test/common/get", filterRequestTest);

                var test = (await lstTestsReq.Content.ReadFromJsonAsync<List<Data_Base.Models.T.Test>>()).SingleOrDefault();

                #endregion

                #region vế 2: lấy Questions và Answers
                var filterRequestQuestion = new CommonFilterRequest
                {
                    Filters = new Dictionary<string, string>
                    {
                        { "Package_Id", Vpackage.Id.ToString() }
                    },
                };

                var questionsReq = await _httpClient.PostAsJsonAsync("https://localhost:7187/api/Question/common/get", filterRequestQuestion);

                var lstQuestions = await questionsReq.Content.ReadFromJsonAsync<List<Data_Base.Models.Q.Question>>();
                Exam_Room_Student ers = new Exam_Room_Student();
                if (lstQuestions == null)
                {
                    var filter = new CommonFilterRequest
                    {
                        Filters = new Dictionary<string, string>
                        {
                            { "Test_Id", test.Id.ToString() },
                            { "Student_Id", student.Id.ToString() }
                        },
                    };

                    var rep = await _httpClient.PostAsJsonAsync("https://localhost:7187/api/Exam_Room_Student/common/get", filter);

                    ers = (await rep.Content.ReadFromJsonAsync<List<Exam_Room_Student>>()).SingleOrDefault();
                    await _httpClient.DeleteAsync($"https://localhost:7187/api/Exam_Room_Student/Delete/{ers.Id}");
                    await _httpClient.DeleteAsync($"https://localhost:7187/api/Test/Delete/{test.Id}");
                    return null;
                }

                var randomQuestionIds = lstQuestions.OrderBy(id => Guid.NewGuid()) // trộn ngẫu nhiên danh sách ID
                                        .Take(Vpackage.Number_Of_Questions) // lấy đúng số lượng mong muốn
                                        .ToList();

                List<Data_Base.Models.T.Test_Question> lstTq = new List<Test_Question>();

                foreach (var question in randomQuestionIds)
                {
                    Test_Question tq = new Test_Question();
                    tq.Question_Id = question.Id;
                    tq.Test_Id = test.Id;

                    lstTq.Add(tq);
                }

                List<Test_Question> newLstTq = new List<Test_Question>();
                if (lstTq != null && lstTq.Count > 0)
                {
                     var post = await _httpClient.PostAsJsonAsync("https://localhost:7187/api/Test_Question/PostList", lstTq);

                    newLstTq = await post.Content.ReadFromJsonAsync<List<Test_Question>>();
                }

                #endregion

                #region vế 3: lấy ra bài thì câu hỏi và đáp án tương ứng
                if (IsCheckType)
                {
                    var filterRequestAnswers = new CommonFilterRequest
                    {
                        Filters = new Dictionary<string, string>
                        {
                            { "Question_Id", string.Join(",", randomQuestionIds.Select(o => o.Id)) }
                        },
                    };

                    var answersReq = await _httpClient.PostAsJsonAsync("https://localhost:7187/api/Answers/common/get", filterRequestAnswers);

                    if (answersReq == null)
                    {
                        await _httpClient.DeleteAsync($"https://localhost:7187/api/Test_Question/DeleteLst?{newLstTq.Select(o => o.Id).ToList()}");
                        await _httpClient.DeleteAsync($"https://localhost:7187/api/Exam_Room_Student/Delete/{ers.Id}");
                        await _httpClient.DeleteAsync($"https://localhost:7187/api/Test/Delete/{test.Id}");
                        return null;
                    }
                        

                    var lstAnswers = await answersReq.Content.ReadFromJsonAsync<List<Data_Base.Models.A.Answers>>();

                    histDTO = new HistDTO // package
                    {
                        PackageId = Vpackage.Id,
                        PackageName = Vpackage.Package_Name,
                        PackageTypeId = Vpackage.Package_Type_Id,
                        PackageType = Vpackage.Package_Type_Name,
                        ExecutionTime = Vpackage.ExecutionTime,
                        SubjectName = Vpackage.Subject_Name,
                        ClassName = Vpackage.Class_Name,
                        PointTypeId = Vpackage.Point_Type_Id,
                        TestId = test.Id,
                        TestCode = test.Test_Code,
                        Status = test.Status,
                        Questions = randomQuestionIds.Select(Que => new Question
                        {
                            QuestionId = Que.Id,
                            QuestionName = Que.Question_Name,
                            Type = Que.Question_Type_Id,
                            Level = Que.Question_Level_Id,
                            Answers = lstAnswers.Where(r => r.Question_Id == Que.Id).Select(Ans => new Answer
                            {
                                AnswerId = Ans.Id,
                                AnswersName = Ans.Answers_Name,
                                QuestionId = Que.Id
                            }).ToList(),
                        }).ToList()
                    };
                }
                else
                {
                    histDTO = new HistDTO // package
                    {
                        PackageId = Vpackage.Id,
                        PackageName = Vpackage.Package_Name,
                        PackageTypeId = Vpackage.Package_Type_Id,
                        PackageType = Vpackage.Package_Type_Name,
                        ExecutionTime = Vpackage.ExecutionTime,
                        SubjectName = Vpackage.Subject_Name,
                        ClassName = Vpackage.Class_Name,
                        PointTypeId = Vpackage.Point_Type_Id,
                        TestId = test.Id,
                        TestCode = test.Test_Code,
                        Status = test.Status,
                        Questions = randomQuestionIds.Select(Que => new Question
                        {
                            QuestionId = Que.Id,
                            QuestionName = Que.Question_Name,
                           // MaximumScore = Que.Maximum_Score,
                            Type = Que.Question_Type_Id,
                            Level = Que.Question_Level_Id,
                        }).ToList()
                    };
                }
                #endregion

                return histDTO;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        //tạo ko dùng
        public async Task<List<Exam_Room_Student_Answer_HisTory>> CreateHis(int Package_Code, int StudentId, int QuestIonId, List<int> AnswerIds)
        {
            if (Package_Code == 0 || AnswerIds == null || AnswerIds.Count == 0)
            {
                return null;
            }

            #region vế 1: lấy ExamRoomPackages và ExamRoomStudent
            var packages = await _httpClient.GetFromJsonAsync<List<Data_Base.Models.P.Package>>("https://localhost:7187/api/Package/Get");

            var packageId = packages.FirstOrDefault(o => o.Package_Code == Package_Code)?.Id;

            if (packageId == null)
            {
                return null;
            }

            var ExamRoomPackages = await _httpClient.GetFromJsonAsync<List<Exam_Room_Package>>("https://localhost:7187/api/Exam_Room_Package/Get");

            var ExamRoomPackageId = ExamRoomPackages.FirstOrDefault(o => o.Package_Id == packageId)?.Id;

            if (ExamRoomPackageId == null)
            {
                return null;
            }

            var ExamRoomStudents = await _httpClient.GetFromJsonAsync<List<Exam_Room_Student>>("https://localhost:7187/api/Exam_Room_Student/Get");

            var ExamRoomStudent = ExamRoomStudents.FirstOrDefault(o => o.Exam_Room_Package_Id == ExamRoomPackageId).Id;
            #endregion

            #region vế 2: khởi tạo và lưu đáp án đã chọn (Exam_Room_Student_Answer_HisTory(), xóa nếu đã có
            var AnsHislst = new List<Exam_Room_Student_Answer_HisTory>();

            var Answers = await _httpClient.GetFromJsonAsync<List<Data_Base.Models.A.Answers>>("https://localhost:7187/api/Exam_Room_Student/Get");

            var AnswersId = Answers.Where(o => o.Question_Id == QuestIonId).Select(x => x.Id).ToList();

            if (AnswersId == null)
            {
                return null;
            }

            var SAHs = await _httpClient.GetFromJsonAsync<List<Data_Base.Models.E.Exam_Room_Student_Answer_HisTory>>("https://localhost:7187/api/Exam_Room_Student/Get");

            var SAH = SAHs.Where(o => AnswersId.Contains(o.ID) && o.Exam_Room_Student_Id == ExamRoomStudent).ToList();
            if (SAH != null || SAH.Count > 0)
            {
                foreach (var item in SAH)
                {
                    await _httpClient.DeleteAsync($"https://localhost:7187/api/Exam_Room_Student_Answer_HisTory/Delete/{item.ID}");
                }
            }
            else
            {
                foreach (var answerId in AnswerIds)
                {
                    var ansHis = new Exam_Room_Student_Answer_HisTory
                    {
                        Answer_Id = answerId,
                        Exam_Room_Student_Id = ExamRoomStudent
                    };
                    var response = await _httpClient.PostAsJsonAsync("https://localhost:7187/api/Exam_Room_Student_Answer_HisTory/Post", ansHis);

                    if (response.IsSuccessStatusCode)
                    {
                        var ERSAH = await response.Content.ReadFromJsonAsync<Exam_Room_Student_Answer_HisTory>();
                        AnsHislst.Add(ERSAH);
                    }
                }
            }
            #endregion

            return AnsHislst;
        }

        public async Task<bool> CreateStudentAnswer(List<int> lstAns, string studentCode, int testId) // trắc nghiệm
        {
            try
            {
                if ((lstAns == null && lstAns.Count <= 0) || studentCode == null || testId <= 0)
                {
                    return false;
                }

                var filterStudent = new CommonFilterRequest
                {
                    Filters = new Dictionary<string, string>
                    {
                        { "Student_Code", studentCode }
                    }
                };

                var repStudent = await _httpClient.PostAsJsonAsync("https://localhost:7187/api/Student/common/get", filterStudent);

                if (!repStudent.IsSuccessStatusCode)
                {
                    return false;
                }

                Data_Base.Models.S.Student student = (await repStudent.Content.ReadFromJsonAsync<List<Data_Base.Models.S.Student>>()).SingleOrDefault();

                List<Data_Base.Models.E.Exam_Room_Student_Answer_HisTory> lstExamRoomStudentAnsHt = new List<Data_Base.Models.E.Exam_Room_Student_Answer_HisTory>();

                var filter = new CommonFilterRequest
                {
                    Filters = new Dictionary<string, string>
                        {
                             { "Student_Id", student.Id.ToString() },
                             { "Test_Id", testId.ToString()}
                        }
                };

                var response = await _httpClient.PostAsJsonAsync("https://localhost:7187/api/Exam_Room_Student/common/get", filter);

                if (!response.IsSuccessStatusCode)
                {
                    return false;
                }

                var ers = (await response.Content.ReadFromJsonAsync<List<Data_Base.Models.E.Exam_Room_Student>>()).SingleOrDefault();

                foreach (var item in lstAns)
                {
                    Data_Base.Models.E.Exam_Room_Student_Answer_HisTory examRoomStudentAnsHt = new Data_Base.Models.E.Exam_Room_Student_Answer_HisTory();
                    examRoomStudentAnsHt.Answer_Id = item;
                    examRoomStudentAnsHt.Exam_Room_Student_Id = ers.Id;

                    lstExamRoomStudentAnsHt.Add(examRoomStudentAnsHt);
                }

                if (lstExamRoomStudentAnsHt != null && lstExamRoomStudentAnsHt.Count > 0)
                {
                    var lstHis = await _httpClient.PostAsJsonAsync("https://localhost:7187/api/Exam_Room_Student_Answer_HisTory/PostList", lstExamRoomStudentAnsHt);

                    if (lstHis.IsSuccessStatusCode)
                    {
                        var studentAnswers = await lstHis.Content.ReadFromJsonAsync<List<Exam_Room_Student_Answer_HisTory>>();

                        var filterTQ = new CommonFilterRequest
                        {
                            Filters = new Dictionary<string, string>
                            {
                                 { "Test_Id", testId.ToString()}
                            }
                        };

                        var TQ = await _httpClient.PostAsJsonAsync("https://localhost:7187/api/Test_Question/common/get", filter);

                        List<Test_Question> lstTQ = await TQ.Content.ReadFromJsonAsync<List<Data_Base.Models.T.Test_Question>>();

                        int totalQuestions = lstTQ.Count;

                        int correctCount = 0;
                        foreach (var testQuestion in lstTQ)
                        {
                            int questionId = testQuestion.Question_Id;

                            // 3.1. Lấy đáp án đúng của câu hỏi từ API Answer
                            var filterAns = new CommonFilterRequest
                            {
                                Filters = new Dictionary<string, string>
                            {
                                 { "Question_Id", questionId.ToString() },
                            }
                            };

                            var ansResponse = await _httpClient.PostAsJsonAsync("https://localhost:7187/api/Test/common/get", filterAns);
                            if (!ansResponse.IsSuccessStatusCode) continue;

                            var answers = await ansResponse.Content.ReadFromJsonAsync<List<Data_Base.Models.A.Answers>>();
                            if (answers == null) continue;

                            var correctAnswerIds = answers.Where(a => a.Right_Answer == 1).Select(a => a.Id).ToList();

                            // 3.2. Lấy các đáp án học sinh đã chọn cho câu hỏi hiện tại
                            var studentSelectedIds = studentAnswers.Where(sa => answers.Any(a => a.Id == sa.Answer_Id)).Select(sa => sa.Answer_Id).ToList();

                            // 3.3. So sánh: học sinh chọn đúng hết và không dư thừa
                            bool isCorrect =
                                studentSelectedIds.Count == correctAnswerIds.Count &&
                                !studentSelectedIds.Except(correctAnswerIds).Any();

                            if (isCorrect)
                                correctCount++;
                        }

                        double pointPerQuestion = 10.0 / totalQuestions;// tính điểm cảu bài thi

                        DateTime currentTime = DateTime.Now;
                        int time = (int)(currentTime - ConvertLong.ConvertLongToDateTime(ers.Check_Time)).TotalSeconds;

                        Exam_HisTory examHistory = new Exam_HisTory();
                        examHistory.Score = pointPerQuestion; /// điểm
                        examHistory.Create_Time = ConvertLong.ConvertDateTimeToLong(currentTime); // thời gian kết thúc
                        examHistory.Exam_Room_Student_Id = ers.Id;
                        examHistory.Actual_Execution_Time = time; /// thời gian thi thực tế            

                        var checkExHis = await _httpClient.PostAsJsonAsync("https://localhost:7187/api/Exam_HisTory/Post", examHistory);

                        if (!checkExHis.IsSuccessStatusCode)
                        {
                            return false;
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

        public async Task<bool> CreateAnswerStudentAns(List<Ans> lstAns, string studentId, int testId) /// tự luận
        {
            bool s = true;
            try
            {
                if (lstAns == null && lstAns.Count <= 0 && studentId != null & testId > 0)
                {
                    s = false;
                    return s;
                }

                List<Data_Base.Models.E.Exam_Room_Student_Answer_HisTory> lstExamRoomStudentAnsHt = new List<Data_Base.Models.E.Exam_Room_Student_Answer_HisTory>();
                List<Data_Base.Models.A.Answers> lstAnswersADO = new List<Data_Base.Models.A.Answers>();
                var filter = new CommonFilterRequest
                {
                    Filters = new Dictionary<string, string>
                        {
                             { "Student_Id", studentId },
                             { "Test_Id", testId.ToString()}
                        }
                };

                var response = await _httpClient.PostAsJsonAsync("https://localhost:7187/api/Exam_Room_Student/common/get", filter);

                if (!response.IsSuccessStatusCode)
                {
                    s = false;
                    return s;
                }

                var ersId = (await response.Content.ReadFromJsonAsync<List<Data_Base.Models.E.Exam_Room_Student>>()).SingleOrDefault().Id;


                foreach (var item in lstAns)
                {
                    Data_Base.Models.A.Answers ans = new Data_Base.Models.A.Answers();
                    ans.Answers_Name = item.Name;
                    ans.Question_Id = item.QuesId;
                    ans.Right_Answer = 1;

                    lstAnswersADO.Add(ans);
                }

                if (lstAnswersADO != null)
                {
                    var answersRep = await _httpClient.PostAsJsonAsync("https://localhost:7187/api/Answers/PostList", lstAnswersADO);

                    if (!answersRep.IsSuccessStatusCode)
                    {
                        s = false;
                        return s;
                    }

                    var lstAnswersId = (await answersRep.Content.ReadFromJsonAsync<List<Data_Base.Models.A.Answers>>()).Select(o => o.Id).ToList();

                    foreach (var item in lstAnswersId)
                    {
                        Data_Base.Models.E.Exam_Room_Student_Answer_HisTory examRoomStudentAnsHt = new Data_Base.Models.E.Exam_Room_Student_Answer_HisTory();
                        examRoomStudentAnsHt.Answer_Id = item;
                        examRoomStudentAnsHt.Exam_Room_Student_Id = ersId;

                        lstExamRoomStudentAnsHt.Add(examRoomStudentAnsHt);
                    }

                    if (lstExamRoomStudentAnsHt != null && lstExamRoomStudentAnsHt.Count > 0)
                    {
                        await _httpClient.PostAsJsonAsync("https://localhost:7187/api/Exam_Room_Student_Answer_HisTory/PostList", lstExamRoomStudentAnsHt);
                    }
                }

                return s;
            }
            catch (Exception ex)
            {
                s = false;
                return s;
            }
        }

        public class Ans
        {
            public int QuesId { get; set; }
            public string Name { get; set; }
        }

        public class HistDTO
        {
            public int PackageId { get; set; }
            public string PackageName { get; set; }
            public string PackageType { get; set; }
            public int PackageTypeId { get; set; }
            public int ExecutionTime { get; set; }
            public int TestId { get; set; }
            public string TestCode { get; set; }
            public string SubjectName { get; set; }
            public string ClassName { get; set; }
            public int PointTypeId { get; set; }
            public int Status { get; set; }
            public List<Question> Questions { get; set; }
        }

        public class Question
        {
            public int? QuestionId { get; set; }
            public string QuestionName { get; set; }
            public long? MaximumScore { get; set; }
            public int Type { get; set; }
            public int Level { get; set; }
            public List<Answer>? Answers { get; set; }
        }

        public class Answer
        {
            public int? AnswerId { get; set; }
            public string AnswersName { get; set; }
            public int IsCorrect { get; set; }
            public int? QuestionId { get; set; }
        }
    }
}
