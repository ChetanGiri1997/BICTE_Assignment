using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EmployeeManagementSystem.Data;
using EmployeeManagementSystem.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagementSystem.Controllers
{
    [Authorize]
    public class EmployeeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EmployeeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Employee
        public async Task<IActionResult> Index()
        {
            var employees = await _context.Employees
                .Include(e => e.Qualifications)
                .ToListAsync();
            Console.WriteLine($"Employees loaded: {employees.Count}");
            return View(employees);
        }

        // GET: Employee/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Employee/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Employee employee)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(employee);
                    await _context.SaveChangesAsync();
                    Console.WriteLine($"Employee created: {employee.Name}, Id: {employee.Id}");
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error creating employee: {ex.Message}");
                    ModelState.AddModelError("", "An error occurred while creating the employee.");
                }
            }
            else
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine($"Validation error: {error.ErrorMessage}");
                }
            }
            return View(employee);
        }

        // GET: Employee/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }
            return View(employee);
        }

        // POST: Employee/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Employee employee)
        {
            if (id != employee.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(employee);
                    await _context.SaveChangesAsync();
                    Console.WriteLine($"Employee updated: {employee.Name}, Id: {employee.Id}");
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeExists(employee.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error updating employee: {ex.Message}");
                    ModelState.AddModelError("", "An error occurred while updating the employee.");
                }
            }
            else
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine($"Validation error: {error.ErrorMessage}");
                }
            }
            return View(employee);
        }

        // GET: Employee/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .Include(e => e.Qualifications)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employee == null)
            {
                return NotFound();
            }
            return View(employee);
        }

        // POST: Employee/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee != null)
            {
                try
                {
                    _context.Employees.Remove(employee);
                    await _context.SaveChangesAsync();
                    Console.WriteLine($"Employee deleted: Id: {id}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error deleting employee: {ex.Message}");
                    ModelState.AddModelError("", "An error occurred while deleting the employee.");
                    return View(employee);
                }
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Employee/AddQualification/5
        public IActionResult AddQualification(int id)
        {
            var employee = _context.Employees.Find(id);
            if (employee == null)
            {
                return NotFound();
            }
            // Initialize Course to satisfy required property
            var qualification = new Qualification { EmployeeId = id, Course = "" };
            ViewBag.EmployeeId = id;
            return View(qualification);
        }

        // POST: Employee/AddQualification
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddQualification(Qualification qualification)
        {
            // Log the incoming qualification
            Console.WriteLine($"Qualification - Id: {qualification.Id}, EmployeeId: {qualification.EmployeeId}, Course: {qualification.Course}, YearPassed: {qualification.YearPassed}, MarksPercentage: {qualification.MarksPercentage}");

            if (ModelState.IsValid)
            {
                // Ensure Id is not set (let SQL Server auto-generate)
                qualification.Id = 0;

                try
                {
                    _context.Add(qualification);
                    await _context.SaveChangesAsync();
                    Console.WriteLine("Qualification inserted successfully.");
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error inserting qualification: {ex.Message}");
                    ModelState.AddModelError("", "An error occurred while saving the qualification.");
                }
            }
            else
            {
                // Log validation errors
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine($"Validation error: {error.ErrorMessage}");
                }
            }

            ViewBag.EmployeeId = qualification.EmployeeId;
            return View(qualification);
        }

        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.Id == id);
        }
    }
}