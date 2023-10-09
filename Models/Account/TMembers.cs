using System.ComponentModel.DataAnnotations;

namespace TeamManageSystem.Models.Account
{
    public class TMembers
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [Display(Name = "Member Name")]
        public string TMname { get; set; }
        public int LeadId { get; set; }
        [Display(Name = "Position")]
        public string Role { get; set; }
        public decimal Salary { get; set; }
    }
}
