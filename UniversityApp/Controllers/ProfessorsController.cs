using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using UniversityApp.Models;

namespace UniversityApp.Controllers
{
    public class ProfessorsController : Controller
    {
        private readonly UniversityDBContext _context;

        public ProfessorsController(UniversityDBContext context)
        {
            _context = context;
        }

        // GET: Professor Account welcome
        public async Task<IActionResult> Account()
        {

            var userid = HttpContext.Session.GetString("userid");

            var professor = _context.Professors.Where(p => p.Userid.ToString().Equals(userid)).FirstOrDefault();

            return View(professor);
        }

        // GET: Professor's Lessons
        public async Task<IActionResult> Lessons()
        {

            var userid = HttpContext.Session.GetString("userid");

            var professor = _context.Professors.Where(p => p.Userid.ToString().Equals(userid)).FirstOrDefault();

            var lessons = await _context.Courses.Where(c => c.ProfessorId == professor.ProfessorId).ToListAsync();

            return View(lessons);
        }

        // GET: Registered Students of a Course
        public async Task<IActionResult> RegisteredStudents(int? id)
        {
            //TODO: check if the given course belongs to current professor

            var registered_students = await _context.Courses
                .Include(cs => cs.CourseHasStudents)
                .ThenInclude(cs => cs.Student)
                .FirstOrDefaultAsync(c => c.CourseId == id);

            return View(registered_students);
        }

        // GET: Professors
        public async Task<IActionResult> Index()
        {
            var universityDBContext = _context.Professors.Include(p => p.User);
            return View(await universityDBContext.ToListAsync());
        }

        // POST: Professor uploads grades
        [HttpPost]
        public async Task<IActionResult> UploadGrades(int CourseId, IFormFile usercsv)
        {
            string filename = usercsv.FileName;
            filename = Path.GetFileName(filename);

            string uploadfilepath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Files", filename);
            var stream = new FileStream(uploadfilepath, FileMode.Create);
            await usercsv.CopyToAsync(stream);

            stream.Close();

            string _path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Files", filename);

            string[] lines = System.IO.File.ReadAllLines(_path);

            foreach (string line in lines)
            {
                string[] args = line.Split(',');
                Student selectedStudent = _context.Students.Where(s => s.RegistrationNumber == Int32.Parse(args[0])).Single();
                CourseHasStudent courseHasStudent = _context.CourseHasStudents.Where(x => x.CourseId == CourseId && x.StudentId == selectedStudent.StudentId).Single();
                courseHasStudent.Grade = Int32.Parse(args[1]);

                _context.Update(courseHasStudent);
                await _context.SaveChangesAsync();


            }

            return RedirectToAction("RegisteredStudents", new { id = CourseId });
        }

        // GET: Professors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Professors == null)
            {
                return NotFound();
            }

            var professor = await _context.Professors
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.ProfessorId == id);
            if (professor == null)
            {
                return NotFound();
            }

            return View(professor);
        }

        // GET: Professors/Create
        public IActionResult Create()
        {
            ViewData["Userid"] = new SelectList(_context.Users, "Userid", "Userid");
            return View();
        }

        // POST: Professors/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProfessorId,Afm,Name,Surname,Department,Userid")] Professor professor)
        {
            if (ModelState.IsValid)
            {
                _context.Add(professor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Userid"] = new SelectList(_context.Users, "Userid", "Userid", professor.Userid);
            return View(professor);
        }

        // GET: Professors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Professors == null)
            {
                return NotFound();
            }

            var professor = await _context.Professors.FindAsync(id);
            if (professor == null)
            {
                return NotFound();
            }
            ViewData["Userid"] = new SelectList(_context.Users, "Userid", "Userid", professor.Userid);
            return View(professor);
        }

        // POST: Professors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProfessorId,Afm,Name,Surname,Department,Userid")] Professor professor)
        {
            if (id != professor.ProfessorId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(professor);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProfessorExists(professor.ProfessorId))
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
            ViewData["Userid"] = new SelectList(_context.Users, "Userid", "Userid", professor.Userid);
            return View(professor);
        }

        // GET: Professors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Professors == null)
            {
                return NotFound();
            }

            var professor = await _context.Professors
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.ProfessorId == id);
            if (professor == null)
            {
                return NotFound();
            }

            return View(professor);
        }

        // POST: Professors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Professors == null)
            {
                return Problem("Entity set 'UniversityDBContext.Professors'  is null.");
            }
            var professor = await _context.Professors.FindAsync(id);
            if (professor != null)
            {
                _context.Professors.Remove(professor);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProfessorExists(int id)
        {
          return _context.Professors.Any(e => e.ProfessorId == id);
        }
    }
}
