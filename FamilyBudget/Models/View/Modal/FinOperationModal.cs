using FamilyBudget.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FamilyBudget.ModalViewModels
{
    public class FinOperationModal
    {
        public int Id { get; set; }

        [Display(Name = "Тип")]
        public FinType FinType { get; set; }
        public bool ForAll { get; set; }

        [Display(Name = "Сумма")]
        public int Value { get; set; }

        [Display(Name = "Категория")]
        public int? CategoryId { get; set; }

        [Display(Name = "Категория")]
        public Category Category { get; set; }

        [Display(Name = "Член семьи")]
        public int? ProjectMemberId { get; set; }

        [Display(Name = "Член семьи")]
        public ProjectMember ProjectMember { get; set; }
        public SelectList ProjectMembers { get; set; }

        public int ProjectId { get; set; }
        public Project Project { get; set; }

        [Display(Name = "Дата создания")]
        public DateTime CreateTime { get; set; }

        [Display(Name = "Дата обновления")]
        public DateTime UpdateTime { get; set; }

        public bool IsValid { get { return !(IsNotFound || IsForbid); } }
        public bool IsNotFound { get; set; }
        public bool IsForbid { get; set; }
    }
}
