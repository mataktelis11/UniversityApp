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
    public class SecretariesController : Controller
    {
        private readonly UniversityDBContext _context;

        public SecretariesController(UniversityDBContext context)
        {
            _context = context;
        }

        // GET: Secretaries
        // Redirect to wellcome page
        public async Task<IActionResult> Index()
        {
            return await Task.Run<ActionResult>(() => RedirectToAction("Account"));
        }


        // GET: Secretaries/Account
        // Secretay's wellcome page
        public async Task<IActionResult> Account()
        {
            if (HttpContext.Session.GetString("userid") == null)
                return View("AuthorizationError");

            if (!(HttpContext.Session.GetString("role").Equals("Secretaries")))
                return View("NoRightsError");

            var userid = HttpContext.Session.GetString("userid");
            var secretary = await _context.Secretaries.Where(s => s.Userid.ToString().Equals(userid)).FirstOrDefaultAsync();

            return View(secretary);
        }

        // GET: Secretaries/UniversityCourses
        // Retrieve all the courses
        public async Task<IActionResult> UniversityCourses()
        {
            if (HttpContext.Session.GetString("userid") == null)
                return View("AuthorizationError");

            if (!(HttpContext.Session.GetString("role").Equals("Secretaries")))
                return View("NoRightsError");

            var universityDBcontext = _context.Courses.Include(c => c.Professor).OrderBy( c => c.Semester);
            return View(await universityDBcontext.ToListAsync());
        }

        // GET: Secretaries/CreateCourse
        // Form to create a new Course
        public IActionResult CreateCourse()
        {
            List<SelectListItem> professors = new SelectList(_context.Professors, "ProfessorId", "ProfessorId").ToList();
            professors.Insert(0, (new SelectListItem { Text = "[None]" }));
            ViewData["ProfessorId"] = professors;
            return View();
        }

        // POST: Courses/CreateCourse
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCourse([Bind("CourseId,Title,Semester,ProfessorId")] Course course)
        {          
            if(course.ProfessorId== null)
            {
                ModelState.Remove("ProfessorId");
            }
            if (ModelState.IsValid)
            {
                _context.Add(course);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(UniversityCourses));
            }

            List<SelectListItem> professors = new SelectList(_context.Professors, "ProfessorId", "ProfessorId").ToList();
            professors.Insert(0, (new SelectListItem { Text = "[None]" }));
            ViewData["ProfessorId"] = professors;
            return View(course);
        }

        // GET: Secretaries/UniversityCourses/6
        // Form to assign a professor for a given Course
        public async Task<IActionResult> AssignProfessor(int id)
        {
            if (HttpContext.Session.GetString("userid") == null)
                return View("AuthorizationError");

            if (!(HttpContext.Session.GetString("role").Equals("Secretaries")))
                return View("NoRightsError");

            var course = await _context.Courses.FindAsync(id);

            ViewData["Professorid"] = new SelectList(_context.Professors, "ProfessorId", "ProfessorId");

            return View(course);
        }

        // POST: Secretaries/AssignProfessor/6
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignProfessor(int id, [Bind("CourseId,Title,Semester,ProfessorId")] Course course)
        {
            if (id != course.CourseId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(course);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    var length = await _context.Courses.Where(c => c.CourseId == course.CourseId).ToListAsync();

                    if (length.Count <1)
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(UniversityCourses));
            }
            ViewData["ProfessorId"] = new SelectList(_context.Professors, "ProfessorId", "ProfessorId", course.ProfessorId);
            return View(course);
        }


        //

        // GET: Secretaries/UniversityStudents
        // obtain all the students
        public async Task<IActionResult> UniversityStudents()
        {
            var UniversityDBContext = _context.Students;
            return View(await UniversityDBContext.ToListAsync());
        }

        // GET: Secretaries/UniversityProfessors
        // obtain all the Professors
        public async Task<IActionResult> UniversityProfessors()
        {
            var UniversityDBContext = _context.Professors;
            return View(await UniversityDBContext.ToListAsync());
        }

        // bellow are the build in methods


        // GET: Secretaries/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Secretaries == null)
            {
                return NotFound();
            }

            var secretary = await _context.Secretaries
                .Include(s => s.User)
                .FirstOrDefaultAsync(m => m.SecretaryId == id);
            if (secretary == null)
            {
                return NotFound();
            }

            return View(secretary);
        }

        // GET: Secretaries/Create
        public IActionResult Create()
        {
            ViewData["Userid"] = new SelectList(_context.Users, "Userid", "Userid");
            return View();
        }

        // POST: Secretaries/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SecretaryId,Name,Surname,Department,Userid")] Secretary secretary)
        {
            if (ModelState.IsValid)
            {
                _context.Add(secretary);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Userid"] = new SelectList(_context.Users, "Userid", "Userid", secretary.Userid);
            return View(secretary);
        }

        // GET: Secretaries/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Secretaries == null)
            {
                return NotFound();
            }

            var secretary = await _context.Secretaries.FindAsync(id);
            if (secretary == null)
            {
                return NotFound();
            }
            ViewData["Userid"] = new SelectList(_context.Users, "Userid", "Userid", secretary.Userid);
            return View(secretary);
        }

        // POST: Secretaries/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SecretaryId,Name,Surname,Department,Userid")] Secretary secretary)
        {
            if (id != secretary.SecretaryId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(secretary);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SecretaryExists(secretary.SecretaryId))
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
            ViewData["Userid"] = new SelectList(_context.Users, "Userid", "Userid", secretary.Userid);
            return View(secretary);
        }

        // GET: Secretaries/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Secretaries == null)
            {
                return NotFound();
            }

            var secretary = await _context.Secretaries
                .Include(s => s.User)
                .FirstOrDefaultAsync(m => m.SecretaryId == id);
            if (secretary == null)
            {
                return NotFound();
            }

            return View(secretary);
        }

        // POST: Secretaries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Secretaries == null)
            {
                return Problem("Entity set 'UniversityDBContext.Secretaries'  is null.");
            }
            var secretary = await _context.Secretaries.FindAsync(id);
            if (secretary != null)
            {
                _context.Secretaries.Remove(secretary);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SecretaryExists(int id)
        {
          return _context.Secretaries.Any(e => e.SecretaryId == id);
        }
    }
}
