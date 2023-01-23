using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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

        // GET: Professors
        // redirect to wellcome page
        public async Task<IActionResult> Index()
        {
            return await Task.Run<ActionResult>(() => RedirectToAction("Account"));
        }

        // GET: Professors/Account
        // Welcome page of Professor-role user who logged in.
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> Account()
        {
            if (HttpContext.Session.GetString("userid") == null)
                return View("AuthorizationError");

            if (!(HttpContext.Session.GetString("role").Equals("Professors")))
                return View("NoRightsError");

            var userid = HttpContext.Session.GetString("userid");
            var professor = await _context.Professors.Where(p => p.Userid.ToString().Equals(userid)).FirstOrDefaultAsync();

            return View(professor);
        }

        // GET: Professors/Lessons
        // Retrieve the lessons of the logged in professor.
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> Lessons()
        {
            if (HttpContext.Session.GetString("userid") == null)
                return View("AuthorizationError");

            if (!(HttpContext.Session.GetString("role").Equals("Professors")))
                return View("NoRightsError");

            var userid = HttpContext.Session.GetString("userid");
            var professor = _context.Professors.Where(p => p.Userid.ToString().Equals(userid)).FirstOrDefault();

            var lessons = await _context.Courses.Where(c => c.ProfessorId == professor.ProfessorId).ToListAsync();

            return View(lessons);
        }

        // GET: Professors/RegisteredStudents/6
        // Retrieve Registered Students of a Course.
        public async Task<IActionResult> RegisteredStudents(int? id)
        {
            if (HttpContext.Session.GetString("userid") == null)
                return View("AuthorizationError");

            if (!(HttpContext.Session.GetString("role").Equals("Professors")))
                return View("NoRightsError");

            var userid = HttpContext.Session.GetString("userid");
            var professor = _context.Professors.Where(p => p.Userid.ToString().Equals(userid)).FirstOrDefault();

            var course = await _context.Courses.FirstOrDefaultAsync(c => c.CourseId == id && c.ProfessorId == professor.ProfessorId);

            if (course == null)
                return View("NoRightsError");

            var registered_students = await _context.Courses
                .Include(cs => cs.CourseHasStudents)
                .ThenInclude(cs => cs.Student)
                .FirstOrDefaultAsync(c => c.CourseId == id);

            return View(registered_students);
        }

        // POST: Professors/UploadGrades/CourseId/6
        // Professor uploads grades with .csv file for specified course

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

            List<string> nonexistingStudents = new List<string>();
            List<string> invalidStudents = new List<string>();

            foreach (string line in lines)
            {
                string[] args = line.Split(',');

                if (!_context.Students.Any(s => s.RegistrationNumber == Int32.Parse(args[0])))
                {
                    nonexistingStudents.Add(args[0]);
                    continue;
                }

                Student selectedStudent = _context.Students.Where(s => s.RegistrationNumber == Int32.Parse(args[0])).Single();
                   
                CourseHasStudent courseHasStudent = _context.CourseHasStudents.Where(x => x.CourseId == CourseId && x.StudentId == selectedStudent.StudentId).Single();

                if (courseHasStudent.Grade != null)
                {
                    invalidStudents.Add(args[0]);
                    continue;
                }                   
                courseHasStudent.Grade = Int32.Parse(args[1]);
                _context.Update(courseHasStudent);
               
            }

            await _context.SaveChangesAsync();

            if (nonexistingStudents.Count > 0 || invalidStudents.Count > 0)
            {
                ViewData["nonexistingStudents"] = nonexistingStudents;
                ViewData["invalidStudents"] = invalidStudents;
                ViewData["CourseId"] = CourseId;
                return View("ResultReport");
            }

            
            return RedirectToAction("RegisteredStudents", new { id = CourseId });
        }

        public async Task<IActionResult> EditGrades(int id)
        {
            if (HttpContext.Session.GetString("userid") == null)
                return View("AuthorizationError");

            if (!(HttpContext.Session.GetString("role").Equals("Professors")))
                return View("NoRightsError");

            // add NoRightsError

            var userid = HttpContext.Session.GetString("userid");
            var professor = await _context.Professors.Where(p => p.Userid.ToString().Equals(userid)).FirstOrDefaultAsync();

            ViewData["courseId"] = id;

            var unregisteredGrades = _context.CourseHasStudents.Where(chs => chs.CourseId == id && chs.Grade == null).Include(chs => chs.Student);

            return View(unregisteredGrades);
        }

        [HttpPost]
        public async Task<IActionResult> EditGrades(int id, string addedGrades)
        {
            if (HttpContext.Session.GetString("userid") == null)
                return View("AuthorizationError");

            if (!(HttpContext.Session.GetString("role").Equals("Professors")))
                return View("NoRightsError");

            // add NoRightsError
            // check if addedGrades is empty

            foreach (string element in addedGrades.Split(' '))
            {
                var data = element.Split('-');
                int gradeId = int.Parse(data[0]);
                int grade = int.Parse(data[1]);

                var chs = await _context.CourseHasStudents.FindAsync(gradeId);

                chs.Grade = grade;

                _context.Update(chs);
            }

            await _context.SaveChangesAsync();

/*            var userid = HttpContext.Session.GetString("userid");
            var professor = await _context.Professors.Where(p => p.Userid.ToString().Equals(userid)).FirstOrDefaultAsync();*/

            return RedirectToAction("RegisteredStudents", new { id = id });
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
