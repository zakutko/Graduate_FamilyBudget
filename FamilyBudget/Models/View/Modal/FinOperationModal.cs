using FamilyBudget.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace FamilyBudget.ModalViewModels
{
    public class FinOperationModal
    {
        public int Id { get; set; }
        public FinType FinType { get; set; }
        public bool ForAll { get; set; }

        public int Value { get; set; }

        public int? CategoryId { get; set; }
        public Category Category { get; set; }

        public int? ProjectMemberId { get; set; }
        public ProjectMember ProjectMember { get; set; }
        public SelectList ProjectMembers { get; set; }

        public int ProjectId { get; set; }
        public Project Project { get; set; }

        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }

        public bool IsValid { get { return !(IsNotFound || IsForbid); } }
        public bool IsNotFound { get; set; }
        public bool IsForbid { get; set; }
    }
}
