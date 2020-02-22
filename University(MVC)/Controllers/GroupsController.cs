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
    public class GroupsController : Controller
    {
        private readonly UniversityContext _context;

        public GroupsController(UniversityContext context)
        {
            _context = context;
        }
        
        public async Task<IActionResult> ByDepartment(int? id , string name)
        {
            ViewBag.DepartmentName = name;
            var universityContext = _context.Groups.Where(g=>g.DepartmentId==id).Include(g => g.ClassPr).Include(g => g.Department);
            return View(await universityContext.ToListAsync());
        }
        // GET: Groups
        public async Task<IActionResult> Index()
        {
            var universityContext = _context.Groups.Include(g => g.ClassPr).Include(g => g.Department);
            return View(await universityContext.ToListAsync());
        }

        public async Task<IActionResult> Shedule(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var groups = await _context.Groups
                .FirstOrDefaultAsync(g => g.Id == id);
            if (groups == null)
            {
                return NotFound();
            }

            return RedirectToAction("Index", "Shedules", new { id = groups.Id, name = groups.Name });
        }
        // GET: Groups/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var groups = await _context.Groups
                .FirstOrDefaultAsync(m => m.Id == id);
            if (groups == null)
            {
                return NotFound();
            }

            //return View(groups);
            return RedirectToAction("Index", "Students", new { id = groups.Id, name = groups.Name });
        }

        // GET: Groups/Create
        public IActionResult Create()
        {
            ViewData["ClassPrId"] = new SelectList(_context.Students, "Id", "Name");
            ViewData["DepartmentId"] = new SelectList(_context.Deparments, "Id", "Name");
            return View();
        }

        // POST: Groups/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,ClassPrId,DepartmentId")] Groups groups)
        {
            
            if (ModelState.IsValid && groups.Name.Length==3) {
  
                _context.Add(groups);
                await _context.SaveChangesAsync();
                for (int i = 1; i < 8; i++)
                {
                    Shedule shedule = new Shedule();
                    shedule.DayId = i;
                    shedule.GroupId = groups.Id;
                    _context.Shedule.Add(shedule);
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction("Index");
                // return RedirectToAction(nameof(Index));
            }
            return RedirectToAction("Index");
        }

        // GET: Groups/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var groups = await _context.Groups.FindAsync(id);
            if (groups == null)
            {
                return NotFound();
            }
            ViewData["ClassPrId"] = new SelectList(_context.Students.Where(g => g.GroupId == groups.Id), "Id", "Name", groups.ClassPrId);
            //ViewData["ClassPrId"] = new SelectList(students, "Id", "Name", groups.ClassPrId);
            ViewData["DepartmentId"] = new SelectList(_context.Deparments, "Id", "Name", groups.DepartmentId);
            return View(groups);
        }

        // POST: Groups/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,Name,ClassPrId,DepartmentId")] Groups groups)
        {
            if (id != groups.Id)
            {
                return NotFound();
            }

                try
                {
                    _context.Update(groups);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GroupsExists(groups.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                //return RedirectToAction(nameof(Index));
            
            ViewData["ClassPrId"] = new SelectList(_context.Students.Where(g => g.GroupId == groups.Id), "Id", "Name", groups.ClassPrId);
            ViewData["DepartmentId"] = new SelectList(_context.Deparments, "Id", "Name", groups.DepartmentId);
            //return View(groups);
            return RedirectToAction("Index", "Groups");
        }

        // GET: Groups/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var groups = await _context.Groups
                .Include(g => g.Department)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (groups == null)
            {
                return NotFound();
            }

            return View(groups);
        }

        // POST: Groups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var groups = await _context.Groups.FindAsync(id);
            //Students delete
            groups.ClassPrId = null;
            _context.Groups.Update(groups);
            var students = from s in _context.Students
                           where s.GroupId == id
                           select s;
            foreach (var s in students)
            {
                _context.Students.Remove(s);
            }

            await _context.SaveChangesAsync();
            //LessonToShcedules delete
            List<long?> l_t_s = new List<long?>();
            foreach(var s in _context.Shedule)
            {
                if (s.GroupId == id) { l_t_s.Add(s.Id); }
            }
            var lessondtoschedule = from lts in _context.LessonToschedule
                                    where l_t_s.Contains(lts.ScheduleId)
                                    select lts;
            foreach(var lts in lessondtoschedule)
            {
                _context.LessonToschedule.Remove(lts);
            }
            await _context.SaveChangesAsync();
            //Shcdules delete
            var shedules = from s in _context.Shedule
                           where s.GroupId == id
                           select s;
            foreach(var s in shedules)
            {
                _context.Shedule.Remove(s);
            }
            await _context.SaveChangesAsync();
            //Group delete
            _context.Groups.Remove(groups);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GroupsExists(long id)
        {
            return _context.Groups.Any(e => e.Id == id);
        }
    }
}
