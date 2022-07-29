using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace DiabetesPredictor.Web.Models
{
    public class LoginModel
    {
    
        
            [Required]
            [EmailAddress]
            [Display(Name = "Email Address")]
            public string Email { get; set; }

       
            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

         
            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
    }
}
