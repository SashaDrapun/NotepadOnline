using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using NotepadOnline.Models;
using NotepadOnline.ViewModels;
using System;

namespace AuthApp.Controllers
{
    public class AccountController : Controller
    {
        private ApplicationContext db;
        public AccountController(ApplicationContext context)
        {
            db = context;
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await db.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
                if (user != null)
                {
                    if (IsUserValid(user,model))
                    {
                        await Authenticate(model.Email); // аутентификация

                        user.DateLastLogin = DateTime.Now;

                        await db.SaveChangesAsync();

                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ViewData["email"] = "Пользователя с такой почтой не существует";
                }
            }
            return View(model);
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel model)
        {
             User user = await db.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
             if (user == null)
             {
                if (model.Password != model.ConfirmPassword)
                {
                    @ViewData["confirmPassword"] = "Пароли не совпадают";
                }
                else
                {
                    db.Users.Add(new User
                    {
                        Email = model.Email,
                        Password = model.Password,
                        Name = model.Name,
                        Surname = model.Surname,
                        DateRegistration = DateTime.Now,
                        DateLastLogin = DateTime.Now,
                        Status = "Разблокирован"
                    });
                    await db.SaveChangesAsync();

                    await Authenticate(model.Email); // аутентификация

                    return RedirectToAction("Index", "Home");
                }         
             }
             else
             {
                 @ViewData["email"] = "Пользователь с такой почтой уже существует";
             }

            return View(model);
        }

        private async Task Authenticate(string userName)
        {
            // создаем один claim
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, userName)
            };
            // создаем объект ClaimsIdentity
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            // установка аутентификационных куки
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }
        [NonAction]
        private bool IsUserValid(User user, LoginModel model)
        {
            if (user.Password != model.Password)
            {
                ViewData["password"] = "Пароль введен неверно";
                return false;
            }
            if (user.Status == "Заблокирован")
            {
                ViewData["main"] = "Вы заблокированы";
                return false;
            }
            return true;
        }
    }
}