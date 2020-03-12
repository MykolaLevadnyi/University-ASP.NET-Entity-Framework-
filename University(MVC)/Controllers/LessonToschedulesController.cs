using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using University_MVC_;

namespace University_MVC_.Controllers
{
    public class LessonToschedulesController : Controller
    {
        private readonly UniversityContext _context;

        public LessonToschedulesController(UniversityContext context)
        {
            _context = context;
        }

        // GET: LessonToschedules
        public async Task<IActionResult> Index(int? id,string error)
        {
            ViewBag.Error = error;
            ViewBag.SheduleId = id;
            var universityContext = _context.LessonToschedule.Where(s=>s.ScheduleId==id).Include(l => l.Lesson);
            return View(await universityContext.ToListAsync());
        }

        // GET: LessonToschedules/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lessonToschedule = await _context.LessonToschedule
                .Include(l => l.Lesson)
                .Include(l => l.Schedule)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (lessonToschedule == null)
            {
                return NotFound();
            }

            return View(lessonToschedule);
        }

        // GET: LessonToschedules/Create
        public IActionResult Create(int? Id)
        {
            ViewBag.SheduleId = Id;
            ViewData["LessonId"] = new SelectList(_context.Lessons, "Id", "Name");
            return View();
        }

        // POST: LessonToschedules/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int ScheduleId, [Bind("Id,LessonId,Num")] LessonToschedule lessonToschedule)
        {
            LessonToschedule lts = new LessonToschedule();
            lts.LessonId = lessonToschedule.LessonId;
            lessonToschedule.ScheduleId = ScheduleId;
            lts.ScheduleId = ScheduleId;
            lts.Num = lessonToschedule.Num;
            // CHECK LESSONS FOR UNIQUENESS
            var lessons = from l in _context.LessonToschedule
                          where l.ScheduleId == ScheduleId
                          select l.LessonId;
            // CHECK LESSONS_NUM FOR UNIQUENESS

            var l_nums = from l in _context.LessonToschedule
                         where l.ScheduleId == ScheduleId
                         select l.Num;
            if (ModelState.IsValid && !lessons.Contains(lessonToschedule.LessonId) && !l_nums.Contains(lessonToschedule.Num) && lessonToschedule.Num>0)
            {
            _context.Add(lts);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", new { id = ScheduleId });
            }
            return RedirectToAction("Index", new { id = ScheduleId , error="Не створено"});
        }

        // GET: LessonToschedules/Edit/5
        public async Task<IActionResult> Edit(long? id,long? sheduleId)
        {
       
            if (id == null)
            {
                return NotFound();
            }

            var lessonToschedule = await _context.LessonToschedule.FindAsync(id);
            if (lessonToschedule == null)
            {
                return NotFound();
            }
            ViewBag.SheduleId = sheduleId;
            ViewData["LessonId"] = new SelectList(_context.Lessons, "Id", "Name", lessonToschedule.LessonId);
            return View(lessonToschedule);
        }

        // POST: LessonToschedules/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id,[Bind("Id,ScheduleId,LessonId,Num")] LessonToschedule lessonToschedule)
        {
            // CHECK LESSONS_NUM FOR UNIQUENESS

            var l_nums = (from l in _context.LessonToschedule
                         where l.ScheduleId == lessonToschedule.ScheduleId
                         select l.Num).ToList();

            if(l_nums.Contains(lessonToschedule.Num))
            {
                ModelState.AddModelError("Num", "This num is incorect");
                return View(lessonToschedule);
            }
            if (id != lessonToschedule.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid && !l_nums.Contains(lessonToschedule.Num) && lessonToschedule.Num > 0)
            {
                try
                {
                    _context.Update(lessonToschedule);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LessonToscheduleExists(lessonToschedule.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index", new { id = _context.LessonToschedule.Where(s => s.Id == id).FirstOrDefault().ScheduleId });
            }
            return RedirectToAction("Index",new {id=_context.LessonToschedule.Where(s=>s.Id==id).FirstOrDefault().ScheduleId , error = "Помилка редагування" });
        }

        // GET: LessonToschedules/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lessonToschedule = await _context.LessonToschedule
                .Include(l => l.Lesson)
                .Include(l => l.Schedule)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (lessonToschedule == null)
            {
                return NotFound();
            }

            return View(lessonToschedule);
        }

        // POST: LessonToschedules/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long? id)
        {
            var s = _context.LessonToschedule.Where(s => s.Id == id).FirstOrDefault().ScheduleId;
            var lessonToschedule = await _context.LessonToschedule.FindAsync(id);
            _context.LessonToschedule.Remove(lessonToschedule);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", new { id = s });
        }

        private bool LessonToscheduleExists(long id)
        {
            return _context.LessonToschedule.Any(e => e.Id == id);
        }
    }
}
