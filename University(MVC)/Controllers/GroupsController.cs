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

            if (ModelState.IsValid)
            {
                _context.Add(groups);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClassPrId"] = new SelectList(_context.Students, "Id", "Name", groups.ClassPrId);
            ViewData["DepartmentId"] = new SelectList(_context.Deparments, "Id", "Name", groups.DepartmentId);
            return View(groups);
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

            if (ModelState.IsValid)
            {
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
                return RedirectToAction(nameof(Index));
            } 
            ViewData["ClassPrId"] = new SelectList(_context.Students.Where(g => g.GroupId == groups.Id), "Id", "Name", groups.ClassPrId);
            ViewData["DepartmentId"] = new SelectList(_context.Deparments, "Id", "Name", groups.DepartmentId);
            return View(groups);
        }

        // GET: Groups/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var groups = await _context.Groups
                .Include(g => g.ClassPr)
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
