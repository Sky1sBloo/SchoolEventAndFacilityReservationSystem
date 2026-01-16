using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SFERS.Models;

namespace SFERS.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        if (User.Identity == null || !User.Identity.IsAuthenticated)
        {
            return View();
        }

        if (User.IsInRole("User"))
        {
            return RedirectToAction("Index", "Dashboard");
        }
        else if (User.IsInRole("Admin"))
        {
            return RedirectToAction("Index", "Analytics", new { area = "Admin" });
        }
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
