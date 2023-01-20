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


        // GET: Secretary's account welcome
        public async Task<IActionResult> Account()
        {
            var userid = HttpContext.Session.GetString("userid");

            var secretary = _context.Secretaries.Where(s => s.Userid.ToString().Equals(userid)).FirstOrDefault();

            return View(secretary);
        }


        // GET: Secretaries
        public async Task<IActionResult> Index()
        {
            var universityDBContext = _context.Secretaries.Include(s => s.User);
            return View(await universityDBContext.ToListAsync());
        }

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
