using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Threading.Tasks;
using Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using UniversityApp.Models;
using X.PagedList;
using static NuGet.Packaging.PackagingConstants;

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
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
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
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> UniversityCourses(int? page, string? sortOrder, string? search)
        {
            if (HttpContext.Session.GetString("userid") == null)
                return View("AuthorizationError");

            if (!(HttpContext.Session.GetString("role").Equals("Secretaries")))
                return View("NoRightsError");

            var courses = _context.Courses.Include(c => c.Professor).Include(c => c.CourseHasStudents).ToList();


            ViewData["CurrentSortOrder"] = String.IsNullOrEmpty(sortOrder) ? "semester" : sortOrder;

            ViewData["TitleSortParam"] = sortOrder == "title" ? "title_desc" : "title";
            ViewData["SemesterSortParam"] = String.IsNullOrEmpty(sortOrder) ? "semester_desc" : "";
            ViewData["StatusSortParam"] = sortOrder == "status" ? "status_desc" : "status";

            ViewData["CurrentFilter"] = search;

            // Search
            if (!String.IsNullOrEmpty(search))
            {
                courses = courses.Where(c => c.Title.ToLower().Contains(search.ToLower())).ToList();
            }

            // Sorting
            switch (sortOrder)
            {
                case "title_desc":
                    courses = courses.OrderByDescending(c => c.Title).ToList();
                    break;

                case "title":
                    courses = courses.OrderBy(c => c.Title).ToList();
                    break;

                case "semester_desc":
                    courses = courses.OrderByDescending(c => c.Semester).ToList();
                    break;

                case "status":
                    courses = courses.OrderBy(c => c.ProfessorId).ToList();
                    break;

                case "status_desc":
                    courses = courses.OrderByDescending(c => c.ProfessorId).ToList();
                    break;

                default:
                    courses = courses.OrderBy(c => c.Semester).ToList();
                    break;
            }


            // Pagination
            if (page != null && page < 1)
            {
                page = 1;
            }
            ViewData["Page"] = page;
            int pageSize = 10;

            var coursesData = courses.ToPagedList(page ?? 1, pageSize);


            return View(coursesData);
        }

        // GET: Secretaries/CreateCourse
        // Form to create a new Course
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public IActionResult CreateCourse()
        {
            List<SelectListItem> professors = new SelectList(_context.Professors, "ProfessorId", "FullnameAFM").ToList();
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
            int courseid = 1;
            if (_context.Courses.Count() > 0)
            {
                courseid = Int32.Parse(_context.Courses.OrderByDescending(s => s.CourseId).FirstOrDefault().CourseId.ToString());
                courseid += 1;
            }


            if (course.ProfessorId== null)
            {
                ModelState.Remove("ProfessorId");
            }

            if (ModelState.IsValid)
            {
                course.CourseId= courseid;

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
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> AssignProfessor(int id)
        {
            if (HttpContext.Session.GetString("userid") == null)
                return View("AuthorizationError");

            if (!(HttpContext.Session.GetString("role").Equals("Secretaries")))
                return View("NoRightsError");

            var course = await _context.Courses.FindAsync(id);

            ViewData["Professorid"] = new SelectList(_context.Professors, "ProfessorId", "FullnameAFM");

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
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> UniversityStudents(int? page, int? pageSize, string? search, string? searchParam, string? sortOrder)
        {
            if (HttpContext.Session.GetString("userid") == null)
                return View("AuthorizationError");

            if (!(HttpContext.Session.GetString("role").Equals("Secretaries")))
                return View("NoRightsError");

            ViewData["CurrentSortOrder"] = String.IsNullOrEmpty(sortOrder) ? "reg" : sortOrder;

            ViewData["RegSortParm"] = String.IsNullOrEmpty(sortOrder) ? "reg_desc" : "";
            ViewData["NameSortParm"] = sortOrder == "name" ? "name_desc" : "name";
            ViewData["SurnameSortParm"] = sortOrder == "surname" ? "surname_desc" : "surname";
            ViewData["DeptSortParm"] = sortOrder == "dept" ? "dept_desc" : "dept";

            ViewData["CurrentFilter"] = search;
            ViewData["searchParam"] = String.IsNullOrEmpty(searchParam) ? "name" : searchParam;

            
            if (pageSize != null && pageSize < 15)
            {
                pageSize = 15;
            }
            else if(pageSize > 15 && pageSize <= 25)
            {
                pageSize = 25;
            }
            else if(pageSize >= 35)
            {
                pageSize= 35;
            }

            ViewData["pageSize"] = pageSize ?? 15;

            var students = from s in _context.Students select s;

            // Search
            if (!String.IsNullOrEmpty(search))
            {
                switch (searchParam)
                {
                    case "name":
                        students = students.Where(e => e.Name.Contains(search));
                        break;

                    case "surname":
                        students = students.Where(e => e.Surname.Contains(search));
                        break;

                    case "dept":
                        students = students.Where(e => e.Department.Contains(search));
                        break;

                    case "reg":
                        students = students.Where(e => e.RegistrationNumber.ToString().Contains(search));
                        break;

                    default:
                        students = students.Where(e => e.Name.Contains(search));
                        break;
                }

            }

            // SortOrder
            switch (sortOrder)
            {
                case "reg_desc":
                    students = students.OrderByDescending(s => s.RegistrationNumber);
                    break;

                case "name_desc":
                    students = students.OrderByDescending(s => s.Name);
                    break;

                case "name":
                    students = students.OrderBy(s => s.Name);
                    break;

                case "surname_desc":
                    students = students.OrderByDescending(s => s.Surname);
                    break;

                case "surname":
                    students = students.OrderBy(s => s.Surname);
                    break;

                case "dept_desc":
                    students = students.OrderByDescending(s => s.Department);
                    break;

                case "dept":
                    students = students.OrderBy(s => s.Department);
                    break;

                default:
                    students = students.OrderBy(s => s.RegistrationNumber);
                    break;
            }


            // Pagination
            if (page != null && page < 1)
            {
                page = 1;
            }

            var studentsData = students.ToPagedList(page ?? 1, pageSize ?? 15);
            ViewData["Page"] = page;

            return View(studentsData);
        }

        // GET: Secretaries/CreateStudent
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public IActionResult CreateStudent()
        {
            if (HttpContext.Session.GetString("userid") == null)
                return View("AuthorizationError");

            if (!(HttpContext.Session.GetString("role").Equals("Secretaries")))
                return View("NoRightsError");

            return View();
        }

        // POST: Students/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateStudent([Bind("RegistrationNumber,Name,Surname,Department,Userid")] Student student)
        {
            int studentid = 1;
            if (_context.Students.Count() > 0)
            {
                studentid = Int32.Parse(_context.Students.OrderByDescending(s => s.StudentId).FirstOrDefault().StudentId.ToString());
                studentid += 1;
            }

            int userid = Int32.Parse(_context.Users.OrderByDescending(s => s.Userid).FirstOrDefault().Userid.ToString());
            userid += 1;


            if (ModelState.IsValid)
            {
                student.StudentId= studentid;
                

                User user = new User();
                user.Userid = userid;
                user.Username = "p" + student.RegistrationNumber.ToString();
                user.Password = student.RegistrationNumber.ToString();
                user.Role = "Students";


                student.User = user;
                _context.Users.Add(user);
                _context.Add(student);

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(UniversityStudents));
            }
            
            return View(student);
        }


        // GET: Secretaries/StudentDetails/6
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> StudentDetails(int? id, int? semester, string? sortOrder)
        {
            if (HttpContext.Session.GetString("userid") == null)
                return View("AuthorizationError");

            if (!(HttpContext.Session.GetString("role").Equals("Secretaries")))
                return View("NoRightsError");

            if (id == null || _context.Students == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .Include(x => x.CourseHasStudents)
                .ThenInclude(x => x.Course)
                .FirstOrDefaultAsync(m => m.StudentId == id);

            if (student == null)
            {
                return NotFound();
            }

            // number of registered lessons
            int registeredlessons = _context.CourseHasStudents.Where(s => s.StudentId == student.StudentId).Count();

            // number of passed lessons
            var passedlessons = _context.CourseHasStudents.Where(s => s.StudentId == student.StudentId && s.Grade >= 5);

            int etcs = passedlessons.Count() * 5;

            ViewData["registered"] = registeredlessons;
            ViewData["etcs"] = etcs;

            // semester
            if(semester == null || semester > 8 || semester < 0 )
                semester= 1;
            ViewData["CurrentSemester"] = semester;

            // sorting
            ViewData["CurrentSortOrder"] = String.IsNullOrEmpty(sortOrder) ? "title" : sortOrder;

            ViewData["TitleSortParam"] = String.IsNullOrEmpty(sortOrder) || sortOrder.Equals("title") ? "title_desc" : "";
            ViewData["SemesterSortParam"] = sortOrder == "semester" ? "semester_desc" : "semester";
            ViewData["GradeSortParam"] = sortOrder == "grade" ? "grade_desc" : "grade";

            return View(student);
        }


        // GET: Secretaries/StudentAssignCourses/6
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> StudentAssignCourses(int? id)
        {
            if (HttpContext.Session.GetString("userid") == null)
                return View("AuthorizationError");

            if (!(HttpContext.Session.GetString("role").Equals("Secretaries")))
                return View("NoRightsError");

            if (id == null || _context.Students == null)
            {
                return NotFound();
            }

            var student = await _context.Students.FirstOrDefaultAsync(m => m.StudentId == id);

            if (student == null)
            {
                return NotFound();
            }

            // get the courses that this Student has registered
            var registeredCourses = from c in _context.CourseHasStudents
                                    where c.StudentId == student.StudentId
                                    select c.Course;
            // get the courses that this Student hasnt registered
            var availableCourses = _context.Courses.Where(course => !registeredCourses.Contains(course));

            ViewData["availableCourses"] = availableCourses.ToList();

            ViewData["check"] = "nones";

            return View(student);
        }

        // GET: Secretaries/StudentAssignCourses/6
        [HttpPost]
        public async Task<IActionResult> StudentAssignCourses(int id, string selectedCourses)
        {
            if (id == null || _context.Students == null)
            {
                return NotFound();
            }

            var student = await _context.Students.FirstOrDefaultAsync(m => m.StudentId == id);

            if (student == null)
            {
                return NotFound();
            }
           
            int gradeId = 1;
            if(_context.CourseHasStudents.Count() > 0)
            {
                gradeId = Int32.Parse(_context.CourseHasStudents.OrderByDescending(chs => chs.GradeId).FirstOrDefault().GradeId.ToString());
                gradeId += 1;
            }

            var s = selectedCourses.Split(' ');

            foreach (string course in selectedCourses.Split(' '))
            {
                if (course.Equals(""))
                    continue;
                int courseid = Int32.Parse(course);

                CourseHasStudent courseNew = new CourseHasStudent();
                courseNew.StudentId = id;
                courseNew.Student = student;
                courseNew.CourseId = courseid;
                courseNew.GradeId = gradeId;
                gradeId++;

                _context.CourseHasStudents.Add(courseNew);
            }


            await _context.SaveChangesAsync();

            //// get the courses that this Student has registered
            //var registeredCourses = from c in _context.CourseHasStudents
            //                        where c.StudentId == student.StudentId
            //                        select c.Course;
            //// get the courses that this Student hasnt registered
            //var availableCourses = _context.Courses.Where(course => !registeredCourses.Contains(course));

            //ViewData["availableCourses"] = availableCourses.ToList();
            //return View(student);

            return RedirectToAction("StudentDetails", new { id = id });
        }



        // GET: Secretaries/UniversityProfessors
        // obtain all the Professors
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> UniversityProfessors(int? page, string? sortOrder, string? search)
        {
            if (HttpContext.Session.GetString("userid") == null)
                return View("AuthorizationError");

            if (!(HttpContext.Session.GetString("role").Equals("Secretaries")))
                return View("NoRightsError");

            var professors = _context.Professors.ToList(); //.OrderBy(p => p.Name).ThenBy(p => p.Surname).ToList();

            ViewData["CurrentSortOrder"] = String.IsNullOrEmpty(sortOrder) ? "fullname" : sortOrder;

            ViewData["FullnameSortParam"] = String.IsNullOrEmpty(sortOrder) ? "fullname_desc" : "";
            ViewData["DepartmentSortParam"] = sortOrder == "department" ? "department_desc" : "department";

            ViewData["CurrentFilter"] = search;

            // Search
            if (!String.IsNullOrEmpty(search))
            {
                professors = professors.Where(c => c.Name.ToLower().Contains(search.ToLower()) || c.Surname.ToLower().Contains(search.ToLower())).Distinct().ToList();
            }

            // Sorting
            switch (sortOrder)
            {
                case "department_desc":
                    professors = professors.OrderByDescending(c => c.Department).ToList();
                    break;

                case "department":
                    professors = professors.OrderBy(c => c.Department).ToList();
                    break;

                case "fullname_desc":
                    professors = professors.OrderByDescending(p => p.Name).ThenByDescending(p => p.Surname).ToList();
                    break;

                default:
                    professors = professors.OrderBy(p => p.Name).ThenBy(p => p.Surname).ToList();
                    break;
            }

            // Pagination
            if (page != null && page < 1)
            {
                page = 1;
            }
            int pageSize = 10;
            var professorsData = professors.ToPagedList(page ?? 1, pageSize);
            ViewData["Page"] = page;

            return View(professorsData);
        }

        // GET: Secretaries/CreateProfessor
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public IActionResult CreateProfessor()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProfessor([Bind("ProfessorId,Afm,Name,Surname,Department,Userid")] Professor professor)
        {
            int professorid = 1;
            if (_context.Professors.Count() > 0)
            {
                professorid = Int32.Parse(_context.Professors.OrderByDescending(p => p.ProfessorId).FirstOrDefault().ProfessorId.ToString());
                professorid += 1;
            }

            int userid = Int32.Parse(_context.Users.OrderByDescending(s => s.Userid).FirstOrDefault().Userid.ToString());
            userid += 1;

            if (ModelState.IsValid)
            {
                professor.ProfessorId= professorid;

                User user = new User();
                user.Userid = userid;
                user.Username = "a" + professor.Afm.ToString();
                user.Password = professor.Afm.ToString();
                user.Role = "Professors";

                professor.User = user;
                _context.Users.Add(user);
                _context.Add(professor);

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(UniversityProfessors));
            }
            ViewData["Userid"] = new SelectList(_context.Users, "Userid", "Userid", professor.Userid);
            return View(professor);
        }



        // GET: Secretaries/ProfessorDetails/6
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> ProfessorDetails(int? id, string? sortOrder)
        {
            if (HttpContext.Session.GetString("userid") == null)
                return View("AuthorizationError");

            if (!(HttpContext.Session.GetString("role").Equals("Secretaries")))
                return View("NoRightsError");


            if (id == null || _context.Professors == null)
            {
                return NotFound();
            }

            var professor = await _context.Professors
                .Include(p => p.Courses)              
                .FirstOrDefaultAsync(m => m.ProfessorId == id);
            if (professor == null)
            {
                return NotFound();
            }

            ViewData["availableCourses"] = new SelectList(_context.Courses.Where(c => c.Professor == null || c.ProfessorId == null), "CourseId", "TitleSemester");

            int registeredCourses = _context.Courses.Where(c => c.ProfessorId == professor.ProfessorId).Count();

            ViewData["registeredCourses"] = registeredCourses;

            // Sorting
            ViewData["CurrentSortOrder"] = String.IsNullOrEmpty(sortOrder) ? "semester" : sortOrder;

            ViewData["TitleSortParam"] = sortOrder == "title" ? "title_desc" : "title";
            ViewData["SemesterSortParam"] = String.IsNullOrEmpty(sortOrder) ? "semester_desc" : "";


            return View(professor);
        }



        [HttpPost]
        public async Task<IActionResult> AssignProfessorCourse(int courseid, int professorid)
        {

            // check if there are no available lessons

            var professor = await _context.Professors
                .Include(p => p.Courses)
                .FirstOrDefaultAsync(m => m.ProfessorId == professorid);
            if (professor == null)
            {
                return NotFound();
            }


            Course course = _context.Courses.Where(c => c.CourseId== courseid).FirstOrDefault();
            course.ProfessorId = professorid;
            _context.Update(course);
            await _context.SaveChangesAsync();

            return RedirectToAction("ProfessorDetails", new { id = professorid });
        }

        private bool SecretaryExists(int id)
        {
          return _context.Secretaries.Any(e => e.SecretaryId == id);
        }
    }
}
