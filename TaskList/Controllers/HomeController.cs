using Microsoft.AspNetCore.Mvc;
// using TaskList.Models;

namespace TaskList.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet("/")]
        public ActionResult Index()
        {
            return View();
        }
    }
}
