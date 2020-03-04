using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NotepadOnline.Models; // пространство имен моделей и контекста данных
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using Microsoft.AspNetCore.SignalR;
using System.Threading;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace NotepadOnline.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationContext db;
        private readonly IHubContext<DisconnectHub> _hubContext;
        public HomeController(ApplicationContext context, IHubContext<DisconnectHub> hubContext)
        {
            db = context;
            _hubContext = hubContext;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            bool isUserExists = await DoesUserExists();
            if (isUserExists)
            {
                ViewData["currentUser"] = User.Identity.Name;
                return View(db.Users.ToList());
            }
            else
            {
                return RedirectToAction("Logout","Account");
            }
           
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Delete(List<int> checkedId)
        {
            IQueryable<User> removedUsers = db.Users.Where(user => checkedId.Contains(user.Id));

            List<string> removedEmails = new List<string>();

            foreach (var user in removedUsers)
            {
                db.Users.Remove(user);
            }
            await db.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Lock(List<int> checkedId)
        {
            IQueryable<User> users = db.Users.Where(user => checkedId.Contains(user.Id));
            foreach (var user in users)
            {
                user.Status = "Заблокирован";
            }
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Unlock(List<int> checkedId)
        {
            IQueryable<User> users = db.Users.Where(user => checkedId.Contains(user.Id));
            foreach (var user in users)
            {
                user.Status = "Разблокирован";
            }
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        [NonAction]
        public async Task<bool> DoesUserExists()
        {
            User user = await db.Users.FirstOrDefaultAsync(u => u.Email == User.Identity.Name);

            if(user != null && user.Status != "Заблокирован")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
