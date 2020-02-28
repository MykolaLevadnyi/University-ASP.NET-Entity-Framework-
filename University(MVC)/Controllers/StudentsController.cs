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
    public class StudentsController : Controller
    {
        private readonly UniversityContext _context;

        public StudentsController(UniversityContext context)
        {
            _context = context;
        }

        // GET: Students
        public async Task<IActionResult> Index(int? id,string name)
        {
            //var universityContext = _context.Students.Include(s => s.Group);
            //return View(await universityContext.ToListAsync());
            if (id == null) return RedirectToAction("Index", "Groups");
            ViewBag.GroupId = id;
            ViewBag.GroupName = name;
            var studentsByGroup = _context.Students.Where(s => s.GroupId == id).Include(s => s.Group);

            return View(await studentsByGroup.ToListAsync());
        }

        // GET: Students/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var students = await _context.Students
                .Include(s => s.Group)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (students == null)
            {
                return NotFound();
            }

            return View(students);
        }

        // GET: Students/Create
        public IActionResult Create(int? groupId)
        {
            ViewBag.GroupId = groupId;
            ViewBag.GroupName = _context.Groups.Where(g => g.Id == groupId).FirstOrDefault().Name;
            var Genders = new List<string> { "Male", "Female" };
            ViewData["Gender"]= new SelectList(Genders);
            return View();
        }

        // POST: Students/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int? groupId,[Bind("Id,Name,Surname,Gender,Birthday")] Students students)
        {
            students.GroupId = groupId;
            ViewBag.GroupId = groupId;
            if (ModelState.IsValid)
            {
                _context.Add(students);
                await _context.SaveChangesAsync();
                //return RedirectToAction(nameof(Index));
                return RedirectToAction("Index", "Students", new { id = groupId, name = _context.Groups.Where(g => g.Id == groupId).FirstOrDefault().Name });
            }
            //ViewData["GroupId"] = new SelectList(_context.Groups, "Id", "Name", student.GroupId);
            //return View(student);
            var Genders = new List<string> { "Male", "Female" };
            ViewData["Gender"] = new SelectList(Genders);
            return RedirectToAction("Index", "Students", new { id = groupId, name = _context.Groups.Where(g => g.Id == groupId).FirstOrDefault().Name });
        }

        // GET: Students/Edit/5
        public async Task<IActionResult> Edit(long? id,long? groupId)
        {
            ViewBag.GroupId = groupId;
            if (id == null)
            {
                return NotFound();
            }

            var students = await _context.Students.FindAsync(id);
            if (students == null)
            {
                return NotFound();
            }
            var Genders = new List<string> { "Male", "Female" };
            ViewData["Gender"] = new SelectList(Genders);
            return View(students);
        }

        // POST: Students/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, long? groupid, [Bind("Id,Name,Surname,Gender,Birthday")] Students students)
        {
            students.GroupId = groupid;
            if (id != students.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(students);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentsExists(students.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction("Index", new { id = groupid, name = _context.Groups.Where(g => g.Id == groupid).FirstOrDefault().Name });
            }
            var Genders = new List<string> { "Male", "Female" };
            ViewData["Gender"] = new SelectList(Genders);
            ViewData["GroupId"] = new SelectList(_context.Groups);
            return RedirectToAction("Index", new {id =groupid,name=_context.Groups.Where(g=>g.Id==groupid).FirstOrDefault().Name });
        }

        // GET: Students/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var students = await _context.Students
                .Include(s => s.Group)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (students == null)
            {
                return NotFound();
            }

            return View(students);
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var students = await _context.Students.FindAsync(id);
            var Id = students.GroupId;
            var Name = _context.Groups.Where(g => g.Id == Id).FirstOrDefault().Name;
            var groups =await _context.Groups
                .Where(p => p.ClassPrId == id).FirstOrDefaultAsync();
            if (groups != null)
            {
                groups.ClassPrId = null;
                _context.Groups.Update(groups);
            }
            _context.Students.Remove(students);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index",new { id = Id, name = Name });
        }

        private bool StudentsExists(long id)
        {
            return _context.Students.Any(e => e.Id == id);
        }
    }
}
