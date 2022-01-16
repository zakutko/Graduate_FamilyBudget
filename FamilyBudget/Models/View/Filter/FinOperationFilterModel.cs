using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyBudget.Models.View.Filter
{
    public class FinOperationFilterModel
    {
        public FinOperationFilterModel(int? project, string category, string projectMember, FinType? finType, DateTime? beginDate, DateTime? endDate)
        {
            var finTypes = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("All", "Любой"),
                new KeyValuePair<string, string>("Income", "Доход"),
                new KeyValuePair<string, string>("Charge", "Расход"),
            };
            FinTypes = new SelectList(finTypes, "Key", "Value", finType);
            SelectedProject = project;
            SelectedFinType = finType;
            SelectedCategory = category;
            SelectedProjectMember = projectMember;
            SelectedBeginDate = beginDate;
            SelectedEndDate = endDate;
        }
        public SelectList FinTypes { get; private set; }
        public int? SelectedProject { get; private set; }

        public FinType? SelectedFinType { get; private set; }
        public string SelectedCategory { get; private set; }

        public string SelectedProjectMember { get; private set; }

        public DateTime? SelectedBeginDate { get; private set; }
        public DateTime? SelectedEndDate { get; private set; }
    }

}
