using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyBudget.Models.View.Enums
{
    public enum FinOperationSortDirEnum
    { 
        Ascending,
        Descending
    }

    public enum FinOperationSortModelEnum
    {
        Default,
        FinType,
        Category,
        CreateTime,
        Value
    }
}
