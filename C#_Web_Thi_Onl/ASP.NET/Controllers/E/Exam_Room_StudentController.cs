using ASP.NET.Controllers.G;
using Data_Base.GenericRepositories;
using Data_Base.Models.E;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ASP.NET.Controllers.E
{
    [Route("api/[controller]")]
    [ApiController]
    public class Exam_Room_StudentController : GenericController<Exam_Room_Student>
    {
        public Exam_Room_StudentController(GenericRepository<Exam_Room_Student> repository) : base(repository)
        {
        }
    }
}
