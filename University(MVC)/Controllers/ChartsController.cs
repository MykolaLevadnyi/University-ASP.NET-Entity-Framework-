using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace University_MVC_.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChartsController : ControllerBase
    {
        private readonly UniversityContext _context;

        public ChartsController(UniversityContext context)
        {
            _context = context;
        }
        [HttpGet("JsonData")]
        public JsonResult JsonData()
        {
            var departments = _context.Deparments.Include(d => d.Groups).ToList();
            List<object> catgroups = new List<object>();

            catgroups.Add(new[] {"Кафедра","К-сть студентів"});

            foreach (var d in departments)
            {
                var groups = from g in _context.Groups
                             where g.DepartmentId == d.Id
                             select g.Students;
                int count = 0;
                foreach(var g in groups)
                {
                    count += g.Count();
                }
                catgroups.Add(new object[] { d.Name, count/*d.Groups.Count()*/ }) ;
            }
            return new JsonResult(catgroups);

        }
        [HttpGet("JsonDataL")]
        public JsonResult JsonDataL(int groupId)
        {
            var lessons = _context.Lessons;

            List<object> catgroups = new List<object>();

            catgroups.Add(new[] { "Премет", "К-сть на неділю" });

            foreach (var l in lessons)
            {
                var Lcount = _context.LessonToschedule.Where(lts => lts.LessonId == l.Id);
                catgroups.Add(new object[] { l.Name, Lcount.Count()/*d.Groups.Count()*/ });
            }
            return new JsonResult(catgroups);

        }


    }
}