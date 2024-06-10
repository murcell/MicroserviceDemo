using System.ComponentModel.DataAnnotations;

namespace FreeCourse.Web.Models
{
    public class SignInInput
    {
        [Display(Name ="Email Adresiniz")]
        public string Email { get; set; }
        [Display(Name = "Şireniz")]
        public string Password { get; set; }
        [Display(Name = "Beni Hatırla")]
        public bool IsRemember { get; set; }
    }
}
