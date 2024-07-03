using System.ComponentModel.DataAnnotations;

namespace Selit24_webapp.Models
{
    public class UserInfoModel
    {
        //Tbluser
        [Required (ErrorMessage = "First Name is Required.")]
        public string? Firstname { get; set; } = null!;
        [Required(ErrorMessage = "Last Name is Required.")]
        public string? Lastname { get; set; } = null!;

        public string? Locationcountry { get; set; } = null!;

        public string? Locationstate { get; set; } = null!;

        public string? Locationdistrict { get; set; } = null!;

        public string? Locationlocality { get; set; } = null!;

        public int? Locationpincode { get; set; }

        [Required(ErrorMessage = "Profile Image is Required.")]
        public IFormFile? Profileimage { get; set; }
        public string? ProfileImageBase64 { get; set; }


        //Tbluserlogin
        public long Userid { get; set; }
        [Required(ErrorMessage = "Email is Required.")]
        public string? Useremail { get; set; } = null!;

        [Required(ErrorMessage = "Password is Required.")]
        public string? Userpassword { get; set; } = null!;
        public bool Isdeleted { get; set; }
        public bool Activestatus { get; set; }

    }
}
