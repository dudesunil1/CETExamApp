using Microsoft.AspNetCore.Mvc;

namespace CETExamApp.Controllers
{
    public class TestController : Controller
    {
        [HttpGet]
        public IActionResult SimpleTest()
        {
            return Json(new { message = "TestController is working!", timestamp = DateTime.Now });
        }
    }
}
