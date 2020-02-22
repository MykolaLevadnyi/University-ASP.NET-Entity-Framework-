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
    public class ShedulesController : Controller
    {
        private readonly UniversityContext _context;

        public ShedulesController(UniversityContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Indexx()
        {
            var universityContext = _context.Shedule.Include(s => s.Group).Include(s => s.Day);
            return View(await universityContext.ToListAsync());
        }
        // GET: Shedules
        public async Task<IActionResult> Index(int? id , string name)
        {
            ViewBag.GroupId = id;
            ViewBag.GroupName = name;
            var universityContext = _context.Shedule.Where(s=>s.GroupId==id).Include(s => s.Day);
            return View(await universityContext.ToListAsync());
        }

        public async Task<IActionResult> ShowLessons(long? id/*, int? groupid*/)
        {
            if(id==null) return NotFound();

            var Shedule = await _context.Shedule
                .FirstOrDefaultAsync(m => m.Id == id);

            return RedirectToAction("Index", "LessonToschedules", new { ID = id });

        }

        // GET: Shedules/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shedule = await _context.Shedule
                .Include(s => s.Day)
                .Include(s => s.Group)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (shedule == null)
            {
                return NotFound();
            }

            return View(shedule);
        }

        // GET: Shedules/Create
        public IActionResult Create(long? id,string name)
        {
            ViewBag.GroupId = id;
            ViewBag.GroupN = name;
            //ViewBag.GroupN = name;
            var cdays = _context.Shedule.Where(s => s.GroupId == id).Select(d => d.Day);
            var days = from day in _context.Days
                        where !cdays.Contains(day)
                        select day;

            ViewData["DayId"] = new SelectList(days/*_context.Days*/, "Id", "Name");
            return View();
        }

        // POST: Shedules/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(long? groupId, [Bind("Id,DayId")] Shedule shedule)
        {
            shedule.GroupId = groupId;
            if (ModelState.IsValid)
            {
                _context.Add(shedule);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", new { id = groupId, name = _context.Groups.Where(g => g.Id == groupId).FirstOrDefault().Name });
                //  return RedirectToAction(nameof(Index));
            }
            ViewData["DayId"] = new SelectList(_context.Days, "Id", "Name", shedule.DayId);
            return RedirectToAction("Index", new {id=groupId,name= _context.Groups.Where(g=>g.Id==groupId).FirstOrDefault().Name });
        }

        // GET: Shedules/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shedule = await _context.Shedule.FindAsync(id);
            if (shedule == null)
            {
                return NotFound();
            }
            ViewData["DayId"] = new SelectList(_context.Days, "Id", "Name", shedule.DayId);
            ViewData["GroupId"] = new SelectList(_context.Groups, "Id", "Name", shedule.GroupId);
            return View(shedule);
        }

        // POST: Shedules/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,DayId,GroupId")] Shedule shedule)
        {
            if (id != shedule.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(shedule);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SheduleExists(shedule.Id))
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
            ViewData["DayId"] = new SelectList(_context.Days, "Id", "Name", shedule.DayId);
            ViewData["GroupId"] = new SelectList(_context.Groups, "Id", "Name", shedule.GroupId);
            return View(shedule);
        }

        // GET: Shedules/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shedule = await _context.Shedule
                .Include(s => s.Day)
                .Include(s => s.Group)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (shedule == null)
            {
                return NotFound();
            }

            return View(shedule);
        }

        // POST: Shedules/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var shedule = await _context.Shedule.FindAsync(id);
            _context.Shedule.Remove(shedule);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SheduleExists(long id)
        {
            return _context.Shedule.Any(e => e.Id == id);
        }
    }
}
