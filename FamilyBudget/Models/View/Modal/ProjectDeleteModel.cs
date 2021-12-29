using FamilyBudget.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyBudget.ModalViewModels
{
    public class ProjectDeleteModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public IdentityUser Owner { get; set; }

        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }

        public bool IsValid { get { return !(IsNotFound || IsForbid); } }
        public bool IsNotFound { get; set; }
        public bool IsForbid { get; set; }

        public int membersCount { get; set; }
        public int incomesCount { get; set; }
        public int chargesCount { get; set; }
    }
}
