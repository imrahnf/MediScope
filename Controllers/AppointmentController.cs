using Microsoft.AspNetCore.Mvc;

namespace MediScope.Controllers;

public class AppointmentController : Controller
{
    // Simple logic to validate whether the right user is in the place
    public bool IsValidUser(string role)
    {
        var userRole = HttpContext.Session.GetString("Role");
        return userRole == role;
    }
    
    public IActionResult Book()
    {
        if (!IsValidUser("Patient"))
        {
            return RedirectToAction("Login", "Account");
        }
        
        return View();
    }
}