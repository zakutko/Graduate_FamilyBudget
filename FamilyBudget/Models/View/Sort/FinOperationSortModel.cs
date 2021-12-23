using FamilyBudget.Models.View.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyBudget.Models.View.Sort
{
    public class FinOperationSortModel
    {
        public FinOperationSortEnum FinTypeSort { get; private set; } 
        public FinOperationSortEnum CreateTimeSort { get; private set; }   
        public FinOperationSortEnum CategorySort { get; private set; }
        public FinOperationSortEnum ValueSort { get; private set; }
        public FinOperationSortEnum Current { get; private set; }     

        public FinOperationSortModel(FinOperationSortEnum sortOrder)
        {
            FinTypeSort = sortOrder == FinOperationSortEnum.FinTypeAsc ? FinOperationSortEnum.FinTypeDesc : FinOperationSortEnum.FinTypeAsc;
            CreateTimeSort = sortOrder == FinOperationSortEnum.CreateTimeAsc ? FinOperationSortEnum.CreateTimeDesc : FinOperationSortEnum.CreateTimeAsc;
            CategorySort = sortOrder == FinOperationSortEnum.CategoryAsc ? FinOperationSortEnum.CategoryDesc : FinOperationSortEnum.CategoryAsc;
            ValueSort = sortOrder == FinOperationSortEnum.ValueAsc ? FinOperationSortEnum.ValueDesc : FinOperationSortEnum.ValueAsc;
            Current = sortOrder;
        }
    }

}
