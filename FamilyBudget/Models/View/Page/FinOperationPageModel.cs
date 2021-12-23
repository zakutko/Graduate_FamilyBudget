using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyBudget.Models.View.Page
{
    public class FinOperationPageModel
    {
        public int PageNumber { get; private set; }
        public int TotalPages { get; private set; }

        public FinOperationPageModel(int count, int pageNumber, int pageSize)
        {
            PageNumber = pageNumber;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        }

        public bool HasPreviousPage { get { return (PageNumber > 1); } }

        public bool HasNextPage { get { return (PageNumber < TotalPages); } }
    }
}
