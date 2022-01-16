using FamilyBudget.Data;
using FamilyBudget.Extensions;
using FamilyBudget.Models;
using FamilyBudget.CRUD_models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using FamilyBudget.Models.View.Enums;
using FamilyBudget.Models.View.Details;
using FamilyBudget.Models.View.Sort;
using FamilyBudget.Models.View.Page;
using FamilyBudget.Models.View.Filter;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FamilyBudget.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly SignInManager<IdentityUser> _signInManager;

        private readonly ApplicationDbContext _context;

        private IdentityUser user { get { return CurrentUser(); } }
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context,
            SignInManager<IdentityUser> signInManager)
        {
            _logger = logger;
            _context = context;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> Index()
        {
            var all_projects = await _context.Projects.Include(p => p.Owner).ToListAsync();
            var viewable_projects = all_projects.Where(x => user.CanView(x, _context)).ToList();

            return View(viewable_projects);
        }

        public async Task<IActionResult> Details(int? id, string category, string projectMember, FinType? finType, DateTime? beginDate, DateTime? endDate, int page = 1, 
            FinOperationSortModelEnum sortModel = FinOperationSortModelEnum.Default,
            FinOperationSortDirEnum sortDir = FinOperationSortDirEnum.Ascending)
        {
            var project = _context.Projects
                .FirstOrDefault(p => p.Id == id);

            if (project == null)
            {
                return NotFound();
            }

            if (!user.CanView(project, _context))
            {
                return Forbid();
            }

            #region Фильтрация

            IQueryable<FinOperation> finOperations = _context.FinOperations
                .Include(x => x.Project)
                .Include(x => x.Category)
                .Include(x => x.ProjectMember);

            finOperations = finOperations
                .Where(p => p.ProjectId == id);

            if (beginDate != null)
            {
                finOperations = finOperations
                    .Where(p => p.CreateTime >= beginDate);
            }

            if (endDate != null)
            {
                finOperations = finOperations
                    .Where(p => p.CreateTime <= endDate);
            }

            if (finType != null)
            {
                finOperations = finOperations
                    .Where(p => p.FinType == finType);
            }

            if (!String.IsNullOrEmpty(category))
            {
                finOperations = finOperations.Where(p => p.Category.Name.Contains(category));
            }

            if (!String.IsNullOrEmpty(projectMember) && projectMember != "Любой")
            {
                finOperations = finOperations.Where(p => p.ProjectMember.NameInProject.Contains(projectMember));
            }

            #endregion
            #region Сортировка
            switch (sortModel)
            {
                case FinOperationSortModelEnum.ProjectMember:
                    if (sortDir == FinOperationSortDirEnum.Ascending) { finOperations = finOperations.OrderBy(s => s.ProjectMember.NameInProject); break; }

                    finOperations = finOperations.OrderByDescending(s => s.ProjectMember.NameInProject);
                    break;
                case FinOperationSortModelEnum.FinType:
                    if (sortDir == FinOperationSortDirEnum.Ascending) { finOperations = finOperations.OrderBy(s => s.FinType); break; }
                    
                    finOperations = finOperations.OrderByDescending(s => s.FinType);
                    break;
                case FinOperationSortModelEnum.Category:
                    if (sortDir == FinOperationSortDirEnum.Ascending) { finOperations = finOperations.OrderBy(s => s.Category.Name); break; }

                    finOperations = finOperations.OrderByDescending(s => s.Category.Name);
                    break;
                case FinOperationSortModelEnum.CreateTime:
                    if (sortDir == FinOperationSortDirEnum.Ascending) { finOperations = finOperations.OrderBy(s => s.CreateTime); break; }

                    finOperations = finOperations.OrderByDescending(s => s.CreateTime);
                    break;
                case FinOperationSortModelEnum.Value:
                    if (sortDir == FinOperationSortDirEnum.Ascending) { finOperations = finOperations.OrderBy(s => s.Value); break; }

                    finOperations = finOperations.OrderByDescending(s => s.Value);
                    break;
                default:
                    finOperations = finOperations.OrderByDescending(s => s.CreateTime);
                    break;
            }
            #endregion
            #region Пагинация

            int pageSize = 10;

            var count = await finOperations.CountAsync();
            var items = await finOperations.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            #endregion

            var projectMembers = await _context.ProjectMembers.Where(pm => pm.ProjectId == id).Select(x => x.NameInProject).ToListAsync();
            projectMembers.Insert(0, "Любой");
            var members = new SelectList(projectMembers);

            var detailsModel = new HomeProjectDetailsModel
            {
                ProjectName = project.Name,
                PageViewModel = new FinOperationPageModel(count, page, pageSize),
                SortViewModel = new FinOperationSortModel(sortModel, sortDir),
                FilterViewModel = new FinOperationFilterModel(id, category, projectMember, finType, beginDate, endDate),
                FinOperations = items,
                ProjectMembers = members,
                PieByCategory = finOperations.GroupBy(x => x.Category.Name)
                .Select(x => new HomeProjectDetailsModel.PieItem { Name = x.Key ?? "—", Value = x.Sum(y => y.Value) }).ToList(),
                PieByProjectMember = finOperations.GroupBy(x => x.ProjectMember.NameInProject)
                .Select(x => new HomeProjectDetailsModel.PieItem { Name = x.Key ?? "—", Value = x.Sum(y => y.Value) }).ToList(),
                PieByFinOp = finOperations.GroupBy(x => x.FinType)
                .Select(x => new HomeProjectDetailsModel.PieItem { Name = x.Key == FinType.Income ? "Доход" : "Расход", Value = x.Sum(y => y.Value) }).ToList()
            };
            return View(detailsModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [AllowAnonymous]
        public IActionResult OverwriteDB()
        {
            _context.RemoveRange(_context.Projects);
            _context.RemoveRange(_context.ProjectMembers);
            _context.RemoveRange(_context.Categories);
            _context.RemoveRange(_context.FinOperations);
            _context.RemoveRange(_context.Users);

            var user = CreateIdentityUser();
            _context.Add(user);
            var project = CreateProject(user);
            _context.Add(project);

            _context.SaveChanges();
            _signInManager.SignOutAsync();

            return Ok("ok");
        }

        private Project CreateProject(IdentityUser user)
        {
            var nowTime = DateTime.Now;
            var project = new Project
            {
                Name = "Семейный бюджет",
                OwnerId = user.Id,
                Owner = user,
                CreateTime = nowTime,
                UpdateTime = nowTime,
            };

            CRUD_project.Create(project, user);
            #region добавление членов проекта и финансовых операций
            project.ProjectMembers.Add(
                new ProjectMember
                {
                    NameInProject = "Мама",
                    UserId = user.Id,
                    User = user,
                    ProjectId = project.Id,
                    Project = project,
                    CreateTime = nowTime,
                    UpdateTime = nowTime
                });


            project.ProjectMembers.Add(
                new ProjectMember
                {
                    NameInProject = "Дочка",
                    UserId = user.Id,
                    User = user,
                    ProjectId = project.Id,
                    Project = project,
                    CreateTime = nowTime,
                    UpdateTime = nowTime
                });

            project.FinOperations.Add(
                new FinOperation
                {
                    FinType = FinType.Income,
                    ForAll = true,
                    Value = 20000,
                    CategoryId = null,
                    ProjectMemberId = project.ProjectMembers.Find(p => p.NameInProject == "Владелец").Id,
                    ProjectMember = project.ProjectMembers.Find(p => p.NameInProject == "Владелец"),
                    ProjectId = project.Id,
                    Project = project,
                    CreateTime = new DateTime(2021, 12, 3, 22, 11, 11),
                    UpdateTime = new DateTime(2021, 12, 3, 22, 11, 11),
                });

            project.FinOperations.Add(
                new FinOperation
                {
                    FinType = FinType.Income,
                    ForAll = true,
                    Value = 15000,
                    CategoryId = null,
                    ProjectMemberId = project.ProjectMembers.Find(p => p.NameInProject == "Мама").Id,
                    ProjectMember = project.ProjectMembers.Find(p => p.NameInProject == "Мама"),
                    ProjectId = project.Id,
                    Project = project,
                    CreateTime = new DateTime(2021, 12, 7, 22, 11, 11),
                    UpdateTime = new DateTime(2021, 12, 7, 22, 11, 11),
                });

            project.FinOperations.Add(
                new FinOperation
                {
                    FinType = FinType.Income,
                    ForAll = true,
                    Value = 17000,
                    CategoryId = null,
                    ProjectMemberId = project.ProjectMembers.Find(p => p.NameInProject == "Владелец").Id,
                    ProjectMember = project.ProjectMembers.Find(p => p.NameInProject == "Владелец"),
                    ProjectId = project.Id,
                    Project = project,
                    CreateTime = new DateTime(2022, 1, 3, 22, 11, 11),
                    UpdateTime = new DateTime(2022, 1, 3, 22, 11, 11),
                });

            project.FinOperations.Add(
                new FinOperation
                {
                    FinType = FinType.Income,
                    ForAll = true,
                    Value = 10000,
                    CategoryId = null,
                    ProjectMemberId = project.ProjectMembers.Find(p => p.NameInProject == "Мама").Id,
                    ProjectMember = project.ProjectMembers.Find(p => p.NameInProject == "Мама"),
                    ProjectId = project.Id,
                    Project = project,
                    CreateTime = new DateTime(2022, 1, 7, 22, 11, 11),
                    UpdateTime = new DateTime(2022, 1, 7, 22, 11, 11),
                });

            project.FinOperations.Add(
                new FinOperation
                {
                    FinType = FinType.Charge,
                    ForAll = true,
                    Value = 1200,
                    CategoryId = project.Categories.Find(c => c.Name == "продукты").Id,
                    Category = project.Categories.Find(c => c.Name == "продукты"),
                    ProjectMemberId = project.ProjectMembers.Find(p => p.NameInProject == "Семья").Id,
                    ProjectMember = project.ProjectMembers.Find(p => p.NameInProject == "Семья"),
                    ProjectId = project.Id,
                    Project = project,
                    CreateTime = new DateTime(2021, 12, 5, 22, 11, 11),
                    UpdateTime = new DateTime(2021, 12, 5, 22, 11, 11),
                });
            project.FinOperations.Add(
                new FinOperation
                {
                    FinType = FinType.Charge,
                    ForAll = true,
                    Value = 600,
                    CategoryId = project.Categories.Find(c => c.Name == "продукты").Id,
                    Category = project.Categories.Find(c => c.Name == "продукты"),
                    ProjectMemberId = project.ProjectMembers.Find(p => p.NameInProject == "Семья").Id,
                    ProjectMember = project.ProjectMembers.Find(p => p.NameInProject == "Семья"),
                    ProjectId = project.Id,
                    Project = project,
                    CreateTime = new DateTime(2021, 12, 6, 22, 11, 11),
                    UpdateTime = new DateTime(2021, 12, 6, 22, 11, 11),
                });
            project.FinOperations.Add(
                new FinOperation
                {
                    FinType = FinType.Charge,
                    ForAll = true,
                    Value = 300,
                    CategoryId = project.Categories.Find(c => c.Name == "продукты").Id,
                    Category = project.Categories.Find(c => c.Name == "продукты"),
                    ProjectMemberId = project.ProjectMembers.Find(p => p.NameInProject == "Владелец").Id,
                    ProjectMember = project.ProjectMembers.Find(p => p.NameInProject == "Владелец"),
                    ProjectId = project.Id,
                    Project = project,
                    CreateTime = new DateTime(2021, 12, 7, 22, 11, 11),
                    UpdateTime = new DateTime(2021, 12, 7, 22, 11, 11),
                });
            project.FinOperations.Add(
                new FinOperation
                {
                    FinType = FinType.Charge,
                    ForAll = true,
                    Value = 400,
                    CategoryId = project.Categories.Find(c => c.Name == "продукты").Id,
                    Category = project.Categories.Find(c => c.Name == "продукты"),
                    ProjectMemberId = project.ProjectMembers.Find(p => p.NameInProject == "Дочка").Id,
                    ProjectMember = project.ProjectMembers.Find(p => p.NameInProject == "Дочка"),
                    ProjectId = project.Id,
                    Project = project,
                    CreateTime = new DateTime(2021, 12, 7, 22, 11, 11),
                    UpdateTime = new DateTime(2021, 12, 7, 22, 11, 11),
                });
            project.FinOperations.Add(
                new FinOperation
                {
                    FinType = FinType.Charge,
                    ForAll = true,
                    Value = 700,
                    CategoryId = project.Categories.Find(c => c.Name == "продукты").Id,
                    Category = project.Categories.Find(c => c.Name == "продукты"),
                    ProjectMemberId = project.ProjectMembers.Find(p => p.NameInProject == "Семья").Id,
                    ProjectMember = project.ProjectMembers.Find(p => p.NameInProject == "Семья"),
                    ProjectId = project.Id,
                    Project = project,
                    CreateTime = new DateTime(2021, 12, 13, 22, 11, 11),
                    UpdateTime = new DateTime(2021, 12, 13, 22, 11, 11),
                });
            project.FinOperations.Add(
                new FinOperation
                {
                    FinType = FinType.Charge,
                    ForAll = true,
                    Value = 900,
                    CategoryId = project.Categories.Find(c => c.Name == "продукты").Id,
                    Category = project.Categories.Find(c => c.Name == "продукты"),
                    ProjectMemberId = project.ProjectMembers.Find(p => p.NameInProject == "Семья").Id,
                    ProjectMember = project.ProjectMembers.Find(p => p.NameInProject == "Семья"),
                    ProjectId = project.Id,
                    Project = project,
                    CreateTime = new DateTime(2021, 12, 16, 22, 11, 11),
                    UpdateTime = new DateTime(2021, 12, 16, 22, 11, 11),
                });
            project.FinOperations.Add(
                new FinOperation
                {
                    FinType = FinType.Charge,
                    ForAll = true,
                    Value = 200,
                    CategoryId = project.Categories.Find(c => c.Name == "продукты").Id,
                    Category = project.Categories.Find(c => c.Name == "продукты"),
                    ProjectMemberId = project.ProjectMembers.Find(p => p.NameInProject == "Семья").Id,
                    ProjectMember = project.ProjectMembers.Find(p => p.NameInProject == "Семья"),
                    ProjectId = project.Id,
                    Project = project,
                    CreateTime = new DateTime(2021, 12, 17, 22, 11, 11),
                    UpdateTime = new DateTime(2021, 12, 17, 22, 11, 11),
                });
            project.FinOperations.Add(
                new FinOperation
                {
                    FinType = FinType.Charge,
                    ForAll = true,
                    Value = 1200,
                    CategoryId = project.Categories.Find(c => c.Name == "продукты").Id,
                    Category = project.Categories.Find(c => c.Name == "продукты"),
                    ProjectMemberId = project.ProjectMembers.Find(p => p.NameInProject == "Семья").Id,
                    ProjectMember = project.ProjectMembers.Find(p => p.NameInProject == "Семья"),
                    ProjectId = project.Id,
                    Project = project,
                    CreateTime = new DateTime(2021, 12, 21, 22, 11, 11),
                    UpdateTime = new DateTime(2021, 12, 21, 22, 11, 11),
                });
            project.FinOperations.Add(
                new FinOperation
                {
                    FinType = FinType.Charge,
                    ForAll = true,
                    Value = 1200,
                    CategoryId = project.Categories.Find(c => c.Name == "продукты").Id,
                    Category = project.Categories.Find(c => c.Name == "продукты"),
                    ProjectMemberId = project.ProjectMembers.Find(p => p.NameInProject == "Семья").Id,
                    ProjectMember = project.ProjectMembers.Find(p => p.NameInProject == "Семья"),
                    ProjectId = project.Id,
                    Project = project,
                    CreateTime = new DateTime(2021, 12, 23, 22, 11, 11),
                    UpdateTime = new DateTime(2021, 12, 23, 22, 11, 11),
                });
            project.FinOperations.Add(
                new FinOperation
                {
                    FinType = FinType.Charge,
                    ForAll = true,
                    Value = 500,
                    CategoryId = project.Categories.Find(c => c.Name == "продукты").Id,
                    Category = project.Categories.Find(c => c.Name == "продукты"),
                    ProjectMemberId = project.ProjectMembers.Find(p => p.NameInProject == "Владелец").Id,
                    ProjectMember = project.ProjectMembers.Find(p => p.NameInProject == "Владелец"),
                    ProjectId = project.Id,
                    Project = project,
                    CreateTime = new DateTime(2021, 12, 23, 22, 11, 11),
                    UpdateTime = new DateTime(2021, 12, 23, 22, 11, 11),
                });
            project.FinOperations.Add(
                new FinOperation
                {
                    FinType = FinType.Charge,
                    ForAll = true,
                    Value = 300,
                    CategoryId = project.Categories.Find(c => c.Name == "продукты").Id,
                    Category = project.Categories.Find(c => c.Name == "продукты"),
                    ProjectMemberId = project.ProjectMembers.Find(p => p.NameInProject == "Мама").Id,
                    ProjectMember = project.ProjectMembers.Find(p => p.NameInProject == "Мама"),
                    ProjectId = project.Id,
                    Project = project,
                    CreateTime = new DateTime(2021, 12, 24, 22, 11, 11),
                    UpdateTime = new DateTime(2021, 12, 24, 22, 11, 11),
                });
            project.FinOperations.Add(
                new FinOperation
                {
                    FinType = FinType.Charge,
                    ForAll = true,
                    Value = 3500,
                    CategoryId = project.Categories.Find(c => c.Name == "продукты").Id,
                    Category = project.Categories.Find(c => c.Name == "продукты"),
                    ProjectMemberId = project.ProjectMembers.Find(p => p.NameInProject == "Семья").Id,
                    ProjectMember = project.ProjectMembers.Find(p => p.NameInProject == "Семья"),
                    ProjectId = project.Id,
                    Project = project,
                    CreateTime = new DateTime(2021, 12, 27, 22, 11, 11),
                    UpdateTime = new DateTime(2021, 12, 27, 22, 11, 11),
                });
            project.FinOperations.Add(
                new FinOperation
                {
                    FinType = FinType.Charge,
                    ForAll = true,
                    Value = 2200,
                    CategoryId = project.Categories.Find(c => c.Name == "продукты").Id,
                    Category = project.Categories.Find(c => c.Name == "продукты"),
                    ProjectMemberId = project.ProjectMembers.Find(p => p.NameInProject == "Семья").Id,
                    ProjectMember = project.ProjectMembers.Find(p => p.NameInProject == "Семья"),
                    ProjectId = project.Id,
                    Project = project,
                    CreateTime = new DateTime(2021, 12, 29, 22, 11, 11),
                    UpdateTime = new DateTime(2021, 12, 29, 22, 11, 11),
                });
            project.FinOperations.Add(
                new FinOperation
                {
                    FinType = FinType.Charge,
                    ForAll = true,
                    Value = 1700,
                    CategoryId = project.Categories.Find(c => c.Name == "продукты").Id,
                    Category = project.Categories.Find(c => c.Name == "продукты"),
                    ProjectMemberId = project.ProjectMembers.Find(p => p.NameInProject == "Семья").Id,
                    ProjectMember = project.ProjectMembers.Find(p => p.NameInProject == "Семья"),
                    ProjectId = project.Id,
                    Project = project,
                    CreateTime = new DateTime(2021, 12, 31, 22, 11, 11),
                    UpdateTime = new DateTime(2021, 12, 31, 22, 11, 11),
                });
            project.FinOperations.Add(
                new FinOperation
                {
                    FinType = FinType.Charge,
                    ForAll = true,
                    Value = 1700,
                    CategoryId = project.Categories.Find(c => c.Name == "продукты").Id,
                    Category = project.Categories.Find(c => c.Name == "продукты"),
                    ProjectMemberId = project.ProjectMembers.Find(p => p.NameInProject == "Семья").Id,
                    ProjectMember = project.ProjectMembers.Find(p => p.NameInProject == "Семья"),
                    ProjectId = project.Id,
                    Project = project,
                    CreateTime = new DateTime(2022, 1, 2, 22, 11, 11),
                    UpdateTime = new DateTime(2022, 1, 2, 22, 11, 11),
                });
            project.FinOperations.Add(
                new FinOperation
                {
                    FinType = FinType.Charge,
                    ForAll = true,
                    Value = 600,
                    CategoryId = project.Categories.Find(c => c.Name == "продукты").Id,
                    Category = project.Categories.Find(c => c.Name == "продукты"),
                    ProjectMemberId = project.ProjectMembers.Find(p => p.NameInProject == "Семья").Id,
                    ProjectMember = project.ProjectMembers.Find(p => p.NameInProject == "Семья"),
                    ProjectId = project.Id,
                    Project = project,
                    CreateTime = new DateTime(2022, 1, 2, 22, 11, 11),
                    UpdateTime = new DateTime(2022, 1, 2, 22, 11, 11),
                });
            project.FinOperations.Add(
                new FinOperation
                {
                    FinType = FinType.Charge,
                    ForAll = true,
                    Value = 900,
                    CategoryId = project.Categories.Find(c => c.Name == "продукты").Id,
                    Category = project.Categories.Find(c => c.Name == "продукты"),
                    ProjectMemberId = project.ProjectMembers.Find(p => p.NameInProject == "Семья").Id,
                    ProjectMember = project.ProjectMembers.Find(p => p.NameInProject == "Семья"),
                    ProjectId = project.Id,
                    Project = project,
                    CreateTime = new DateTime(2022, 1, 4, 22, 11, 11),
                    UpdateTime = new DateTime(2022, 1, 4, 22, 11, 11),
                });
            project.FinOperations.Add(
                new FinOperation
                {
                    FinType = FinType.Charge,
                    ForAll = true,
                    Value = 700,
                    CategoryId = project.Categories.Find(c => c.Name == "продукты").Id,
                    Category = project.Categories.Find(c => c.Name == "продукты"),
                    ProjectMemberId = project.ProjectMembers.Find(p => p.NameInProject == "Мама").Id,
                    ProjectMember = project.ProjectMembers.Find(p => p.NameInProject == "Мама"),
                    ProjectId = project.Id,
                    Project = project,
                    CreateTime = new DateTime(2022, 1, 4, 22, 11, 11),
                    UpdateTime = new DateTime(2022, 1, 4, 22, 11, 11),
                });
            project.FinOperations.Add(
                new FinOperation
                {
                    FinType = FinType.Charge,
                    ForAll = true,
                    Value = 260,
                    CategoryId = project.Categories.Find(c => c.Name == "продукты").Id,
                    Category = project.Categories.Find(c => c.Name == "продукты"),
                    ProjectMemberId = project.ProjectMembers.Find(p => p.NameInProject == "Дочка").Id,
                    ProjectMember = project.ProjectMembers.Find(p => p.NameInProject == "Дочка"),
                    ProjectId = project.Id,
                    Project = project,
                    CreateTime = new DateTime(2022, 1, 5, 22, 11, 11),
                    UpdateTime = new DateTime(2022, 1, 5, 22, 11, 11),
                });
            project.FinOperations.Add(
                new FinOperation
                {
                    FinType = FinType.Charge,
                    ForAll = true,
                    Value = 1800,
                    CategoryId = project.Categories.Find(c => c.Name == "продукты").Id,
                    Category = project.Categories.Find(c => c.Name == "продукты"),
                    ProjectMemberId = project.ProjectMembers.Find(p => p.NameInProject == "Семья").Id,
                    ProjectMember = project.ProjectMembers.Find(p => p.NameInProject == "Семья"),
                    ProjectId = project.Id,
                    Project = project,
                    CreateTime = new DateTime(2022, 1, 8, 22, 11, 11),
                    UpdateTime = new DateTime(2022, 1, 8, 22, 11, 11),
                });
            project.FinOperations.Add(
                new FinOperation
                {
                    FinType = FinType.Charge,
                    ForAll = true,
                    Value = 2200,
                    CategoryId = project.Categories.Find(c => c.Name == "продукты").Id,
                    Category = project.Categories.Find(c => c.Name == "продукты"),
                    ProjectMemberId = project.ProjectMembers.Find(p => p.NameInProject == "Семья").Id,
                    ProjectMember = project.ProjectMembers.Find(p => p.NameInProject == "Семья"),
                    ProjectId = project.Id,
                    Project = project,
                    CreateTime = new DateTime(2022, 1, 11, 22, 11, 11),
                    UpdateTime = new DateTime(2022, 1, 11, 22, 11, 11),
                });
            project.FinOperations.Add(
                new FinOperation
                {
                    FinType = FinType.Charge,
                    ForAll = true,
                    Value = 400,
                    CategoryId = project.Categories.Find(c => c.Name == "продукты").Id,
                    Category = project.Categories.Find(c => c.Name == "продукты"),
                    ProjectMemberId = project.ProjectMembers.Find(p => p.NameInProject == "Семья").Id,
                    ProjectMember = project.ProjectMembers.Find(p => p.NameInProject == "Семья"),
                    ProjectId = project.Id,
                    Project = project,
                    CreateTime = new DateTime(2022, 1, 13, 22, 11, 11),
                    UpdateTime = new DateTime(2022, 1, 13, 22, 11, 11),
                });
            project.FinOperations.Add(
                new FinOperation
                {
                    FinType = FinType.Charge,
                    ForAll = true,
                    Value = 1200,
                    CategoryId = project.Categories.Find(c => c.Name == "продукты").Id,
                    Category = project.Categories.Find(c => c.Name == "продукты"),
                    ProjectMemberId = project.ProjectMembers.Find(p => p.NameInProject == "Семья").Id,
                    ProjectMember = project.ProjectMembers.Find(p => p.NameInProject == "Семья"),
                    ProjectId = project.Id,
                    Project = project,
                    CreateTime = new DateTime(2022, 1, 13, 8, 11, 11),
                    UpdateTime = new DateTime(2022, 1, 13, 8, 11, 11),
                });
            project.FinOperations.Add(
                new FinOperation
                {
                    FinType = FinType.Charge,
                    ForAll = true,
                    Value = 3300,
                    CategoryId = project.Categories.Find(c => c.Name == "кафе").Id,
                    Category = project.Categories.Find(c => c.Name == "кафе"),
                    ProjectMemberId = project.ProjectMembers.Find(p => p.NameInProject == "Семья").Id,
                    ProjectMember = project.ProjectMembers.Find(p => p.NameInProject == "Семья"),
                    ProjectId = project.Id,
                    Project = project,
                    CreateTime = new DateTime(2021, 12, 13, 8, 11, 11),
                    UpdateTime = new DateTime(2021, 12, 13, 8, 11, 11),
                });
            project.FinOperations.Add(
                new FinOperation
                {
                    FinType = FinType.Charge,
                    ForAll = true,
                    Value = 1200,
                    CategoryId = project.Categories.Find(c => c.Name == "кафе").Id,
                    Category = project.Categories.Find(c => c.Name == "кафе"),
                    ProjectMemberId = project.ProjectMembers.Find(p => p.NameInProject == "Владелец").Id,
                    ProjectMember = project.ProjectMembers.Find(p => p.NameInProject == "Владелец"),
                    ProjectId = project.Id,
                    Project = project,
                    CreateTime = new DateTime(2021, 12, 17, 8, 11, 11),
                    UpdateTime = new DateTime(2021, 12, 17, 8, 11, 11),
                });
            project.FinOperations.Add(
                new FinOperation
                {
                    FinType = FinType.Charge,
                    ForAll = true,
                    Value = 700,
                    CategoryId = project.Categories.Find(c => c.Name == "кафе").Id,
                    Category = project.Categories.Find(c => c.Name == "кафе"),
                    ProjectMemberId = project.ProjectMembers.Find(p => p.NameInProject == "Мама").Id,
                    ProjectMember = project.ProjectMembers.Find(p => p.NameInProject == "Мама"),
                    ProjectId = project.Id,
                    Project = project,
                    CreateTime = new DateTime(2021, 12, 22, 8, 11, 11),
                    UpdateTime = new DateTime(2021, 12, 22, 8, 11, 11),
                });
            project.FinOperations.Add(
                new FinOperation
                {
                    FinType = FinType.Charge,
                    ForAll = true,
                    Value = 1000,
                    CategoryId = project.Categories.Find(c => c.Name == "развлечения").Id,
                    Category = project.Categories.Find(c => c.Name == "развлечения"),
                    ProjectMemberId = project.ProjectMembers.Find(p => p.NameInProject == "Дочка").Id,
                    ProjectMember = project.ProjectMembers.Find(p => p.NameInProject == "Дочка"),
                    ProjectId = project.Id,
                    Project = project,
                    CreateTime = new DateTime(2021, 12, 15, 8, 11, 11),
                    UpdateTime = new DateTime(2021, 12, 15, 8, 11, 11),
                });
            project.FinOperations.Add(
                new FinOperation
                {
                    FinType = FinType.Charge,
                    ForAll = true,
                    Value = 2000,
                    CategoryId = project.Categories.Find(c => c.Name == "развлечения").Id,
                    Category = project.Categories.Find(c => c.Name == "развлечения"),
                    ProjectMemberId = project.ProjectMembers.Find(p => p.NameInProject == "Дочка").Id,
                    ProjectMember = project.ProjectMembers.Find(p => p.NameInProject == "Дочка"),
                    ProjectId = project.Id,
                    Project = project,
                    CreateTime = new DateTime(2022, 1, 9, 8, 11, 11),
                    UpdateTime = new DateTime(2022, 1, 9, 8, 11, 11),
                });
            project.FinOperations.Add(
                new FinOperation
                {
                    FinType = FinType.Charge,
                    ForAll = true,
                    Value = 1500,
                    CategoryId = project.Categories.Find(c => c.Name == "развлечения").Id,
                    Category = project.Categories.Find(c => c.Name == "развлечения"),
                    ProjectMemberId = project.ProjectMembers.Find(p => p.NameInProject == "Дочка").Id,
                    ProjectMember = project.ProjectMembers.Find(p => p.NameInProject == "Дочка"),
                    ProjectId = project.Id,
                    Project = project,
                    CreateTime = new DateTime(2021, 12, 19, 8, 11, 11),
                    UpdateTime = new DateTime(2021, 12, 19, 8, 11, 11),
                });
            project.FinOperations.Add(
                new FinOperation
                {
                    FinType = FinType.Charge,
                    ForAll = true,
                    Value = 1500,
                    CategoryId = project.Categories.Find(c => c.Name == "развлечения").Id,
                    Category = project.Categories.Find(c => c.Name == "развлечения"),
                    ProjectMemberId = project.ProjectMembers.Find(p => p.NameInProject == "Дочка").Id,
                    ProjectMember = project.ProjectMembers.Find(p => p.NameInProject == "Дочка"),
                    ProjectId = project.Id,
                    Project = project,
                    CreateTime = new DateTime(2021, 12, 23, 8, 11, 11),
                    UpdateTime = new DateTime(2021, 12, 23, 8, 11, 11),
                });
            project.FinOperations.Add(
                new FinOperation
                {
                    FinType = FinType.Charge,
                    ForAll = true,
                    Value = 1500,
                    CategoryId = project.Categories.Find(c => c.Name == "развлечения").Id,
                    Category = project.Categories.Find(c => c.Name == "развлечения"),
                    ProjectMemberId = project.ProjectMembers.Find(p => p.NameInProject == "Мама").Id,
                    ProjectMember = project.ProjectMembers.Find(p => p.NameInProject == "Мама"),
                    ProjectId = project.Id,
                    Project = project,
                    CreateTime = new DateTime(2021, 12, 23, 8, 11, 11),
                    UpdateTime = new DateTime(2021, 12, 23, 8, 11, 11),
                });
            project.FinOperations.Add(
                new FinOperation
                {
                    FinType = FinType.Charge,
                    ForAll = true,
                    Value = 3000,
                    CategoryId = project.Categories.Find(c => c.Name == "развлечения").Id,
                    Category = project.Categories.Find(c => c.Name == "развлечения"),
                    ProjectMemberId = project.ProjectMembers.Find(p => p.NameInProject == "Семья").Id,
                    ProjectMember = project.ProjectMembers.Find(p => p.NameInProject == "Семья"),
                    ProjectId = project.Id,
                    Project = project,
                    CreateTime = new DateTime(2022, 1, 4, 8, 11, 11),
                    UpdateTime = new DateTime(2022, 1, 4, 8, 11, 11),
                });
            project.FinOperations.Add(
                new FinOperation
                {
                    FinType = FinType.Charge,
                    ForAll = true,
                    Value = 3000,
                    CategoryId = project.Categories.Find(c => c.Name == "подарки").Id,
                    Category = project.Categories.Find(c => c.Name == "подарки"),
                    ProjectMemberId = project.ProjectMembers.Find(p => p.NameInProject == "Семья").Id,
                    ProjectMember = project.ProjectMembers.Find(p => p.NameInProject == "Семья"),
                    ProjectId = project.Id,
                    Project = project,
                    CreateTime = new DateTime(2021, 12, 23, 8, 11, 11),
                    UpdateTime = new DateTime(2021, 12, 23, 8, 11, 11),
                });
            project.FinOperations.Add(
                new FinOperation
                {
                    FinType = FinType.Charge,
                    ForAll = true,
                    Value = 5000,
                    CategoryId = project.Categories.Find(c => c.Name == "подарки").Id,
                    Category = project.Categories.Find(c => c.Name == "подарки"),
                    ProjectMemberId = project.ProjectMembers.Find(p => p.NameInProject == "Семья").Id,
                    ProjectMember = project.ProjectMembers.Find(p => p.NameInProject == "Семья"),
                    ProjectId = project.Id,
                    Project = project,
                    CreateTime = new DateTime(2021, 12, 27, 8, 11, 11),
                    UpdateTime = new DateTime(2021, 12, 27, 8, 11, 11),
                });
            #endregion
            return project;

        }
        private IdentityUser CreateIdentityUser()
        {
            const string EMAIL = "demo@mail.ru";
            var user = new IdentityUser
            {
                UserName = EMAIL,
                NormalizedUserName = EMAIL.ToUpper(),
                Email = EMAIL,
                NormalizedEmail = EMAIL.ToUpper(),
                EmailConfirmed = true,
                SecurityStamp = new Guid().ToString(),
            };
            var hasher = new PasswordHasher<IdentityUser>();
            var hashedPass = hasher.HashPassword(user, "123456");
            user.PasswordHash = hashedPass;

            return user;
        }

        private IdentityUser CurrentUser()
        {
            var username = HttpContext.User.Identity.Name;
            return _context.Users
                .FirstOrDefault(m => m.UserName == username);
        }
    }
}