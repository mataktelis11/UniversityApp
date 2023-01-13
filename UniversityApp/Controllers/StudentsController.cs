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

        private bool StudentExists(int id)
        {
          return _context.Students.Any(e => e.StudentId == id);
        }
    }
}
