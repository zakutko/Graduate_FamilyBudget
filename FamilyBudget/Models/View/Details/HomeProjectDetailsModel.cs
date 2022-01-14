using FamilyBudget.Models.View.Filter;
using FamilyBudget.Models.View.Page;
using FamilyBudget.Models.View.Sort;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyBudget.Models.View.Details
{
    public class HomeProjectDetailsModel
    {
        public class PieItem
        {
            public string Name;
            public int Value;
        }
        public string ProjectName { get; set; }

        public SelectList ProjectMembers { get; set; }
        public IEnumerable<PieItem> PieByCategory { get; set; }
        public IEnumerable<PieItem> PieByProjectMember { get; set; }
        public IEnumerable<PieItem> PieByFinOp { get; set; }
        public IEnumerable<FinOperation> FinOperations { get; set; }
        public FinOperationPageModel PageViewModel { get; set; }
        public FinOperationFilterModel FilterViewModel { get; set; }
        public FinOperationSortModel SortViewModel { get; set; }
        public bool IsEmpty { get { return !FinOperations.Any(); } }
    }
}
