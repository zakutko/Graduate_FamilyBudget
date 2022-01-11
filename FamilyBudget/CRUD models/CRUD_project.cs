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
        public static void Create(Project project, IdentityUser user)
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
                Name = "продукты", 
                Project = project, 
                CreateTime = timeNow, 
                UpdateTime = timeNow 
            });
            project.Categories.Add(new Category 
            { 
                ProjectId = project.Id, 
                Name = "транспорт", 
                Project = project, 
                CreateTime = timeNow, 
                UpdateTime = timeNow 
            });
            project.Categories.Add(new Category
            {
                ProjectId = project.Id,
                Name = "кафе",
                Project = project,
                CreateTime = timeNow,
                UpdateTime = timeNow
            });
            project.Categories.Add(new Category
            {
                ProjectId = project.Id,
                Name = "развлечения",
                Project = project,
                CreateTime = timeNow,
                UpdateTime = timeNow
            });
            project.Categories.Add(new Category
            {
                ProjectId = project.Id,
                Name = "подарки",
                Project = project,
                CreateTime = timeNow,
                UpdateTime = timeNow
            });
            project.Categories.Add(new Category
            {
                ProjectId = project.Id,
                Name = "бытовая техника",
                Project = project,
                CreateTime = timeNow,
                UpdateTime = timeNow
            });
        }
    }
}
