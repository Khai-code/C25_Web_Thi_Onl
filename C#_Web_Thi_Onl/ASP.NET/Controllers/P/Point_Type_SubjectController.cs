using ASP.NET.Controllers.G;
using Data_Base.GenericRepositories;
using Data_Base.Models.P;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ASP.NET.Controllers.P
{
    [Route("api/[controller]")]
    [ApiController]
    public class Point_Type_SubjectController : GenericController<Point_Type_Subject>
    {
        public Point_Type_SubjectController(GenericRepository<Point_Type_Subject> repository) : base(repository)
        {
        }
    }
}
