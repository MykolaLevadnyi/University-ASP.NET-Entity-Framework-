using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using University_MVC_;
using System.Web;
using System.IO;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;

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
        public async Task<IActionResult> Index(string error)
        {
            ViewBag.Error = error;
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
            var names = from g in _context.Groups
                        select g.Name;
            if (names.Contains(groups.Name))
            {
                ModelState.AddModelError("Name","This name is already exist");
                return View(groups);
            }
            if (ModelState.IsValid && groups.Name.Length==3 && !names.Contains(groups.Name)) {
  
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
           
            return RedirectToAction("Index", new { error = "Не створено" });
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Import(IFormFile fileExcel)
        {
            if (ModelState.IsValid)
            {
                if (fileExcel != null)
                {
                    using (var stream = new FileStream(fileExcel.FileName, FileMode.Create))
                    {
                        await fileExcel.CopyToAsync(stream);
                        using (XLWorkbook workBook = new XLWorkbook(stream, XLEventTracking.Disabled))
                        {
                            //перегляд усіх листів (в даному випадку категорій)
                            foreach (IXLWorksheet worksheet in workBook.Worksheets)
                            {
                                if(worksheet.Name == "Departments")
                                {
                                    foreach (IXLRow row in worksheet.RowsUsed().Skip(1))
                                    {
                                        try
                                        {
                                            if(row.Cell(1)!=null && row.Cell(2) != null)
                                            {
                                                Deparments dep = new Deparments();
                                                Groups group = new Groups();
                                                dep.Name = row.Cell(1).Value.ToString();
                                                group = _context.Groups.Where(g => g.Name == row.Cell(2).Value.ToString()).FirstOrDefault();
                                                if (group == null)
                                                {
                                                    group.Name = row.Cell(2).Value.ToString();
                                                }
                                                var names_d = (from d in _context.Deparments
                                                               select d.Name).ToList();
                                                if (!names_d.Contains(row.Cell(1).Value.ToString()))
                                                {
                                                    _context.Deparments.Add(dep);
                                                    await _context.SaveChangesAsync();
                                                }
                                                group.DepartmentId = _context.Deparments.Where(d => d.Name == dep.Name).FirstOrDefault().Id;

                                                var names_g = (from g in _context.Groups
                                                               select g.Name).ToList();
                                                if (!names_g.Contains(row.Cell(2).Value.ToString()))
                                                {
                                                    _context.Groups.Add(group);
                                                    await _context.SaveChangesAsync();
                                                    for (int i = 1; i < 8; i++)
                                                    {
                                                        Shedule shedule = new Shedule();
                                                        shedule.GroupId = group.Id;
                                                        shedule.DayId = i;
                                                        _context.Shedule.Add(shedule);
                                                    }
                                                }
                                                else
                                                {
                                                    _context.Groups.Update(group);
                                               }
                                                await _context.SaveChangesAsync();
                                            }
                                        }
                                        catch (Exception e)
                                        {
                                            //logging самостійно :)

                                        }
                                    }

                                }
                                else if(worksheet.Name== "Schedules")
                                {
                                    foreach (IXLRow row in worksheet.RowsUsed().Skip(1))
                                    {
                                        try
                                        {
                                            Groups group = _context.Groups.Where(g => g.Name == row.Cell(1).Value.ToString()).FirstOrDefault();
                                            var names_g = (from g in _context.Groups
                                                           select g.Name).ToList();
                                            if (names_g.Contains(group.Name))
                                            {
                                                var names_l = (from l in _context.Lessons
                                                               select l.Name).ToList();
                                                Shedule shedule = _context.Shedule.Where(s => s.GroupId == group.Id && s.Day.Name == row.Cell(2).Value.ToString()).FirstOrDefault();
                                                   for(int i = 3; i < 8; i++)
                                                   {
                                                        LessonToschedule lts = new LessonToschedule();
                                                        LessonToschedule lts1 = _context.LessonToschedule.Where(l => l.ScheduleId == shedule.Id && l.Num == i - 2).FirstOrDefault();
                                                        if (lts1 != null)
                                                        {
                                                            if(row.Cell(i).Value.ToString()!=null && names_l.Contains(row.Cell(i).Value.ToString()))
                                                            {
                                                                lts1.LessonId = _context.Lessons.Where(l => l.Name == row.Cell(i).Value.ToString()).FirstOrDefault().Id;
                                                                _context.LessonToschedule.Update(lts1);
                                                            } 
                                                        }
                                                        else
                                                        {
                                                            if (row.Cell(i).Value.ToString() != null && names_l.Contains(row.Cell(i).Value.ToString()))
                                                            {
                                                                lts.ScheduleId = shedule.Id;
                                                                lts.Num = i - 2;
                                                                lts1.LessonId = _context.Lessons.Where(l => l.Name == row.Cell(i).Value.ToString()).FirstOrDefault().Id;
                                                                _context.LessonToschedule.Add(lts1);
                                                            }
                                                        }
                                                    await _context.SaveChangesAsync();
                                                    }
                                            }
                                        }
                                        catch (Exception e)
                                        {
                                            //logging самостійно :)

                                        }
                                    }
                                }
                                else {
                                    //worksheet.Name - назва категорії. Пробуємо знайти в БД, якщо відсутня, то створюємо нову
                                    Groups newcat;
                                    var c = (from cat in _context.Groups
                                             where cat.Name.Contains(worksheet.Name)
                                             select cat).ToList();
                                    if (c.Count > 0)
                                    {
                                        newcat = c[0];
                                        foreach (IXLRow row in worksheet.RowsUsed().Skip(1))
                                        {
                                            try
                                            {
                                                Students student = new Students();
                                                student = _context.Students.Where(s => s.Name == row.Cell(1).Value.ToString() && s.Surname == row.Cell(2).Value.ToString() && s.Gender == row.Cell(3).Value.ToString() && s.Birthday == Convert.ToDateTime(row.Cell(4).Value.ToString())).FirstOrDefault();
                                                if (student == null)
                                                {
                                                    student.Name = row.Cell(1).Value.ToString();
                                                    student.Surname = row.Cell(2).Value.ToString();
                                                    student.Gender = row.Cell(3).Value.ToString();
                                                    student.Birthday = Convert.ToDateTime(row.Cell(4).Value.ToString());
                                                    student.GroupId = newcat.Id;
                                                    _context.Students.Add(student);
                                                }
                                                else
                                                {
                                                    student.Name = row.Cell(1).Value.ToString();
                                                    student.Surname = row.Cell(2).Value.ToString();
                                                    student.Gender = row.Cell(3).Value.ToString();
                                                    student.Birthday = Convert.ToDateTime(row.Cell(4).Value.ToString());
                                                    student.GroupId = newcat.Id;
                                                    _context.Students.Update(student);
                                                }
                                                await _context.SaveChangesAsync();
                                            }
                                            catch (Exception e)
                                            {
                                                //logging самостійно :)

                                            }
                                        }
                                    }
                                                     
                                    
                                }
                                
                            }
                        }
                    }
                }

                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        public ActionResult Export()
        {
            using (XLWorkbook workbook = new XLWorkbook(XLEventTracking.Disabled))
            {   // DEPARTMENT INFO
                var departments = _context.Deparments.Include("Groups").ToList();
                var dep_sheet = workbook.Worksheets.Add("Departments");
                int dep_counter = 0;
                foreach(var d in departments)
                {
                    dep_sheet.Cell("A1").Value = "Кафедра";
                    dep_sheet.Cell("B1").Value = "Група";
                    dep_sheet.Row(1).Style.Font.Bold = true;

                    var dep_groups = d.Groups.ToList();
                    for(int i=0;i<dep_groups.Count;i++)
                    {
                        dep_sheet.Cell(dep_counter + 2, 1).Value = d.Name;
                        dep_sheet.Cell(dep_counter + 2, 2).Value = dep_groups[i].Name;
                        dep_counter++;
                    }
                }

                //SCHEDULE INFO
                var schedule = _context.Shedule.Include("LessonToschedule").Include("Group").Include("Day").ToList();
                var schedule_sheet = workbook.Worksheets.Add("Schedules");
                schedule_sheet.Cell("A1").Value = "Група";
                schedule_sheet.Cell("B1").Value = "День";
                schedule_sheet.Cell("C1").Value = "1Пара";
                schedule_sheet.Cell("D1").Value = "2Пара";
                schedule_sheet.Cell("E1").Value = "3Пара";
                schedule_sheet.Cell("F1").Value = "4Пара";
                schedule_sheet.Cell("G1").Value = "5Пара";
                schedule_sheet.Row(1).Style.Font.Bold = true;
                int schedule_counter = 0;
                foreach (var s in schedule)
                {
                    schedule_sheet.Cell(schedule_counter + 2, 1).Value = s.Group.Name;
                    schedule_sheet.Cell(schedule_counter + 2, 2).Value = s.Day.Name;
                    var lessons_by_s = (from lts in _context.LessonToschedule
                                  where lts.ScheduleId == s.Id
                                  select lts).ToList();
                    for(int i = 0; i < 5; i++)
                    {
                        schedule_sheet.Cell(schedule_counter + 2, i + 3).Value = "#";
                        foreach (var l in lessons_by_s)
                        {
                            if (l.Num-1 == i) schedule_sheet.Cell(schedule_counter + 2, i + 3).Value = _context.Lessons.Where(les => les.Id == l.LessonId).FirstOrDefault().Name;
                        }
                    }
                    schedule_counter++;
                }


                /*//LESSONS INFO
                var lessons = _context.Lessons.ToList();
                var Lessons_sheet = workbook.Worksheets.Add("Lessons");
                Lessons_sheet.Cell("A1").Value = "Назва";
                int lessons_counter = 0;
                foreach(var l in lessons)
                {
                    Lessons_sheet.Cell(lessons_counter + 2, 1).Value = l.Name;
                    lessons_counter++;
                }*/

                //GROUPS INFO
                var groups = _context.Groups.Include("Students").ToList();
                //тут, для прикладу ми пишемо усі книжки з БД, в своїх проектах ТАК НЕ РОБИТИ (писати лише вибрані)
                foreach (var c in groups)
                {
                    var worksheet = workbook.Worksheets.Add(c.Name);

                    worksheet.Cell("A1").Value = "Ім'я";
                    worksheet.Cell("B1").Value = "Прізвище";
                    worksheet.Cell("C1").Value = "Стать";
                    worksheet.Cell("D1").Value = "День Народження";
                    worksheet.Row(1).Style.Font.Bold = true;
                    var students = c.Students.ToList();

                    //нумерація рядків/стовпчиків починається з індекса 1 (не 0)
                    for (int i = 0; i < students.Count; i++)
                    {
                        worksheet.Cell(i + 2, 1).Value = students[i].Name;
                        worksheet.Cell(i + 2, 2).Value = students[i].Surname;
                        worksheet.Cell(i + 2, 3).Value = students[i].Gender;
                        worksheet.Cell(i + 2, 4).Value = students[i].Birthday.Value;
                    }
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    stream.Flush();

                    return new FileContentResult(stream.ToArray(),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                    {
                        FileDownloadName = $"library_{DateTime.UtcNow.ToShortDateString()}.xlsx"
                    };
                }
            }
        }



        // POST: Groups/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,Name,ClassPrId,DepartmentId")] Groups groups)
        {
            var names = from g in _context.Groups
                        where g.Name!=groups.Name
                        select g.Name;
            if (names.Contains(groups.Name))
            {
                ModelState.AddModelError("Name", "This name is already exist");
                return View(groups);
            }
            if (id != groups.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid && groups.Name.Length == 3 && !names.Contains(groups.Name))
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
                return RedirectToAction("Index", "Groups");
            }
            
            ViewData["ClassPrId"] = new SelectList(_context.Students.Where(g => g.GroupId == groups.Id), "Id", "Name", groups.ClassPrId);
            ViewData["DepartmentId"] = new SelectList(_context.Deparments, "Id", "Name", groups.DepartmentId);
            //return View(groups);
            return RedirectToAction("Index", "Groups" , new { error = "Помилка редагування" });
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
