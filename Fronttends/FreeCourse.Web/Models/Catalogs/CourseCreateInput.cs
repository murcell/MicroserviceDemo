using System.ComponentModel.DataAnnotations;

namespace FreeCourse.Web.Models.Catalogs
{
    public class CourseCreateInput
    {
        [Display(Name = "Kurs ismi")]
        [Required]
        public string Name { get; set; }

        [Display(Name = "Kurs açıklama")]
        [Required]
        public string Description { get; set; }

        [Display(Name = "Kurs fiyat")]
        [Required]
        public decimal Price { get; set; }

        // validationa takılıyor
       // [Required(AllowEmptyStrings = false)]
        public string Picture { get; set; }

       // [Required(AllowEmptyStrings = false)]
        public string UserId { get; set; }
        [Required]
        public FeatureViewModel Feature { get; set; }

        [Display(Name = "Kurs kategori")]
        [Required]
        public string CategoryId { get; set; }

        [Display(Name = "Kurs Resim")]
        [Required]
        public IFormFile PhotoFormFile { get; set; }
    }
}
