using FamilyBudget.Models;
using FamilyBudget.Data;
using Microsoft.AspNetCore.Identity;
using NUnit.Framework;
using System;
using Microsoft.EntityFrameworkCore;
using FamilyBudget.Extensions;

namespace FamilyBudget.Tests
{
    public class TestExtensions
    {
        ApplicationDbContext Context;
        IdentityUser User1;
        IdentityUser User2;
        Project Project;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "Test")
                .Options;

            Context = new ApplicationDbContext(options);

            const string EMAIL1 = "test1@mail.ru";
            User1 = new IdentityUser
            {
                UserName = EMAIL1,
                NormalizedUserName = EMAIL1.ToUpper(),
                Email = EMAIL1,
                NormalizedEmail = EMAIL1.ToUpper(),
                EmailConfirmed = true,
                SecurityStamp = new Guid().ToString(),
            };

            const string EMAIL2 = "test1@mail.ru";
            User2 = new IdentityUser
            {
                UserName = EMAIL2,
                NormalizedUserName = EMAIL2.ToUpper(),
                Email = EMAIL2,
                NormalizedEmail = EMAIL2.ToUpper(),
                EmailConfirmed = true,
                SecurityStamp = new Guid().ToString(),
            };

            Project = new Project
            {
                Name = "TestProject",
                Owner = User1,
            };

            Context.Add(User1);
            Context.Add(User2);
            Context.Add(Project);

            Context.SaveChanges();
        }

        [Test]
        public void TestProjectExtensions()
        {
            Assert.True(User1.CanView(Project, Context));
            Assert.True(User1.CanEdit(Project, Context));
            Assert.True(User1.CanDelete(Project, Context));

            Assert.False(User2.CanView(Project, Context));
            Assert.False(User2.CanEdit(Project, Context));
            Assert.False(User2.CanDelete(Project, Context));
        }

        [Test]
        public void TestProjectMemberExtensions()
        {
            var projectMember = new ProjectMember
            {
                NameInProject = "TestUser",
                Project = Project,
            };

            Context.Add(projectMember);
            Context.SaveChanges();

            Assert.True(User1.CanView(projectMember, Context));
            Assert.True(User1.CanEdit(projectMember, Context));
            Assert.True(User1.CanDelete(projectMember, Context));

            Assert.False(User2.CanView(projectMember, Context));
            Assert.False(User2.CanEdit(projectMember, Context));
            Assert.False(User2.CanDelete(projectMember, Context));
        }

        [Test]
        public void TestCategoryExtensions()
        {
            var category = new Category
            {
                Name = "TestUser",
                Project = Project,
            };

            Context.Add(category);
            Context.SaveChanges();

            Assert.True(User1.CanView(category, Context));
            Assert.True(User1.CanEdit(category, Context));
            Assert.True(User1.CanDelete(category, Context));

            Assert.False(User2.CanView(category, Context));
            Assert.False(User2.CanEdit(category, Context));
            Assert.False(User2.CanDelete(category, Context));
        }

        [Test]
        public void TestFinOperationExtensions()
        {
            var finOperation = new FinOperation
            {
                Project = Project,
            };

            Context.Add(finOperation);
            Context.SaveChanges();

            Assert.True(User1.CanView(finOperation, Context));
            Assert.True(User1.CanEdit(finOperation, Context));
            Assert.True(User1.CanDelete(finOperation, Context));

            Assert.False(User2.CanView(finOperation, Context));
            Assert.False(User2.CanEdit(finOperation, Context));
            Assert.False(User2.CanDelete(finOperation, Context));
        }
    }
}