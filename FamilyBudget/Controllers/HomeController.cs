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

        public async Task<IActionResult> Details(int? id, string category, FinType? finType, int page = 1,
            FinOperationSortModelEnum sortModel = FinOperationSortModelEnum.Default,
            FinOperationSortDirEnum sortType = FinOperationSortDirEnum.Ascending)
        {
            var project = _context.Projects
                .FirstOrDefault(p => p.Id == id);

            if (project == null)
            {
                return NotFound();
            }

            if (!user.CanView(project,_context))
            {
                return Forbid();
            }

            IQueryable<FinOperation> finOperations = _context.FinOperations
                .Include(x => x.Project)
                .Include(x => x.Category);

            finOperations = finOperations.Where(p => p.ProjectId == id);

            if (finType!= null)
            {
                finOperations = finOperations.Where(p => p.FinType == finType);
            }

            if (!String.IsNullOrEmpty(category))
            {
                finOperations = finOperations.Where(p => p.Category.Name.Contains(category));
            }

            switch (sortModel)
            {
                case FinOperationSortModelEnum.FinType:
                    if(sortType == FinOperationSortDirEnum.Ascending) { finOperations = finOperations.OrderBy(s => s.FinType); break; }

                    finOperations = finOperations.OrderByDescending(s => s.FinType);
                    break;
                case FinOperationSortModelEnum.Category:
                    if (sortType == FinOperationSortDirEnum.Ascending) { finOperations = finOperations.OrderBy(s => s.Category); break; }

                    finOperations = finOperations.OrderByDescending(s => s.Category);
                    break;
                default:
                case FinOperationSortModelEnum.CreateTime:
                    if (sortType == FinOperationSortDirEnum.Ascending) { finOperations = finOperations.OrderBy(s => s.CreateTime); break; }

                    finOperations = finOperations.OrderByDescending(s => s.CreateTime);
                    break;
                case FinOperationSortModelEnum.Value:
                    if (sortType == FinOperationSortDirEnum.Ascending) { finOperations = finOperations.OrderBy(s => s.Value); break; }

                    finOperations = finOperations.OrderByDescending(s => s.Value);
                    break;
            }

            int pageSize = 10;

            var count = await finOperations.CountAsync();
            var items = await finOperations.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            var detailsModel = new HomeProjectDetailsModel
            {
                PageViewModel = new FinOperationPageModel(count, page, pageSize),
                SortViewModel = new FinOperationSortModel(sortModel, sortType),
                FilterViewModel = new FinOperationFilterModel(id, category, finType),
                finOperations = items
            };
            return View(detailsModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        public void OverwriteDB()
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

            CRUD_models.CRUD_project.Create(project, user);
            #region добавление членов проекта и финансовых операций
            project.ProjectMembers.Add(
                new ProjectMember
                {
                    NameInProject = "Лидия Иванова",
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
                    NameInProject = "Сергей Петров",
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
                    FinType = FinType.Charge,
                    ForAll = true,
                    Value = 1200,
                    CategoryId = project.Categories.Find(c => c.Name == "Продукты").Id,
                    Category = project.Categories.Find(c => c.Name == "Продукты"),
                    ProjectMemberId = project.ProjectMembers.Find(p => p.NameInProject == "Владелец").Id,
                    ProjectMember = project.ProjectMembers.Find(p => p.NameInProject == "Владелец"),
                    ProjectId = project.Id,
                    Project = project,
                    CreateTime = new DateTime(2021,1,12,22,11,11),
                    UpdateTime = new DateTime(2021,1,12,22,11,11),
                });
            project.FinOperations.Add(
                new FinOperation
                {
                    FinType = FinType.Charge,
                    ForAll = true,
                    Value = 110,
                    CategoryId = project.Categories.Find(c => c.Name == "Продукты").Id,
                    Category = project.Categories.Find(c => c.Name == "Продукты"),
                    ProjectMemberId = project.ProjectMembers.Find(p => p.NameInProject == "Владелец").Id,
                    ProjectMember = project.ProjectMembers.Find(p => p.NameInProject == "Владелец"),
                    ProjectId = project.Id,
                    Project = project,
                    CreateTime = new DateTime(2021, 1, 12, 20, 11, 11),
                    UpdateTime = new DateTime(2021, 1, 12, 20, 11, 11),
                });
            project.FinOperations.Add(
                new FinOperation
                {
                    FinType = FinType.Charge,
                    ForAll = true,
                    Value = 11000,
                    CategoryId = project.Categories.Find(c => c.Name == "Подарки").Id,
                    Category = project.Categories.Find(c => c.Name == "Подарки"),
                    ProjectMemberId = project.ProjectMembers.Find(p => p.NameInProject == "Владелец").Id,
                    ProjectMember = project.ProjectMembers.Find(p => p.NameInProject == "Владелец"),
                    ProjectId = project.Id,
                    Project = project,
                    CreateTime = new DateTime(2021, 12, 12, 21, 11, 11),
                    UpdateTime = new DateTime(2021, 12, 12, 21, 11, 11),
                });
                
            project.FinOperations.Add(
                new FinOperation
                {
                    FinType = FinType.Charge,
                    ForAll = true,
                    Value = 11000,
                    CategoryId = project.Categories.Find(c => c.Name == "Бытовая техника").Id,
                    Category = project.Categories.Find(c => c.Name == "Бытовая техника"),
                    ProjectMemberId = project.ProjectMembers.Find(p => p.NameInProject == "Лидия Иванова").Id,
                    ProjectMember = project.ProjectMembers.Find(p => p.NameInProject == "Лидия Иванова"),
                    ProjectId = project.Id,
                    Project = project,
                    CreateTime = new DateTime(2021, 6, 17, 14, 11, 11),
                    UpdateTime = new DateTime(2021, 6, 17, 14, 11, 11),
                });
            project.FinOperations.Add(
                new FinOperation
                {
                    FinType = FinType.Charge,
                    ForAll = true,
                    Value = 2500,
                    CategoryId = project.Categories.Find(c => c.Name == "Развлечения").Id,
                    Category = project.Categories.Find(c => c.Name == "Развлечения"),
                    ProjectMemberId = project.ProjectMembers.Find(p => p.NameInProject == "Лидия Иванова").Id,
                    ProjectMember = project.ProjectMembers.Find(p => p.NameInProject == "Лидия Иванова"),
                    ProjectId = project.Id,
                    Project = project,
                    CreateTime = new DateTime(2021, 10, 20, 10, 11, 11),
                    UpdateTime = new DateTime(2021, 10, 20, 10, 11, 11),
                });

            project.FinOperations.Add(
                new FinOperation
                {
                    FinType = FinType.Charge,
                    ForAll = true,
                    Value = 1100,
                    CategoryId = project.Categories.Find(c => c.Name == "Развлечения").Id,
                    Category = project.Categories.Find(c => c.Name == "Развлечения"),
                    ProjectMemberId = project.ProjectMembers.Find(p => p.NameInProject == "Сергей Петров").Id,
                    ProjectMember = project.ProjectMembers.Find(p => p.NameInProject == "Сергей Петров"),
                    ProjectId = project.Id,
                    Project = project,
                    CreateTime = new DateTime(2021, 4, 12, 20, 0, 0),
                    UpdateTime = new DateTime(2021, 4, 12, 20, 0, 0),
                });
            project.FinOperations.Add(
                new FinOperation
                {
                    FinType = FinType.Charge,
                    ForAll = true,
                    Value = 10000,
                    CategoryId = project.Categories.Find(c => c.Name == "Развлечения").Id,
                    Category = project.Categories.Find(c => c.Name == "Развлечения"),
                    ProjectMemberId = project.ProjectMembers.Find(p => p.NameInProject == "Сергей Петров").Id,
                    ProjectMember = project.ProjectMembers.Find(p => p.NameInProject == "Сергей Петров"),
                    ProjectId = project.Id,
                    Project = project,
                    CreateTime = new DateTime(2021, 5, 12, 20, 0, 0),
                    UpdateTime = new DateTime(2021, 5, 12, 20, 0, 0),
                });

            project.FinOperations.Add(
                new FinOperation
                {
                    FinType = FinType.Charge,
                    ForAll = true,
                    Value = 10000,
                    CategoryId = project.Categories.Find(c => c.Name == "Развлечения").Id,
                    Category = project.Categories.Find(c => c.Name == "Развлечения"),
                    ProjectMemberId = project.ProjectMembers.Find(p => p.NameInProject == "Лидия Иванова").Id,
                    ProjectMember = project.ProjectMembers.Find(p => p.NameInProject == "Лидия Иванова"),
                    ProjectId = project.Id,
                    Project = project,
                    CreateTime = new DateTime(2021, 5, 10, 21, 0, 0),
                    UpdateTime = new DateTime(2021, 5, 10, 21, 0, 0),
                });
            project.FinOperations.Add(
                new FinOperation
                {
                    FinType = FinType.Charge,
                    ForAll = true,
                    Value = 4000,
                    CategoryId = project.Categories.Find(c => c.Name == "Кафе").Id,
                    Category = project.Categories.Find(c => c.Name == "Кафе"),
                    ProjectMemberId = project.ProjectMembers.Find(p => p.NameInProject == "Сергей Петров").Id,
                    ProjectMember = project.ProjectMembers.Find(p => p.NameInProject == "Сергей Петров"),
                    ProjectId = project.Id,
                    Project = project,
                    CreateTime = new DateTime(2021, 5, 10, 21, 0, 0),
                    UpdateTime = new DateTime(2021, 5, 10, 21, 0, 0),
                });

            project.FinOperations.Add(
                new FinOperation
                {
                    FinType = FinType.Charge,
                    ForAll = true,
                    Value = 4000,
                    CategoryId = project.Categories.Find(c => c.Name == "Кафе").Id,
                    Category = project.Categories.Find(c => c.Name == "Кафе"),
                    ProjectMemberId = project.ProjectMembers.Find(p => p.NameInProject == "Владелец").Id,
                    ProjectMember = project.ProjectMembers.Find(p => p.NameInProject == "Владелец"),
                    ProjectId = project.Id,
                    Project = project,
                    CreateTime = new DateTime(2021, 5, 7, 19, 0, 0),
                    UpdateTime = new DateTime(2021, 5, 7, 19, 0, 0),
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