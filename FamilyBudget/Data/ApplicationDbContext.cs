using FamilyBudget.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using FamilyBudget.ModalViewModels;

namespace FamilyBudget.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectMember> ProjectMembers { get; set; }
        public DbSet<FinOperation> FinOperations { get; set; }
        public DbSet<Category> Categories { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }

        public DbSet<FamilyBudget.ModalViewModels.ProjectDeleteModel> ProjectDeleteModel { get; set; }

        public DbSet<FamilyBudget.ModalViewModels.FinOperationModal> FinOperationModal { get; set; }
    }
}
