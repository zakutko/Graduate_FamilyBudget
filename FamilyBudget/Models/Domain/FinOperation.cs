using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyBudget.Models
{
    public class FinOperation
    {
        public int Id { get; set; }

        [Required]
        public FinType FinType { get; set; }

        [Required]
        public bool ForAll { get; set; }

        [Required]
        public int Value { get; set; }

        public int? CategoryId { get; set; }
        public Category Category { get; set; }

        public int? ProjectMemberId { get; set; }
        public ProjectMember ProjectMember { get; set; }

        public int ProjectId { get; set; }
        public Project Project { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }
    }

    public enum FinType
    {
        [Display(Name = "Income")] Income,
        [Display(Name = "Charge")] Charge,
    }
}
