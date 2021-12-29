using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyBudget.Models.View.Filter
{
    public class FinOperationFilterModel
    {
        public FinOperationFilterModel(int? project, string category, FinType? finType)
        {
            var finTypes = Enum.GetNames(typeof(FinType)).ToList();
            finTypes.Insert(0, "All");
            FinTypes = new SelectList(finTypes,finType);
            SelectedProject = project;
            SelectedFinType = finType;
            SelectedCategory = category;
        }
        public SelectList FinTypes { get; private set; }
        public int? SelectedProject { get; private set; }

        public FinType? SelectedFinType { get; private set; }
        public string SelectedCategory { get; private set; }
    }

}
