using Microsoft.AspNetCore.Mvc;
using WebCargo.Models;

namespace WebCargo.Controllers
{
    public class UsersController : Controller
    {
        public IActionResult Index(
            string? search,
            int page = 1)
        {
            string? role =
                HttpContext.Session.GetString(
                "Role"
                );

            if (role != "Admin")
            {
                return RedirectToAction(
                    "Index",
                    "Cargo"
                );
            }
            const int pageSize = 5;

            List<User> users =
                new();

            if (!string.IsNullOrWhiteSpace(
                search))
            {
                User? user =
                    Database.GetUserByEmail(
                        search
                    );

                if (user != null)
                {
                    users.Add(
                        user
                    );
                }

                ViewBag.HasNextPage =
                    false;

                ViewBag.Page =
                    1;

                return View(users);
            }

            users =
                Database.GetUserPage(
                    page,
                    pageSize
                );

            ViewBag.HasNextPage =
                users.Count > pageSize;

            if (users.Count > pageSize)
            {
                users.RemoveAt(
                    pageSize
                );
            }

            ViewBag.Page =
                page;

            return View(users);
        }



        [HttpPost]
        public IActionResult Create(
    string email)
        {
            User? user =
                Database.GetUserByEmail(
                    email
                );

            if (user != null)
            {
                TempData["Error"] =
                    "Bu email zaten mevcut.";

                return RedirectToAction(
                    "Index"
                );
            }

            Database.CreateInvitedUser(
                email
            );

            TempData["Success"] =
                "Kullanıcı davet edildi.";

            return RedirectToAction(
                "Index"
            );
        }

        [HttpPost]
        public IActionResult Delete(
    int id)
        {
            Database.DeleteUser(
                id
            );

            TempData["Success"] =
                "Kullanıcı silindi.";

            return RedirectToAction(
                "Index"
            );
        }
    }
}