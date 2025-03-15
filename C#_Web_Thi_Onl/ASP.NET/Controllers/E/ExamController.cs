﻿using ASP.NET.Controllers.G;
using Data_Base.GenericRepositories;
using Data_Base.Models.E;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ASP.NET.Controllers.E
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExamController : GenericController<Exam>
    {
        public ExamController(GenericRepository<Exam> repository) : base(repository)
        {
        }
    }
}
