using Microsoft.AspNetCore.Mvc;

namespace GolBet.Web.Controllers;

public class ErrorController : Controller
{
    [Route("Error/{statusCode:int}")]
    public IActionResult Handle(int statusCode)
    {
        Response.StatusCode = statusCode;
        return View(statusCode == 404 ? "NotFound" : "General");
    }

    [Route("Error/General")]
    public IActionResult General()
    {
        Response.StatusCode = 500;
        return View();
    }
}
