using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace TeamManageSystem.Models.ViewModel
{
    public class SignUpUserViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage ="Please enter username")]
        //[Remote(action: "UserNameIsExist",controller:"AccountController")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Please enter email")]
        [RegularExpression(@"[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,4}" , ErrorMessage ="please enter valid email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please enter mobile number")]
        [Display(Name = "Mobile Number")]
        public string Mobile { get; set; }

        [Required(ErrorMessage = "Please enter password")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Please enter confirm password")]
        [Compare("Password",ErrorMessage = ("confirm password not matched!"))]
        [Display(Name="Confirm Password")]
        public string ConfirmPassword { get; set; }

       

    }
}
