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
        public IActionResult Semesters(int? page)
        {
            var student = StudentGetter();

            ViewData["CurrentPage"] = page;
            
            if (page == null)
            {
                ViewData["CurrentPage"] = 1;
            }
            var courses = student.CourseHasStudents.Where(c => c.Course.Semester.ToString() == ViewData["CurrentPage"].ToString());
           

            return View(courses);
        }
        public  Student StudentGetter()
        {
            var id = HttpContext.Session.GetString("userid");

            var current_student = _context.Students.Where(a => a.Userid.ToString().Equals(id)).FirstOrDefault();

            var student = _context.Students.Include(x => x.CourseHasStudents).ThenInclude(x => x.Course)
                .FirstOrDefault(m => m.StudentId == current_student.StudentId);
            
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

        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public IActionResult Home()
        {
            var student = StudentGetter();
            student = _context.Students.Include(x => x.User).Where(a => a.Userid== student.Userid).FirstOrDefault();
            return View(student);
        }

        private bool StudentExists(int id)
        {
          return _context.Students.Any(e => e.StudentId == id);
        }
    }
}
