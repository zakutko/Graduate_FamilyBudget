using FamilyBudget.Models.View.Filter;
using FamilyBudget.Models.View.Page;
using FamilyBudget.Models.View.Sort;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyBudget.Models.View.Details
{
    public class HomeProjectDetailsModel
    {
        public IEnumerable<FinOperation> finOperations { get; set; }
        public FinOperationPageModel PageViewModel { get; set; }
        public FinOperationFilterModel FilterViewModel { get; set; }
        public FinOperationSortModel SortViewModel { get; set; }
        public bool isEmpty { get { return !finOperations.Any(); } }
    }
}
