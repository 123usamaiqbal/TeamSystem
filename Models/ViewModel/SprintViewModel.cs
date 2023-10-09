using System.ComponentModel.DataAnnotations;

namespace TeamManageSystem.Models.ViewModel
{
    public class SprintViewModel
    {
        public int Id { get; set; }
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }
        [Display(Name ="End Date")]
        public DateTime EndDate { get; set; }

    }
}
