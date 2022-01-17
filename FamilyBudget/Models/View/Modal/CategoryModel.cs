using FamilyBudget.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.ComponentModel.DataAnnotations;

namespace FamilyBudget.ModalViewModels
{
    public class CategoryModel
    {
        public int Id { get; set; }
        [Display(Name = "Название")]
        public string Name { get; set; }

        public int ProjectId { get; set; }
        public bool IsValid { get { return !(IsNotFound || IsForbid); } }
        public bool IsNotFound { get; set; }
        public bool IsForbid { get; set; }
    }
}
