using System.ComponentModel.DataAnnotations;

namespace TeamManageSystem.Models.Account
{
    public class SprintDateModel
    {
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }
        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; }

    }
}
