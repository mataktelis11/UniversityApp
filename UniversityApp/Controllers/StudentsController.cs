using System;
using System.Collections.Generic;
using System.Drawing.Printing;
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


        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("userid") == null)
                return View("AuthorizationError");

            if (!(HttpContext.Session.GetString("role").Equals("Students")))
                return View("NoRightsError");

            return RedirectToAction("Account");
        }

        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public IActionResult Account()
        {

            if (HttpContext.Session.GetString("userid") == null)
                return View("AuthorizationError");

            if (!(HttpContext.Session.GetString("role").Equals("Students")))
                return View("NoRightsError");

            var student = StudentGetter();
            student = _context.Students.Include(x => x.User).Where(a => a.Userid == student.Userid).FirstOrDefault();

            // number of registered lessons
            int reglessons = _context.CourseHasStudents.Where(s => s.StudentId == student.StudentId).Count();

            // number of passed lessons
            var passedlessons = _context.CourseHasStudents.Where(s => s.StudentId == student.StudentId && s.Grade >= 5);

            int ects = passedlessons.Count() * 5;

            ViewData["reglessons"] = reglessons;
            ViewData["ects"] = ects;


            return View(student);
        }

        // utility function
        public Student StudentGetter()
        {
            var id = HttpContext.Session.GetString("userid");

            var current_student = _context.Students.Where(a => a.Userid.ToString().Equals(id)).FirstOrDefault();

            var student = _context.Students
                .Include(x => x.CourseHasStudents)
                .ThenInclude(x => x.Course)
                .FirstOrDefault(m => m.StudentId == current_student.StudentId);

            return student;
        }

        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public IActionResult Grades(string? sortOrder)
        {
            if (HttpContext.Session.GetString("userid") == null)
                return View("AuthorizationError");

            if (!(HttpContext.Session.GetString("role").Equals("Students")))
                return View("NoRightsError");

            var student = StudentGetter();
            
            var courses = student.CourseHasStudents.OrderBy(s => s.Course.Semester);

            ViewData["CurrentSortOrder"] = String.IsNullOrEmpty(sortOrder) ? "title" : sortOrder;
            ViewData["TitleSortOrder"] = String.IsNullOrEmpty(sortOrder) ? "title_desc" : "";
            ViewData["GradesSortOrder"] = sortOrder == "grades" ? "grades_desc" : "grades";

            // Sorting
            switch (sortOrder)
            {
                case "title_desc":
                    courses = courses.ThenByDescending(c => c.Course.Title);
                    break;

                case "grades":
                    courses = courses.ThenBy(c => c.Grade);
                    break;

                case "grades_desc":
                    courses = courses.ThenByDescending(c => c.Grade);
                    break;

                default:
                    courses = courses.ThenBy(c => c.Course.Title);
                    break;
            }

            // number of registered lessons
            int reglessons = _context.CourseHasStudents.Where(s => s.StudentId == student.StudentId).Count();

            // number of passed lessons
            var passedlessons = _context.CourseHasStudents.Where(s => s.StudentId == student.StudentId && s.Grade >= 5);

            int ects = passedlessons.Count() * 5;

            int sum = 0;

            foreach (var item in passedlessons)
            {
                sum += (int)item.Grade;
            }

            if (passedlessons.Count() > 0)
            {
                ViewData["average"] = sum / passedlessons.Count();
            }
            else
            {
                ViewData["average"] = "-";
            }

            ViewData["ects"] = ects;

            return View(courses);
        }

        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public IActionResult Semesters(int? semester)
        {
            if (HttpContext.Session.GetString("userid") == null)
                return View("AuthorizationError");

            if (!(HttpContext.Session.GetString("role").Equals("Students")))
                return View("NoRightsError");

            var student = StudentGetter();

            // semester must be >=1 and <=8
            if (semester == null || semester < 1)
            {
                semester = 1;
            }
            else if (semester > 8)
            {
                semester = 8;
            }

            ViewData["CurrentSemester"] = semester;

            var courses = student.CourseHasStudents
                .Where(c => c.Course.Semester == semester)
                .OrderBy(c => c.Course.Title);

            return View(courses);
        }

        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public IActionResult Total()
        {
            if (HttpContext.Session.GetString("userid") == null)
                return View("AuthorizationError");

            if (!(HttpContext.Session.GetString("role").Equals("Students")))
                return View("NoRightsError");

            var student = StudentGetter();

            // number of registered lessons
            int reglessons = _context.CourseHasStudents.Where(s => s.StudentId == student.StudentId).Count();

            // number of passed lessons
            var passedlessons = _context.CourseHasStudents.Where(s => s.StudentId == student.StudentId && s.Grade >= 5);

            int ects = passedlessons.Count() * 5;

            int sum = 0;

            foreach(var item in passedlessons)
            {
                sum += (int)item.Grade;
            }

            ViewData["reglessons"] = reglessons;
            ViewData["passedlessons"] = passedlessons.Count();
            ViewData["ects"] = ects;
            if (passedlessons.Count()>0){
                ViewData["average"] = sum / passedlessons.Count();
            }
            else
            {
                ViewData["average"] = "-";
            }
            
            return View();
        }



        private bool StudentExists(int id)
        {
          return _context.Students.Any(e => e.StudentId == id);
        }
    }
}
