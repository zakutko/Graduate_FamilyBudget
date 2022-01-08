using FamilyBudget.Models.View.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyBudget.Models.View.Sort
{
    public class FinOperationSortModel
    {
        public FinOperationSortDirEnum CurrentSortDir { get; set; }
        public FinOperationSortModelEnum CurrentSortModel { get; private set; }
        public bool categorySort { get { return CurrentSortModel == FinOperationSortModelEnum.Category; } } 
        public bool valueSort { get { return CurrentSortModel == FinOperationSortModelEnum.Value; } }
        public bool createTimeSort { get { return CurrentSortModel == FinOperationSortModelEnum.CreateTime; } }
        public bool finTypeSort { get { return CurrentSortModel == FinOperationSortModelEnum.FinType; } }

        public bool isAscending { get { return CurrentSortDir == FinOperationSortDirEnum.Ascending; } }

        public bool sortModelChanged(FinOperationSortModelEnum selectedSortModel)  { return selectedSortModel == CurrentSortModel; }
        public FinOperationSortDirEnum changeSortDir(FinOperationSortDirEnum sortDir) { return sortDir == FinOperationSortDirEnum.Descending ?
                FinOperationSortDirEnum.Ascending : FinOperationSortDirEnum.Descending;
        }

        public FinOperationSortModel(FinOperationSortModelEnum sortModel, FinOperationSortDirEnum sortDir)
        {
            CurrentSortDir = sortDir;
            CurrentSortModel = sortModel;
        }
    }

}
