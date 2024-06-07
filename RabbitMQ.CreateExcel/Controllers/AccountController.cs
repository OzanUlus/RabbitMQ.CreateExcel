using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace RabbitMQ.CreateExcel.Controllers
{
   

    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(string Email , string Password) 
        {
            var user = await _userManager.FindByEmailAsync(Email);
            if (user == null) 
            {
               ModelState.AddModelError("","Kullanıcı bulunumadı");
                return View();
            }

            var result = await _signInManager.PasswordSignInAsync(user,Password,true ,false);
            if (!result.Succeeded) 
            {
                ModelState.AddModelError("", "Kullanıcı adı veya şifre hatalı");
                return View();
            }

            return RedirectToAction(nameof(HomeController.Index), "Home");
        }
    }
}
