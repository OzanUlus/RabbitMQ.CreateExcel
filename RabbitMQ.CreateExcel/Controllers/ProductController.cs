using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.CreateExcel.Models;

namespace RabbitMQ.CreateExcel.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly AppDbContext _appDbContext;

        public ProductController(UserManager<IdentityUser> userManager, AppDbContext appDbContext)
        {
            _userManager = userManager;
            _appDbContext = appDbContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> CreateProductExcel() 
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            var fileName = $"product-excel-{Guid.NewGuid().ToString().Substring(1,10)}";

            UserFile userfile = new()
            { 
              UserId = user.Id,
              FileName = fileName,
              FileStatus = FileStatus.Creating,
            };

            await _appDbContext.UserFiles.AddAsync(userfile);

            await _appDbContext.SaveChangesAsync(); 

            TempData["StartCreaingExcel"] = true;
            return RedirectToAction(nameof(Files));

        }

        public async Task<IActionResult> Files() 
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var files = await _appDbContext.UserFiles.Where(u => u.UserId == user.Id).ToListAsync();

            return View(files);
        }
    }
}
