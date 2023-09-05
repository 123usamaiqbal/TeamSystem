

using System.ComponentModel.DataAnnotations;

namespace TeamManageSystem.Models.Account
{
    public class Task
    {
        public int Id { get; set; }
        public int SprintId { get; set; }
        public string Title { get; set; }
        public int AssigneeId { get; set; }
        public string AssigneeName { get; set; }
        [Display(Name ="Task Rating")]
        public int TaskRate { get; set; }

    }
}
