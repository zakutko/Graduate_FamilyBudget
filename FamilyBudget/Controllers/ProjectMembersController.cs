using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FamilyBudget.Data;
using FamilyBudget.Models;
using FamilyBudget.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace FamilyBudget.Controllers
{
    [Authorize]
    public class ProjectMembersController : Controller
    {
        private readonly ApplicationDbContext _context;

        private IdentityUser user { get { return CurrentUser(); } }
        public ProjectMembersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ProjectMembers
        public async Task<IActionResult> Index()
        {
            var all_project_members = await _context.ProjectMembers
                .Include(p => p.Project)
                .Include(p => p.User).ToListAsync();

            var viewable_project_members = all_project_members.Where(x => user.CanView(x, _context)).ToList();
            return View(viewable_project_members);
        }

        // GET: ProjectMembers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var projectMember = await _context.ProjectMembers
                .Include(p => p.Project)
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (projectMember == null)
            {
                return NotFound();
            }

            if (!user.CanView(projectMember, _context))
            {
                return Forbid();
            }

            return View(projectMember);
        }

        // GET: ProjectMembers/Create
        public IActionResult Create()
        {
            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Name");
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: ProjectMembers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,NameInProject,UserId,ProjectId,CreateTime,UpdateTime")] ProjectMember projectMember)
        {
            if (!user.CanEdit(projectMember, _context))
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                var project = _context.Projects.Find(projectMember.ProjectId);
                project.UpdateTime = DateTime.Now;
                _context.Update(project);

                projectMember.CreateTime = DateTime.Now;
                projectMember.UpdateTime = DateTime.Now;
                _context.Add(projectMember);
                await _context.SaveChangesAsync();
                return RedirectToAction("Edit", "Home", new { id = projectMember.ProjectId });
            }
            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Name", projectMember.ProjectId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", projectMember.UserId);
            return View(projectMember);
        }

        // GET: ProjectMembers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var projectMember = await _context.ProjectMembers.FindAsync(id);
            if (projectMember == null)
            {
                return NotFound();
            }

            if (!user.CanEdit(projectMember, _context))
            {
                return Forbid();
            }

            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Name", projectMember.ProjectId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", projectMember.UserId);
            return View(projectMember);
        }

        // POST: ProjectMembers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,NameInProject,UserId,ProjectId,UpdateTime")] ProjectMember projectMember)
        {
            if (!user.CanEdit(projectMember, _context))
            {
                return Forbid();
            }

            if (id != projectMember.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var project = _context.Projects.Find(projectMember.ProjectId);
                    project.UpdateTime = DateTime.Now;
                    _context.Update(project);

                    projectMember.UpdateTime = DateTime.Now;
                    _context.Update(projectMember);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProjectMemberExists(projectMember.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Edit", "Home", new { id = projectMember.ProjectId });
            }
            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Name", projectMember.ProjectId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", projectMember.UserId);
            return View(projectMember);
        }

        // GET: ProjectMembers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var projectMember = await _context.ProjectMembers
                .Include(p => p.Project)
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (projectMember == null)
            {
                return NotFound();
            }

            if (!user.CanDelete(projectMember, _context))
            {
                return Forbid();
            }

            return View(projectMember);
        }

        // POST: ProjectMembers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var projectMember = await _context.ProjectMembers.FindAsync(id);

            var project = await _context.Projects.FindAsync(projectMember.ProjectId);
            project.UpdateTime = DateTime.Now;
            _context.Update(project);

            _context.ProjectMembers.Remove(projectMember);
            await _context.SaveChangesAsync();
            return RedirectToAction("Edit", "Home", new { id = projectMember.ProjectId });
        }

        private bool ProjectMemberExists(int id)
        {
            return _context.ProjectMembers.Any(e => e.Id == id);
        }
        private IdentityUser CurrentUser()
        {
            var username = HttpContext.User.Identity.Name;
            return _context.Users
                .FirstOrDefault(m => m.UserName == username);
        }

        public async Task<IActionResult> SearchProjectMember(int id, string term)
        {
            try
            {
                var categories = await _context.ProjectMembers
                    .Where(a => a.ProjectId == id)
                    .Where(a => a.NameInProject.Contains(term))
                    .Select(a => new { value = a.NameInProject, id = a.Id })
                    .ToListAsync();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
