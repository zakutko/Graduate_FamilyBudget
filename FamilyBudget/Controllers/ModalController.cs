using FamilyBudget.CRUD_models;
using FamilyBudget.Data;
using FamilyBudget.Extensions;
using FamilyBudget.ModalViewModels;
using FamilyBudget.Models;
using FamilyBudget.Models.View.Modal;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyBudget.Controllers
{
    public class ModalController : Controller
    {

        private readonly ApplicationDbContext _context;

        private IdentityUser user { get { return CurrentUser(); } }
        public ModalController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult ProjectDelete(int? id)
        {
            var projectDelete = new ProjectDeleteModel();

            var project = _context.Projects
                .Include(c => c.ProjectMembers)
                .Include(c => c.FinOperations)
                .FirstOrDefault(p => p.Id == id);

            if (project == null)
            {
                projectDelete.IsNotFound = true;
                return PartialView(projectDelete);
            }

            if (!user.CanDelete(project, _context))
            {
                projectDelete.IsForbid = true;
                return PartialView(projectDelete);
            }

            projectDelete.Id = project.Id;
            projectDelete.Name = project.Name;
            projectDelete.Owner = project.Owner;
            projectDelete.CreateTime = project.CreateTime;
            projectDelete.UpdateTime = project.UpdateTime;
            projectDelete.MembersCount = project.ProjectMembers.Count();
            projectDelete.IncomesCount = project.FinOperations.Where(fo => fo.FinType == FinType.Income).Count();
            projectDelete.ChargesCount = project.FinOperations.Where(fo => fo.FinType == FinType.Charge).Count();

            return PartialView(projectDelete);
        }

        public IActionResult AddFinOperation(int? id, FinType type)
        {
            var addFinOperation = new AddFinOpModel();

            var project = _context.Projects
                .Include(c => c.ProjectMembers)
                .Include(c => c.FinOperations)
                .FirstOrDefault(p => p.Id == id);

            if (project == null)
            {
                addFinOperation.IsNotFound = true;
                return PartialView(addFinOperation);
            }

            if (!user.CanEdit(project, _context))
            {
                addFinOperation.IsForbid = true;
                return PartialView(addFinOperation);
            }

            addFinOperation.ForAll = false;
            addFinOperation.FinType = type;
            addFinOperation.ProjectId = project.Id;

            var projectMembers = project.ProjectMembers.OrderBy(x => x.NameInProject != "Семья").ToList();
            addFinOperation.ProjectMembers = new SelectList(projectMembers, "Id", "NameInProject");

            return PartialView(addFinOperation);
        }

        public IActionResult ProjectCreate()
        {
            ViewData["OwnerId"] = new SelectList(_context.Users, "Id", "Id");
            return PartialView();
        }

        public IActionResult FinOperationDelete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var finOperation = _context.FinOperations
                .Include(f => f.Category)
                .Include(f => f.Project)
                .Include(f => f.ProjectMember)
                .FirstOrDefault(m => m.Id == id.Value);

            var project = _context.Projects
                .Include(c => c.ProjectMembers)
                .Include(c => c.FinOperations)
                .FirstOrDefault(p => p.FinOperations.Contains(finOperation));

            if (!user.CanDelete(finOperation, _context))
            {
                return PartialView(new FinOperationModal { IsForbid = true });
            }

            if (finOperation == null)
            {
                return PartialView(new FinOperationModal { IsNotFound = true });
            }

            var finOperationModal = new FinOperationModal
            {
                Id = finOperation.Id,
                FinType = finOperation.FinType,
                ForAll = finOperation.ForAll,
                Value = finOperation.Value,
                CategoryId = finOperation.CategoryId,
                Category = finOperation.Category,
                ProjectMember = finOperation.ProjectMember,
                ProjectMemberId = finOperation.ProjectMemberId,
                ProjectId = finOperation.ProjectId,
                Project = finOperation.Project,
                CreateTime = finOperation.CreateTime,
                UpdateTime = finOperation.UpdateTime,
            };

            var projectMembers = project.ProjectMembers.ToList();
            finOperationModal.ProjectMembers = new SelectList(projectMembers, "Id", "NameInProject");

            return PartialView(finOperationModal);
        }
        public IActionResult FinOperationEdit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var finOperation = _context.FinOperations
                .Include(f => f.Category)
                .Include(f => f.Project)
                .Include(f => f.ProjectMember)
                .FirstOrDefault(m => m.Id == id.Value);
            var project = _context.Projects
                .Include(c => c.ProjectMembers)
                .Include(c => c.FinOperations)
                .FirstOrDefault(p => p.FinOperations.Contains(finOperation));

            if (!user.CanDelete(finOperation, _context))
            {
                return PartialView(new FinOperationModal { IsForbid = true });
            }

            if (finOperation == null)
            {
                return PartialView(new FinOperationModal { IsNotFound = true });
            }

            var finOperationModal = new FinOperationModal
            {
                Id = finOperation.Id,
                FinType = finOperation.FinType,
                ForAll = finOperation.ForAll,
                Value = finOperation.Value,
                CategoryId = finOperation.CategoryId,
                Category = finOperation.Category,
                ProjectMember = finOperation.ProjectMember,
                ProjectMemberId = finOperation.ProjectMemberId,
                ProjectId = finOperation.ProjectId,
                Project = finOperation.Project,
                CreateTime = finOperation.CreateTime,
                UpdateTime = finOperation.UpdateTime,
            };

            var projectMembers = project.ProjectMembers.ToList();
            finOperationModal.ProjectMembers = new SelectList(projectMembers, "Id", "NameInProject");

            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", finOperationModal.CategoryId);
            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Name", finOperationModal.ProjectId);
            ViewData["ProjectMemberId"] = new SelectList(_context.ProjectMembers, "Id", "NameInProject", finOperationModal.ProjectMemberId);
            return PartialView(finOperationModal);
        }
        private IdentityUser CurrentUser()
        {
            var username = HttpContext.User.Identity.Name;
            return _context.Users
                .FirstOrDefault(m => m.UserName == username);
        }
    }
}
