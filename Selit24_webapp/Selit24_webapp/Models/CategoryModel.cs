using System.ComponentModel.DataAnnotations;

namespace Selit24_webapp.Models
{
    public class CategoryModel
    {
        
        public long Categoryid { get; set; }
        [Required(ErrorMessage = "Category Name is Required.")]
        public string? Categoryname { get; set; }

        [Required(ErrorMessage = "Category Image is Required.")]
        public IFormFile? Categoryimage { get; set; }
        public string? CategoryImageBase64 { get; set; }
    }
}
