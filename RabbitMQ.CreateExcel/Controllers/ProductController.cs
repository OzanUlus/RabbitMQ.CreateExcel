using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.CreateExcel.Models;
using RabbitMQ.CreateExcel.Services;

namespace RabbitMQ.CreateExcel.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly AppDbContext _appDbContext;
        private readonly RabbitMQPublisher _rabbitMQPublisher;

        public ProductController(UserManager<IdentityUser> userManager, AppDbContext appDbContext, RabbitMQPublisher rabbitMQPublisher)
        {
            _userManager = userManager;
            _appDbContext = appDbContext;
            _rabbitMQPublisher = rabbitMQPublisher;
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

            _rabbitMQPublisher.Publish(new Shared.CreateExcelMessage() { FileId = userfile.Id.ToString()  });

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
