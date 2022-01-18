using FamilyBudget.CRUD_models;
using FamilyBudget.Data;
using FamilyBudget.Extensions;
using FamilyBudget.ModalViewModels;
using FamilyBudget.Models;
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

        public IActionResult ProjectMemberCreate(int? id)
        {
            var project = _context.Projects
                .FirstOrDefault(p => p.Id == id);

            var projectMemberModel = new ProjectMemberModel();

            if (project == null)
            {

                projectMemberModel.IsNotFound = true;
                return PartialView(projectMemberModel);
            }

            if (!user.CanEdit(project, _context))
            {
                projectMemberModel.IsForbid = true;
                return PartialView(projectMemberModel);
            }

            projectMemberModel.ProjectId = project.Id;
            projectMemberModel.Users = new SelectList(_context.Users, "Id", "UserName");

            return PartialView(projectMemberModel);
        }

        public IActionResult ProjectMemberEdit(int? id)
        {
            var projectMember = _context.ProjectMembers
                .Include(p => p.User)
                .FirstOrDefault(p => p.Id == id);

            var projectMemberModel = new ProjectMemberModel();

            if (projectMember == null)
            {
                projectMemberModel.IsNotFound = true;
                return PartialView(projectMemberModel);
            }

            if (!user.CanEdit(projectMember, _context))
            {
                projectMemberModel.IsForbid = true;
                return PartialView(projectMemberModel);
            }

            projectMemberModel.Id = projectMember.Id;
            projectMemberModel.ProjectId = projectMember.ProjectId;
            projectMemberModel.NameInProject = projectMember.NameInProject;
            projectMemberModel.User = projectMember.User;
            projectMemberModel.Users = new SelectList(_context.Users, "Id", "UserName");

            return PartialView(projectMemberModel);
        }

        public IActionResult ProjectMemberDelete(int? id)
        {
            var projectMember = _context.ProjectMembers
                .FirstOrDefault(p => p.Id == id);

            var projectMemberModel = new ProjectMemberModel();

            if (projectMember == null)
            {
                projectMemberModel.IsNotFound = true;
                return PartialView(projectMemberModel);
            }

            if (!user.CanDelete(projectMember, _context))
            {
                projectMemberModel.IsForbid = true;
                return PartialView(projectMemberModel);
            }

            projectMemberModel.Id = projectMember.Id;
            projectMemberModel.ProjectId = projectMember.ProjectId;
            projectMemberModel.NameInProject = projectMember.NameInProject;

            return PartialView(projectMemberModel);
        }

        public IActionResult CategoryCreate(int? id)
        {
            var project = _context.Projects
                .FirstOrDefault(p => p.Id == id);

            var categoryModel = new CategoryModel();

            if (project == null)
            {

                categoryModel.IsNotFound = true;
                return PartialView(categoryModel);
            }

            if (!user.CanEdit(project, _context))
            {
                categoryModel.IsForbid = true;
                return PartialView(categoryModel);
            }

            categoryModel.ProjectId = project.Id;

            return PartialView(categoryModel);
        }

        public IActionResult CategoryEdit(int? id)
        {
            var category = _context.Categories
                .FirstOrDefault(p => p.Id == id);

            var categoryModel = new CategoryModel();

            if (category == null)
            {
                categoryModel.IsNotFound = true;
                return PartialView(categoryModel);
            }

            if (!user.CanEdit(category, _context))
            {
                categoryModel.IsForbid = true;
                return PartialView(categoryModel);
            }

            categoryModel.Id = category.Id;
            categoryModel.ProjectId = category.ProjectId;
            categoryModel.Name = category.Name;

            return PartialView(categoryModel);
        }

        public IActionResult CategoryDelete(int? id)
        {
            var category = _context.Categories
                .FirstOrDefault(p => p.Id == id);

            var categoryModel = new CategoryModel();

            if (category == null)
            {
                categoryModel.IsNotFound = true;
                return PartialView(categoryModel);
            }

            if (!user.CanDelete(category, _context))
            {
                categoryModel.IsForbid = true;
                return PartialView(categoryModel);
            }

            categoryModel.Id = category.Id;
            categoryModel.ProjectId = category.ProjectId;
            categoryModel.Name = category.Name;

            return PartialView(categoryModel);
        }

        private IdentityUser CurrentUser()
        {
            var username = HttpContext.User.Identity.Name;
            return _context.Users
                .FirstOrDefault(m => m.UserName == username);
        }
    }
}
