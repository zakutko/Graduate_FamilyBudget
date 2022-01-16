using FamilyBudget.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyBudget.ModalViewModels
{
    public class ProjectDeleteModel
    {
        public int Id { get; set; }

        [Display(Name = "Имя")]
        public string Name { get; set; }

        [Display(Name = "Владелец")]
        public IdentityUser Owner { get; set; }

        [Display(Name = "Дата создания")]
        public DateTime CreateTime { get; set; }

        [Display(Name = "Дата обновления")]
        public DateTime UpdateTime { get; set; }

        public bool IsValid { get { return !(IsNotFound || IsForbid); } }
        public bool IsNotFound { get; set; }
        public bool IsForbid { get; set; }

        [Display(Name = "Количество членов семьи")]
        public int MembersCount { get; set; }

        [Display(Name = "Количество доходов")]
        public int IncomesCount { get; set; }

        [Display(Name = "Количество расходов")]
        public int ChargesCount { get; set; }
    }
}
