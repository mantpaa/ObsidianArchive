using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ObsidianArchive.Utility;

namespace ObsidianArchiveWeb.Areas.Admin.Controllers
{
    [Authorize(Roles = StaticDetails.RoleAdmin)]
    [Area("Admin")]
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
