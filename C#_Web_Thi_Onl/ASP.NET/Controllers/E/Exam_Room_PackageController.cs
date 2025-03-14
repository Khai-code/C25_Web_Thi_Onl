using ASP.NET.Controllers.G;
using Data_Base.GenericRepositories;
using Data_Base.Models.E;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ASP.NET.Controllers.E
{
    [Route("api/[controller]")]
    [ApiController]
    public class Exam_Room_PackageController : GenericController<Exam_Room_Package>
    {
        public Exam_Room_PackageController(GenericRepository<Exam_Room_Package> repository) : base(repository)
        {
        }
    }
}
