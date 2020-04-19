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
    public class DeparmentsController : Controller
    {
        private readonly UniversityContext _context;

        public DeparmentsController(UniversityContext context)
        {
            _context = context;
        }

        // GET: Deparments
        public async Task<IActionResult> Index(string error)
        {
            ViewBag.Error = error;
            return View(await _context.Deparments.ToListAsync());
        }

        // GET: Deparments/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var deparments = await _context.Deparments
                .FirstOrDefaultAsync(m => m.Id == id);
            if (deparments == null)
            {
                return NotFound();
            }

            return RedirectToAction("ByDepartment", "Groups", new { id = deparments.Id, name = deparments.Name });
        }

        // GET: Deparments/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Deparments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] Deparments deparments)
        {
            var dep_name = from d in _context.Deparments
                           select d.Name;
            if (dep_name.Contains(deparments.Name))
            {
                ModelState.AddModelError("Name","This name is already exist");
                return View(deparments);
            }
            if (ModelState.IsValid && !dep_name.Contains(deparments.Name))
            {
                _context.Add(deparments);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction("Index", new { error = "Не створено" });
        }

        // GET: Deparments/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var deparments = await _context.Deparments.FindAsync(id);
            if (deparments == null)
            {
                return NotFound();
            }
            return View(deparments);
        }

        // POST: Deparments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,Name")] Deparments deparments)
        {
            var dep_name = from d in _context.Deparments
                           select d.Name;
            if (dep_name.Contains(deparments.Name))
            {
                ModelState.AddModelError("Name", "This name is already exist");
                return View(deparments);
            }
            if (id != deparments.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid && !dep_name.Contains(deparments.Name))
            {
                try
                {
                    _context.Update(deparments);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DeparmentsExists(deparments.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction("Index", new { error = "Не редаговано" });
        }

        // GET: Deparments/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var deparments = await _context.Deparments
                .FirstOrDefaultAsync(m => m.Id == id);
            if (deparments == null)
            {
                return NotFound();
            }

            return View(deparments);
        }

        // POST: Deparments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {

            var deparments = await _context.Deparments.FindAsync(id);
            List<long?> groupsid = new List<long?>();
            foreach(var s in _context.Groups)
            {
                if (s.DepartmentId == id) groupsid.Add(s.Id);
            }
            var students = from s in _context.Students
                           where groupsid.Contains(s.GroupId)
                           select s;
            var groups = from g in _context.Groups
                         where g.DepartmentId == id
                         select g;
            foreach(var g in groups)
            {
                g.ClassPrId = null;
            }
            foreach(var g in groups)
            {
                _context.Groups.Update(g);
            }
            await _context.SaveChangesAsync();
            foreach (var s in students)
            {
                _context.Students.Remove(s);
            }
            await _context.SaveChangesAsync();
            
            foreach (var g in groups)
            {
                var Schedules = from s in _context.Shedule
                                where s.GroupId == g.Id
                                select s;
                foreach(var s in Schedules)
                {
                    var lessontoshedule = from lts in _context.LessonToschedule
                                          where lts.ScheduleId == s.Id
                                          select lts;
                    foreach(var lts in lessontoshedule)
                    {
                        
                        _context.LessonToschedule.Remove(lts);
                       // await _context.SaveChangesAsync();
                    }
 
                   // _context.Shedule.Remove(s);
                }
                //await _context.SaveChangesAsync();
                //_context.Groups.Remove(g);
            }
            await _context.SaveChangesAsync();
            foreach (var g in groups)
            {
                var Schedules = from s in _context.Shedule
                                where s.GroupId == g.Id
                                select s;
                foreach (var s in Schedules)
                { _context.Shedule.Remove(s); }
            }
            await _context.SaveChangesAsync();
            foreach (var g in groups)
            {
                _context.Groups.Remove(g);
            }
                await _context.SaveChangesAsync();
            _context.Deparments.Remove(deparments);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DeparmentsExists(long id)
        {
            return _context.Deparments.Any(e => e.Id == id);
        }
    }
}
