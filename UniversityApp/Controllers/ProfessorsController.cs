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

            var courses = _context.Courses.Where(c => c.ProfessorId == professor.ProfessorId);

            ViewData["numberCourses"] = courses.Count().ToString();

            ViewData["numberOfNonEmptyCourses"] = courses
                .Where(c => c.CourseHasStudents.Count() > 0)
                .Count().ToString();

            ViewData["numberCoursesReq"] = courses
                .Where(c => c.CourseHasStudents.Count() > 0)
                .Where(c => c.CourseHasStudents.Where(chs => chs.Grade ==null).Count() > 0)
                .Count().ToString();

            return View(professor);
        }

        // GET: Professors/ProfessorCourses
        // Retrieve the lessons of the logged in professor.
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> ProfessorCourses(string? sortOrder)
        {
            if (HttpContext.Session.GetString("userid") == null)
                return View("AuthorizationError");

            if (!(HttpContext.Session.GetString("role").Equals("Professors")))
                return View("NoRightsError");

            var userid = HttpContext.Session.GetString("userid");
            var professor = _context.Professors.Where(p => p.Userid.ToString().Equals(userid)).FirstOrDefault();

            var lessons = await _context.Courses.Where(c => c.ProfessorId == professor.ProfessorId).Include(c => c.CourseHasStudents).ToListAsync();

            ViewData["CurrentSortOrder"] = String.IsNullOrEmpty(sortOrder) ? "title" : sortOrder;

            ViewData["TitleSortParam"] = String.IsNullOrEmpty(sortOrder) ? "title_desc" : "";
            ViewData["SemesterSortParam"] = sortOrder == "semester" ? "semester_desc" : "semester";
            ViewData["StudentsSortParam"] = sortOrder == "students" ? "students_desc" : "students";

            switch (sortOrder)
            {
                case "title_desc":
                    lessons = lessons.OrderByDescending(c => c.Title).ToList();
                    break;

                case "semester":
                    lessons = lessons.OrderBy(c => c.Semester).ToList();
                    break;

                case "semester_desc":
                    lessons = lessons.OrderByDescending(c => c.Semester).ToList();
                    break;

                case "students":
                    lessons = lessons.OrderBy(c => c.CourseHasStudents.Where(c => c.Grade == null).Count()).ToList();
                    break;

                case "students_desc":
                    lessons = lessons.OrderByDescending(c => c.CourseHasStudents.Where(c => c.Grade == null).Count()).ToList();
                    break;

                default:
                    lessons = lessons.OrderBy(c => c.Title).ToList();
                    break;
            }



            return View(lessons);
        }

        // GET: Professors/RegisteredStudents/6
        // Retrieve Registered Students of a Course.
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
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
            // add checks auth

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

        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
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
            ViewData["courseTitle"] = _context.Courses.Where(c => c.CourseId == id).FirstOrDefault().Title;

            var unregisteredGrades = _context.CourseHasStudents
                .Where(chs => chs.CourseId == id && chs.Grade == null)
                .Include(chs => chs.Student)
                .OrderBy(chs => chs.Student.RegistrationNumber);

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

            if(addedGrades == null)
                return RedirectToAction("RegisteredStudents", new { id = id });

            addedGrades = addedGrades.Trim();

            foreach (string element in addedGrades.Split(' '))
            {
                var data = element.Split('-');
                int gradeId = int.Parse(data[0]);
                int grade = int.Parse(data[1]);

                if (grade < 0)
                    grade = 0;
                if (grade > 10)
                    grade = 10;

                var chs = await _context.CourseHasStudents.FindAsync(gradeId);

                chs.Grade = grade;

                _context.Update(chs);
            }

            await _context.SaveChangesAsync();

/*            var userid = HttpContext.Session.GetString("userid");
            var professor = await _context.Professors.Where(p => p.Userid.ToString().Equals(userid)).FirstOrDefaultAsync();*/

            return RedirectToAction("RegisteredStudents", new { id = id });
        }

        private bool ProfessorExists(int id)
        {
          return _context.Professors.Any(e => e.ProfessorId == id);
        }
    }
}
