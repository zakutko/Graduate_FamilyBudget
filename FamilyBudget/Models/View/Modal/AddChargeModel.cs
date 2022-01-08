using FamilyBudget.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyBudget.ModalViewModels
{
    public class AddChargeModel
    {
        public FinOperation finOperation { get; set; }
        public bool IsValid { get { return !(IsNotFound || IsForbid); } }
        public bool IsNotFound { get; set; }
        public bool IsForbid { get; set; }
    }
}
