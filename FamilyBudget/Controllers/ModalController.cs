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
            projectDelete.membersCount = project.ProjectMembers.Count();
            projectDelete.incomesCount = project.FinOperations.Where(fo => fo.FinType == FinType.Income).Count();
            projectDelete.chargesCount = project.FinOperations.Where(fo => fo.FinType == FinType.Charge).Count();

            return PartialView(projectDelete);
        }

        public IActionResult AddCharge(int? id)
        {
            var addCharge = new AddFinOpModel();

            var project = _context.Projects
                .Include(c => c.ProjectMembers)
                .Include(c => c.FinOperations)
                .FirstOrDefault(p => p.Id == id);

            if (project == null)
            {
                addCharge.IsNotFound = true;
                return PartialView(addCharge);
            }

            if (!user.CanEdit(project, _context))
            {
                addCharge.IsForbid = true;
                return PartialView(addCharge);
            }

            addCharge.ForAll = false;
            addCharge.FinType = FinType.Charge;
            addCharge.ProjectId = project.Id;

            var projectMembers = project.ProjectMembers.ToList();
            projectMembers.Insert(0, new ProjectMember { NameInProject = "Семья", Id= -1, ProjectId = project.Id });
            addCharge.ProjectMembers = new SelectList(projectMembers, "Id", "NameInProject");

            return PartialView(addCharge);
        }

        public IActionResult AddIncome(int? id)
        {
            var addIncome = new AddFinOpModel();

            var project = _context.Projects
                .Include(c => c.ProjectMembers)
                .Include(c => c.FinOperations)
                .FirstOrDefault(p => p.Id == id);

            if (project == null)
            {
                addIncome.IsNotFound = true;
                return PartialView(addIncome);
            }

            if (!user.CanEdit(project, _context))
            {
                addIncome.IsForbid = true;
                return PartialView(addIncome);
            }

            addIncome.ForAll = false;
            addIncome.FinType = FinType.Income;
            addIncome.ProjectId = project.Id;
            addIncome.CategoryId = _context.Categories.FirstOrDefault().Id; //??

            var projectMembers = project.ProjectMembers.ToList();
            projectMembers.Insert(0, new ProjectMember { NameInProject = "Семья", Id = -1, ProjectId = project.Id });
            addIncome.ProjectMembers = new SelectList(projectMembers, "Id", "NameInProject");

            return PartialView(addIncome);
        }

        public IActionResult ProjectMemberCreate()
        {
            var projectMemberModel = new ProjectMemberModel();
            return PartialView(projectMemberModel);
        }

        public IActionResult ProjectMemberEdit(int? id)
        {
            var projectMemberModel = new ProjectMemberModel();
            return PartialView(projectMemberModel);
        }

        public IActionResult ProjectMemberDelete(int? id)
        {
            var projectMemberModel = new ProjectMemberModel();
            return PartialView(projectMemberModel);
        }

        public IActionResult CategoryCreate()
        {
            var categoryModel = new CategoryModel();
            return PartialView(categoryModel);
        }

        public IActionResult CategoryEdit(int? id)
        {
            var categoryModel = new CategoryModel();
            return PartialView(categoryModel);
        }

        public IActionResult CategoryDelete(int? id)
        {
            var categoryModel = new CategoryModel();
            return PartialView(categoryModel);
        }

        public IActionResult ProjectCreate()
        {
            ViewData["OwnerId"] = new SelectList(_context.Users, "Id", "Id");
            return PartialView();
        }

        private IdentityUser CurrentUser()
        {
            var username = HttpContext.User.Identity.Name;
            return _context.Users
                .FirstOrDefault(m => m.UserName == username);
        }
    }
}
