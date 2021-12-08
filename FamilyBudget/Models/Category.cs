using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyBudget.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public int ProjectId { get; set; }
        public Project Project { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }
    }
}
