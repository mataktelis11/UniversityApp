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

        // utility function
        public Student StudentGetter()
        {
            var id = HttpContext.Session.GetString("userid");

            var current_student = _context.Students.Where(a => a.Userid.ToString().Equals(id)).FirstOrDefault();

            var student = _context.Students.Include(x => x.CourseHasStudents).ThenInclude(x => x.Course)
                .FirstOrDefault(m => m.StudentId == current_student.StudentId);

            return student;
        }


        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public IActionResult Semesters(int? page)
        {
            if(HttpContext.Session.GetString("userid")==null)
                return View("AuthorizationError");

            if (!(HttpContext.Session.GetString("role").Equals("Students")))
                return View("NoRightsError");


            var student = StudentGetter();

            ViewData["CurrentPage"] = page;
            
            if (page == null)
            {
                ViewData["CurrentPage"] = 1;
            }
            var courses = student.CourseHasStudents.Where(c => c.Course.Semester.ToString() == ViewData["CurrentPage"].ToString());
           

            return View(courses);
        }


        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public IActionResult Grades()
        {
            if (HttpContext.Session.GetString("userid") == null)
                return View("AuthorizationError");

            if (!(HttpContext.Session.GetString("role").Equals("Students")))
                return View("NoRightsError");

            var student = StudentGetter();

            return View(student);
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

            int etcs = passedlessons.Count() * 5;

            int sum = 0;

            foreach(var item in passedlessons)
            {
                sum += (int)item.Grade;
            }

            




            ViewData["reglessons"] = reglessons;
            ViewData["passedlessons"] = passedlessons.Count();
            ViewData["etcs"] = etcs;

            ViewData["average"] = sum / passedlessons.Count();

            //var student = StudentGetter();
            return View();
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

            if(!(HttpContext.Session.GetString("role").Equals("Students")))
                return View("NoRightsError");

            var student = StudentGetter();
            student = _context.Students.Include(x => x.User).Where(a => a.Userid == student.Userid).FirstOrDefault();
            return View(student);


        }

        private bool StudentExists(int id)
        {
          return _context.Students.Any(e => e.StudentId == id);
        }
    }
}
