using System.ComponentModel.DataAnnotations;

namespace TeamManageSystem.Models.Account
{
    public class Rating
    {
        [Key]
        public int Id { get; set; }
        public int MemberId { get; set; }
        [Display(Name = "Member Name")]
        public string MemberName { get; set; }
        [Display(Name = "Rating")]
        public int Ratings { get; set; }
        
        public string FeedBack { get; set; }
    
    }
}
