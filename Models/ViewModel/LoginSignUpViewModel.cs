using System.ComponentModel.DataAnnotations;

namespace TeamManageSystem.Models.ViewModel
{
    public class LoginSignUpViewModel
    {

        public string Username { get; set; }
        
        public string Password { get; set; }
       
        [Display (Name ="Rmemeber Me")]
        public bool IsRemember { get; set; }
    }
}
