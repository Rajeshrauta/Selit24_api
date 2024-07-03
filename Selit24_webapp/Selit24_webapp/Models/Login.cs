using System.ComponentModel.DataAnnotations;

namespace Selit24_webapp.Models
{
    public class Login
    {
        [Required(ErrorMessage = "Please enter your email.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Please enter password")]
        public string Password { get; set; }
        public bool KeepLoggedIN { get; set; }
        [Required(ErrorMessage = "Please enter OTP")]
        public int OTP { get; set; }
        
    }
}
