using TeamManageSystem.Models.ClickupModels;

namespace TeamManageSystem.Models.Account
{
    public class ViewTaskModel
    {
        public List<ClickupTaskAssignee> Assignees { get; set; }
        public List<ClikupTask> ClikupTasks { get; set; }
    }
}
