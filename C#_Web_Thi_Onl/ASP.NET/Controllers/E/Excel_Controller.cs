using Data_Base.App_DbContext;
using Data_Base.Models.A;
using Data_Base.Models.Q;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Data_Base.DTO_Import_Excel;
using OfficeOpenXml;

namespace ASP.NET.Controllers.E
{
    [Route("api/[controller]")]
    [ApiController]
    public class Excel_Controller : ControllerBase
    {
        private readonly Db_Context db_Context;
        public Excel_Controller(Db_Context _Context)
        {
             db_Context = _Context;
        }

        [HttpPost("excel")]
        public async Task<IActionResult> ImportExcel(IFormFile file, [FromQuery] int packageId)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File rỗng");

            var questions = new List<QuestionImportDto>();

            using var stream = file.OpenReadStream();
            using var package = new ExcelPackage(stream);
            var sheet = package.Workbook.Worksheets[0];

            for (int row = 4; row <= sheet.Dimension.End.Row; row++)
            {
                var qName = sheet.Cells[row, 2].Text?.Trim();
                var typeText = sheet.Cells[row, 3].Text;
                var levelText = sheet.Cells[row, 4].Text;

                if (string.IsNullOrWhiteSpace(qName)) continue;

                var answers = new List<AnswerImportDto>();
                for (int col = 5; col <= sheet.Dimension.End.Column; col++)
                {
                    var cell = sheet.Cells[row, col].Text;
                    if (string.IsNullOrWhiteSpace(cell)) continue;

                    bool isCorrect = cell.Trim().StartsWith("*");
                    if (isCorrect) cell = cell.Trim().Substring(1).Trim();

                    answers.Add(new AnswerImportDto
                    {
                        Answers_Name = cell,
                        Right_Answer = isCorrect ? 1 : 0
                    });
                }

                questions.Add(new QuestionImportDto
                {
                    Question_Name = qName,
                    Question_Type_Id = int.TryParse(typeText, out var t) ? t : 1,
                    Question_Level_Id = int.TryParse(levelText, out var l) ? l : 1,
                    Package_Id = packageId,
                    Answers = answers
                });
            }

            foreach (var dto in questions)
            {
                var question = new Question
                {
                    Question_Name = dto.Question_Name,
                    Question_Type_Id = dto.Question_Type_Id,
                    Question_Level_Id = dto.Question_Level_Id,
                    Package_Id = dto.Package_Id
                };

                db_Context.Questions.Add(question);
                await db_Context.SaveChangesAsync();

                foreach (var ans in dto.Answers)
                {
                    db_Context.Answerses.Add(new Answers
                    {
                        Question_Id = question.Id,
                        Answers_Name = ans.Answers_Name,
                        Right_Answer = ans.Right_Answer
                    });
                }
            }

            await db_Context.SaveChangesAsync();
            return Ok("Thêm thành công!");
        }
    }
}
