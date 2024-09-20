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

    public IActionResult Index(int page = 1)
    {
        if (ModelState.IsValid)
        {
            this.AddAlert(BootstrapColor.Success, "Data is <strong>valid</strong>.");
        }
        else
        {
            this.AddAlert(BootstrapColor.Danger, "Data is invalid.");
        }
        
        return View(new TheModelWithEverything {
            CanYouEvenAgain = true,
            SelectEnumQ = SomeOptions.Agree,
            SelectEnumNullableQ = SomeOptions.StronglyDisagree,
            ClassWithMoreStuff = new ClassWithMoreStuff(),
            CurrentPage = page,
            OptionsList = [SomeOptions.Disagree, SomeOptions.Agree]
        });
    }

    [HttpPost]
    public IActionResult Index([FromForm] TheModelWithEverything model)
    {
        if (ModelState.IsValid)
        {
            this.AddAlert(BootstrapColor.Success, "Data is valid.");
        }
        else
        {
            this.AddAlert(BootstrapColor.Danger, "Data is invalid.");
        }
        
        return View(model);
    }

    public IActionResult Alert()
    {
        TempData.AddAlert(BootstrapColor.Warning, "Warning Message");
        return RedirectToAction("Index");
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
