using FamilyBudget.Controllers;
using FamilyBudget.Data;
using FamilyBudget.Models;
using FamilyBudget.Extensions;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System;

namespace FamilyBudget.CRUD_models
{
    public class CRUD_project
    {
        public static void Create(Project project, IdentityUser user, ApplicationDbContext context)
        {
            var timeNow = DateTime.Now;
            project.ProjectMembers.Add(new ProjectMember
            {
                    NameInProject = "Владелец",
                    User = user,
                    UserId = user.Id,
                    ProjectId = project.Id,
                    Project = project,
                    CreateTime = timeNow,
                    UpdateTime = timeNow
            });

            project.Categories.Add(new Category 
            {  
                ProjectId = project.Id, 
                Name = "Продукты", 
                Project = project, 
                CreateTime = timeNow, 
                UpdateTime = timeNow 
            });
            project.Categories.Add(new Category 
            { 
                ProjectId = project.Id, 
                Name = "Транспорт", 
                Project = project, 
                CreateTime = timeNow, 
                UpdateTime = timeNow 
            });
            project.Categories.Add(new Category
            {
                ProjectId = project.Id,
                Name = "Кафе",
                Project = project,
                CreateTime = timeNow,
                UpdateTime = timeNow
            });
            project.Categories.Add(new Category
            {
                ProjectId = project.Id,
                Name = "Развлечения",
                Project = project,
                CreateTime = timeNow,
                UpdateTime = timeNow
            });
            project.Categories.Add(new Category
            {
                ProjectId = project.Id,
                Name = "Подарки",
                Project = project,
                CreateTime = timeNow,
                UpdateTime = timeNow
            });
            project.Categories.Add(new Category
            {
                ProjectId = project.Id,
                Name = "Бытовая техника",
                Project = project,
                CreateTime = timeNow,
                UpdateTime = timeNow
            });
        }
    }
}
