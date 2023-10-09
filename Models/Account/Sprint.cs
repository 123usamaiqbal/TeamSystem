using System.ComponentModel.DataAnnotations;

namespace TeamManageSystem.Models.Account
{
    public class Sprint
    {
        [Key]
        public int Id { get; set; }

        public string Discription { get; set; }
        public int SprintNo { get; set; }
        public string Title { get; set; }
        public int CreatedBy { get; set; }

        [Display(Name = "Created Time")]
        public DateTime CreatedTime { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Start Date")]
        [Required]
        public DateTime SDate { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "End Date")]
        [Required]
        public DateTime EDate { get; set; }


    }
}
