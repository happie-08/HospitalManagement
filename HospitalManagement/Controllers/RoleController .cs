﻿using HospitalManagement.Data;
using HospitalManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.Controllers
{
    public class RoleController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RoleController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Role
        public IActionResult Index()
        {
            return View();
        }

        // GET: Role/GetRoles (For DataTables AJAX)
        [HttpGet]
        public async Task<IActionResult> GetRoles()
        {
            var roles = await _context.Roles
                .Select(r => new
                {
                    r.Id,
                    r.Name,
                    r.Description,
                    r.Active
                })
                .ToListAsync();

            return Json(new { data = roles });
        }

        // GET: Role/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Role/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Role role)
        {
            if (ModelState.IsValid)
            {
                // ✅ Check for duplicate Role Name
                bool exists = await _context.Roles
                    .AnyAsync(r => r.Name.ToLower() == role.Name.ToLower());

                if (exists)
                {
                    ModelState.AddModelError("Name", "This role name already exists!");
                    return View("Create", role);
                }

                _context.Add(role);
                await _context.SaveChangesAsync();
                TempData["success"] = "Role created successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View("Create", role);
        }

        // GET: Role/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var role = await _context.Roles.FindAsync(id);
            if (role == null)
                return NotFound();

            return View("Create", role); // 👈 use Create view for edit
        }

        // POST: Role/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Role role)
        {
            if (id != role.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                // ✅ Check for duplicate Role Name (excluding current role)
                bool exists = await _context.Roles
                    .AnyAsync(r => r.Name.ToLower() == role.Name.ToLower() && r.Id != role.Id);

                if (exists)
                {
                    ModelState.AddModelError("Name", "This role name already exists!");
                    return View("Create", role);
                }

                try
                {
                    _context.Update(role);
                    await _context.SaveChangesAsync();
                    TempData["success"] = "Role updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Roles.Any(e => e.Id == id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View("Create", role);
        }

        // POST: Role/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var role = await _context.Roles.FindAsync(id);
            if (role == null)
                return Json(new { success = false, message = "Role not found" });

            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Role deleted successfully" });
        }
    }
}
