using Microsoft.AspNetCore.Mvc;

namespace ObsidianArchiveWeb.Controllers
{
    public class CategoryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
