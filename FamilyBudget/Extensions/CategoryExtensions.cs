using FamilyBudget.Data;
using FamilyBudget.Models;
using Microsoft.AspNetCore.Identity;
using System.Linq;

namespace FamilyBudget.Extensions
{
    public static class CategoryExtensions
    {
        public static bool CanView(this IdentityUser user, Category category, ApplicationDbContext context)
        {

            var project = context.Projects
                .FirstOrDefault(c => c.Id == category.ProjectId);

            var member = context.ProjectMembers
                .Where(p => p.ProjectId == project.Id)
                .FirstOrDefault(m => m.UserId == user.Id);

            return (project.Owner == user || member != null);
        }
        public static bool CanEdit(this IdentityUser user, Category category, ApplicationDbContext context)
        {
            return CanView(user, category, context);
        }
        public static bool CanDelete(this IdentityUser user, Category category, ApplicationDbContext context)
        {
            return CanView(user, category, context);
        }
    }
}

