using FamilyBudget.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyBudget.ModalViewModels
{
    public class AddFinOpModel
    {
        public FinType FinType { get; set; }

        public bool ForAll { get; set; }

        [Display(Name = "Сумма")]
        public int Value { get; set; }

        [Display(Name = "Категория")]
        public int? CategoryId { get; set; }
        public SelectList ProjectMembers { get; set; }

        [Display(Name = "Член семьи")]
        public int? ProjectMemberId { get; set; }
        public int ProjectId { get; set; }
        public bool IsValid { get { return !(IsNotFound || IsForbid); } }
        public bool IsNotFound { get; set; }
        public bool IsForbid { get; set; }
    }
}
