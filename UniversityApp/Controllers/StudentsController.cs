using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using UniversityApp.Models;

namespace UniversityApp.Controllers
{
    public class StudentsController : Controller
    {
        private readonly UniversityDBContext _context;

        public StudentsController(UniversityDBContext context)
        {
            _context = context;
        }
        public IActionResult Semesters()
        {
            var student = StudentGetter();
            //student.CourseHasStudents = (ICollection<CourseHasStudent>)student.CourseHasStudents.OrderBy(c => c.Grade).ThenBy(c => c.CourseId);

            return View(student);
        }
        public  Student StudentGetter()
        {
            var s1 = HttpContext.Session.GetString("userid");

            var current_student = _context.Students.Where(a => a.Userid.ToString().Equals(s1)).FirstOrDefault();

            var student = _context.Students.Include(x => x.CourseHasStudents).ThenInclude(x => x.Course)
                .FirstOrDefault(m => m.StudentId == current_student.StudentId);
            var student1 = student;
            return student;
        }
       
        public IActionResult Grades()
        {
            var student = StudentGetter();

            return View(student);
        }
        public IActionResult Total()
        {
            var student = StudentGetter();
            return View(student);
        }
        public IActionResult Home()
        {
            var student = StudentGetter();
            student = _context.Students.Include(x => x.User).Where(a => a.Userid== student.Userid).FirstOrDefault();
            return View(student);
        }


        // GET: Students
        public async Task<IActionResult> Index()
        {
            var universityDBContext = _context.Students.Include(s => s.User);
            return View(await universityDBContext.ToListAsync());
        }

        // GET: Students/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Students == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .Include(s => s.User)
                .FirstOrDefaultAsync(m => m.StudentId == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // GET: Students/Create
        public IActionResult Create()
        {
            ViewData["Userid"] = new SelectList(_context.Users, "Userid", "Userid");
            return View();
        }

        // POST: Students/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("StudentId,RegistrationNumber,Name,Surname,Department,Userid")] Student student)
        {
            if (ModelState.IsValid)
            {
                _context.Add(student);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Userid"] = new SelectList(_context.Users, "Userid", "Userid", student.Userid);
            return View(student);
        }

        // GET: Students/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Students == null)
            {
                return NotFound();
            }

            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }
            ViewData["Userid"] = new SelectList(_context.Users, "Userid", "Userid", student.Userid); 
            return View(student);
        }

        // POST: Students/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("StudentId,RegistrationNumber,Name,Surname,Department,Userid")] Student student)
        {
            if (id != student.StudentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(student);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentExists(student.StudentId))
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
            ViewData["Userid"] = new SelectList(_context.Users, "Userid", "Userid", student.Userid);
            return View(student);
        }

        // GET: Students/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Students == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .Include(s => s.User)
                .FirstOrDefaultAsync(m => m.StudentId == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Students == null)
            {
                return Problem("Entity set 'UniversityDBContext.Students'  is null.");
            }
            var student = await _context.Students.FindAsync(id);
            if (student != null)
            {
                _context.Students.Remove(student);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StudentExists(int id)
        {
          return _context.Students.Any(e => e.StudentId == id);
        }
    }
}
