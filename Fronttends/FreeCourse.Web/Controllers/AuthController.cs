using FreeCourse.Web.Models;
using FreeCourse.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace FreeCourse.Web.Controllers
{
    public class AuthController : Controller
    {

        private readonly IdentityService _identiyService;

        public AuthController(IdentityService identiyService)
        {
            _identiyService = identiyService;
        }

        public IActionResult SignIn()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignIn(SignInInput signInInput)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var response =await _identiyService.SignIn(signInInput);
            if (!response.IsSuccessful)
            {
                foreach (var error in response.Errors)
                {
                    ModelState.AddModelError(string.Empty, error);
                }

                return View();
            }

            return RedirectToAction(nameof(Index), "Home");
        }
    }
}
