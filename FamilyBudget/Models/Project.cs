using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyBudget.Models
{
    public class Project
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string OwnerId { get; set; }
        public IdentityUser Owner { get; set; }

        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }

        public List<ProjectMember> ProjectMembers { get; set; } = new List<ProjectMember>();
        public List<FinOperation> FinOperations { get; set; } = new List<FinOperation>();
        public List<Category> Categories { get; set; } = new List<Category>();
    }
}
