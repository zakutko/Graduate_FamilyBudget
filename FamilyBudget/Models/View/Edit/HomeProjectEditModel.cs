using System.Collections.Generic;

namespace FamilyBudget.Models.View.Edit
{
    public class HomeProjectEditModel
    {
        public int projectId { get; set; }
        public Project project { get; set; }
        public List<ProjectMember> projectMembers{ get; set; }
        public List<Category> categories { get; set; }
    }
}
