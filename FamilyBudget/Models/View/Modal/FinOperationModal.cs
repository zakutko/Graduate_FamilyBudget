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

        [Display(Name = "Type")]
        public FinType FinType { get; set; }
        public bool ForAll { get; set; }

        public int Value { get; set; }

        [Display(Name = "Category")]
        public int? CategoryId { get; set; }
        public Category Category { get; set; }

        [Display(Name = "Project member")]
        public int? ProjectMemberId { get; set; }
        public ProjectMember ProjectMember { get; set; }
        public SelectList ProjectMembers { get; set; }

        public int ProjectId { get; set; }
        public Project Project { get; set; }

        [Display(Name = "Create time")]
        public DateTime CreateTime { get; set; }

        [Display(Name = "Update time")]
        public DateTime UpdateTime { get; set; }

        public bool IsValid { get { return !(IsNotFound || IsForbid); } }
        public bool IsNotFound { get; set; }
        public bool IsForbid { get; set; }
    }
}
