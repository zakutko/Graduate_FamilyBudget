using FamilyBudget.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.ComponentModel.DataAnnotations;

namespace FamilyBudget.ModalViewModels
{
    public class ProjectMemberModel
    {
        public int Id { get; set; }

        [Display(Name = "Имя")]
        public string NameInProject { get; set; }
        public string Email { get; set; }

        [Display(Name = "Пользователь")]
        public int UserId { get; set; }
        public IdentityUser User { get; set; }

        public SelectList Users { get; set; }
        public int ProjectId { get; set; }
        public bool IsValid { get { return !(IsNotFound || IsForbid); } }
        public bool IsNotFound { get; set; }
        public bool IsForbid { get; set; }
    }
}
