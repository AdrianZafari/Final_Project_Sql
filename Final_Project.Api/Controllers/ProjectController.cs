using Microsoft.AspNetCore.Mvc;

namespace Final_Project.Api.Controllers;

public class ProjectController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
