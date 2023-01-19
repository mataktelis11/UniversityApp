﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using UniversityApp.Models;

namespace UniversityApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private UniversityDBContext _context;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }




        [HttpPost]
        public ActionResult Login(User user)
        {
            //if (ModelState.IsValid)
            //{
                _context = new UniversityDBContext();
                var obj = _context.Users.Where(a => a.Username.Equals(user.Username) && a.Password.Equals(user.Password)).FirstOrDefault();
                if (obj != null)
                {
                    HttpContext.Session.SetString("username", obj.Username.ToString());
                    HttpContext.Session.SetString("userid", obj.Userid.ToString());
                    HttpContext.Session.SetString("role",obj.Role.ToString());
                //return RedirectToAction("Index");
                return RedirectToAction("Home","Students");
                }
            //}
            return RedirectToAction("Login");
        }

        public ActionResult Logout()
        {
            HttpContext.Session.Clear();
            HttpContext.Session.Remove("username");
            return RedirectToAction("Login");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}