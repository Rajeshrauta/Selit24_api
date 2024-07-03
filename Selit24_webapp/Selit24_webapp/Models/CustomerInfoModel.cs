using System.ComponentModel.DataAnnotations;

namespace Selit24_webapp.Models
{
    public class CustomerInfoModel
    {
        [Required(ErrorMessage = "First Name is Required.")]
        public string? Firstname { get; set; } = null!;
        [Required(ErrorMessage = "Last Name is Required.")]

        public string? Lastname { get; set; } = null!;
        [Required(ErrorMessage = "Profile Image is Required.")]
        public byte[]? Profileimage { get; set; }

        public string? Locationcountry { get; set; } = null!;

        public string? Locationstate { get; set; } = null!;

        public string? Locationdistrict { get; set; } = null!;

        public string? Locationlocality { get; set; } = null!;

        public int? Locationpincode { get; set; }

        //Customer LogIn

        public long Customerid { get; set; }
        [Required(ErrorMessage = "Phone Number is Required.")]
        public long? Phonenumber { get; set; }
        [Required(ErrorMessage = "Email is Required.")]
        public string? Customeremail { get; set; } = null!;

        public bool Isdeleted { get; set; }

        public bool Accountstatus { get; set; }
    }
}
