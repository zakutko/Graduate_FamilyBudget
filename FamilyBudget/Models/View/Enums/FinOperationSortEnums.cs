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
        ProjectMember,
        FinType,
        Category,
        CreateTime,
        Value
    }
}
