using ASP.NET.Controllers.G;
using Data_Base.GenericRepositories;
using Data_Base.Models.E;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ASP.NET.Controllers.E
{
    [Route("api/[controller]")]
    [ApiController]
    public class Exam_HisToryController : GenericController<Exam_HisTory>
    {
        public Exam_HisToryController(GenericRepository<Exam_HisTory> repository) : base(repository)
        {
        }
    }
}
