using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyBudget.Models
{
    public class ProjectMember
    {
        public int Id { get; set; }

        [Required]
        public string NameInProject { get; set; }

        public string UserId { get; set; }
        public IdentityUser User { get; set; }

        public int ProjectId { get; set; }
        public Project Project { get; set; }
    }
}
