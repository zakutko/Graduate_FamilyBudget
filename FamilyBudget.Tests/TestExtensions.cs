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
        Category Category;
        FinOperation FinOperation;
        ProjectMember ProjectMember;

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

            Category = new Category
            {
                Name = "TestUser",
                Project = Project,
            };

            FinOperation = new FinOperation
            {
                Project = Project,
            };

            ProjectMember = new ProjectMember
            {
                NameInProject = "TestUser",
                Project = Project,
            };

            Context.Add(User1);
            Context.Add(User2);
            Context.Add(Project);
            Context.Add(Category);
            Context.Add(FinOperation);
            Context.Add(ProjectMember);

            Context.SaveChanges();
        }

        [Test]
        public void TestProjectExtensionsCanViewTrue()
        {
            Assert.True(User1.CanView(Project, Context));
        }

        [Test]
        public void TestProjectExtensionsCanViewFalse()
        {
            Assert.False(User2.CanView(Project, Context));
        }

        [Test]
        public void TestProjectExtensionsCanEditTrue()
        {
            Assert.True(User1.CanEdit(Project, Context));
        }

        [Test]
        public void TestProjectExtensionsCanEditFalse()
        {
            Assert.False(User2.CanEdit(Project, Context));
        }

        [Test]
        public void TestProjectExtensionsCanDeleteTrue()
        {
            Assert.True(User1.CanDelete(Project, Context));
        }

        [Test]
        public void TestProjectExtensionsCanDeleteFalse()
        {
            Assert.False(User2.CanDelete(Project, Context));
        }

        [Test]
        public void TestProjectMemberExtensionsCanViewTrue()
        {
            Assert.True(User1.CanView(ProjectMember, Context));
        }

        [Test]
        public void TestProjectMemberExtensionsCanViewFalse()
        {
            Assert.False(User2.CanView(ProjectMember, Context));
        }

        [Test]
        public void TestProjectMemberExtensionsCanEditTrue()
        {
            Assert.True(User1.CanEdit(ProjectMember, Context));
        }

        [Test]
        public void TestProjectMemberExtensionsCanEditFalse()
        {
            Assert.False(User2.CanEdit(ProjectMember, Context));
        }

        [Test]
        public void TestProjectMemberExtensionsCanDeleteTrue()
        {
            Assert.True(User1.CanDelete(ProjectMember, Context));
        }

        [Test]
        public void TestProjectMemberExtensionsCanDeleteFalse()
        {
            Assert.False(User2.CanDelete(ProjectMember, Context));
        }

        [Test]
        public void TestCategoryExtensionsCanViewTrue()
        {
            Assert.True(User1.CanView(Category, Context));
        }

        [Test]
        public void TestCategoryExtensionsCanViewFalse()
        {
            Assert.False(User2.CanView(Category, Context));
        }

        [Test]
        public void TestCategoryExtensionsCanEditTrue()
        {
            Assert.True(User1.CanEdit(Category, Context));
        }

        [Test]
        public void TestCategoryExtensionsCanEditFalse()
        {
            Assert.False(User2.CanEdit(Category, Context));
        }

        [Test]
        public void TestCategoryExtensionsCanDeleteTrue()
        {
            Assert.True(User1.CanDelete(Category, Context));
        }

        [Test]
        public void TestCategoryExtensionsCanDeleteFalse()
        {
            Assert.False(User2.CanDelete(Category, Context));
        }

        [Test]
        public void TestFinOperationExtensionsCanViewTrue()
        {
            Assert.True(User1.CanView(FinOperation, Context));
        }

        [Test]
        public void TestFinOperationExtensionsCanViewFalse()
        {
            Assert.False(User2.CanView(FinOperation, Context));
        }

        [Test]
        public void TestFinOperationExtensionsCanEditTrue()
        {
            Assert.True(User1.CanEdit(FinOperation, Context));
        }

        [Test]
        public void TestFinOperationExtensionsCanEditFalse()
        {
            Assert.False(User2.CanEdit(FinOperation, Context));
        }

        [Test]
        public void TestFinOperationExtensionsCanDeleteTrue()
        {
            Assert.True(User1.CanDelete(FinOperation, Context));
        }

        [Test]
        public void TestFinOperationExtensionsCanDeleteFalse()
        {
            Assert.False(User2.CanDelete(FinOperation, Context));
        }
    }
}