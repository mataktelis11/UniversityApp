using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using UniversityApp.Models;
using X.PagedList;

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
        public async Task<IActionResult> RegisteredStudents(int? id, int? page, int? pageSize, string? search, string? searchParam, string? sortOrder)
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


            ViewData["totalStudents"] = _context.CourseHasStudents.Where(chs => chs.CourseId == id).Count().ToString();

            ViewData["CurrentSortOrder"] = String.IsNullOrEmpty(sortOrder) ? "reg" : sortOrder;

            ViewData["RegSortParm"] = String.IsNullOrEmpty(sortOrder) ? "reg_desc" : "";
            ViewData["NameSortParm"] = sortOrder == "name" ? "name_desc" : "name";
            ViewData["SurnameSortParm"] = sortOrder == "surname" ? "surname_desc" : "surname";
            ViewData["GradeSortParm"] = sortOrder == "grade" ? "grade_desc" : "grade";

            ViewData["CurrentFilter"] = search;
            ViewData["searchParam"] = String.IsNullOrEmpty(searchParam) ? "name" : searchParam;

            //var registered_students = await _context.Courses
            //    .Include(cs => cs.CourseHasStudents)
            //    .ThenInclude(cs => cs.Student)
            //    .FirstOrDefaultAsync(c => c.CourseId == id);

            var registered_students =  _context.CourseHasStudents
                .Where(chs => chs.CourseId == id)
                .Include(chs => chs.Student)
                .ToList();

            ViewData["courseid"] = id;
            ViewData["courseTitle"] = _context.Courses.FirstOrDefault(c => c.CourseId == id).Title;

            // page size
            if (pageSize != null && pageSize < 15)
            {
                pageSize = 15;
            }
            else if (pageSize > 15 && pageSize <= 25)
            {
                pageSize = 25;
            }
            else if (pageSize >= 35)
            {
                pageSize = 35;
            }

            ViewData["pageSize"] = pageSize ?? 15;

            // Search
            if (!String.IsNullOrEmpty(search))
            {
                switch (searchParam)
                {
                    case "name":
                        registered_students = registered_students.Where(e => e.Student.Name.ToLower().Contains(search.ToLower())).ToList();
                        break;

                    case "surname":
                        registered_students = registered_students.Where(e => e.Student.Surname.ToLower().Contains(search.ToLower())).ToList();
                        break;

                    case "reg":
                        registered_students = registered_students.Where(e => e.Student.RegistrationNumber.ToString().Contains(search)).ToList();
                        break;

                    default:
                        registered_students = registered_students.Where(e => e.Student.Name.ToLower().Contains(search.ToLower())).ToList();
                        break;
                }

            }

            // SortOrder
            switch (sortOrder)
            {
                case "reg_desc":
                    registered_students = registered_students.OrderByDescending(s => s.Student.RegistrationNumber).ToList();
                    break;

                case "name_desc":
                    registered_students = registered_students.OrderByDescending(s => s.Student.Name).ToList();
                    break;

                case "name":
                    registered_students = registered_students.OrderBy(s => s.Student.Name).ToList();
                    break;

                case "surname_desc":
                    registered_students = registered_students.OrderByDescending(s => s.Student.Surname).ToList();
                    break;

                case "surname":
                    registered_students = registered_students.OrderBy(s => s.Student.Surname).ToList();
                    break;

                case "grade":
                    registered_students = registered_students.OrderBy(s => s.Grade).ToList();
                    break;

                case "grade_desc":
                    registered_students = registered_students.OrderByDescending(s => s.Grade).ToList();
                    break;

                default:
                    registered_students = registered_students.OrderBy(s => s.Student.RegistrationNumber).ToList();
                    break;
            }


            // Pagination
            if (page != null && page < 1)
            {
                page = 1;
            }
            ViewData["Page"] = page;
            

            var registered_studentsData = registered_students.ToPagedList(page ?? 1, pageSize ?? 15);

            return View(registered_studentsData);
        }

        // POST: Professors/UploadGrades/CourseId/6
        // Professor uploads grades with .csv file for specified course

        [HttpPost]
        public async Task<IActionResult> UploadGrades(int CourseId, IFormFile usercsv)
        {
            if (HttpContext.Session.GetString("userid") == null)
                return View("AuthorizationError");

            if (!(HttpContext.Session.GetString("role").Equals("Professors")))
                return View("NoRightsError");

            if (usercsv == null)
            {
                return RedirectToAction("RegisteredStudents", new { id = CourseId });
            }

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
            List<string> invalidGrades = new List<string>();

            foreach (string line in lines)
            {
                string[] args = line.Split(',');

                if (!_context.Students.Any(s => s.RegistrationNumber == Int32.Parse(args[0])))
                {
                    nonexistingStudents.Add(args[0]);
                    continue;
                }

                if (!_context.CourseHasStudents.Where(chs => chs.CourseId == CourseId).Any(chs => chs.Student.RegistrationNumber == Int32.Parse(args[0])))
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

                int grade = Int32.Parse(args[1]);

                if(grade < 0 || grade > 10)
                {
                    invalidGrades.Add(args[0]);
                    continue;
                }

                courseHasStudent.Grade = grade;
                _context.Update(courseHasStudent);
               
            }

            await _context.SaveChangesAsync();

            if (nonexistingStudents.Count > 0 || invalidStudents.Count > 0)
            {
                ViewData["nonexistingStudents"] = nonexistingStudents;
                ViewData["invalidStudents"] = invalidStudents;                
                ViewData["invalidGrades"] = invalidGrades;
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
