using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BootstrapHtmlHelpers.Models;
using Web;

namespace BootstrapHtmlHelpers.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View(new TheModelWithEverything {
            Birthday = DateTime.Now,
            CanYouEvenAgain = true,
            SelectEnumQ = SomeOptions.Agree,
            SelectEnumNullableQ = SomeOptions.Disagree
        });
    }

    [HttpPost]
    public IActionResult Index([FromForm] TheModelWithEverything model)
    {
        return View(model);
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
