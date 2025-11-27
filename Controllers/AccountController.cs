using MediScope.Identity;
using MediScope.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MediScope.Controllers;


public class AccountController : Controller
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly LoggingService _logging;
    
    public AccountController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, LoggingService logging)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _logging = logging;
    }

    // Sign In
    [HttpPost]
    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Login(string username, string password)
    {
        var result = await _signInManager.PasswordSignInAsync(username, password, false, false);

        if (!result.Succeeded)
        {
            ViewBag.Error = "Invalid username or password";
            return View();
        }

        // log successful login
        await _logging.AddAsync($"User '{username}' signed in");
        var user = await _userManager.FindByNameAsync(username);
        var roles = await _userManager.GetRolesAsync(user);

        if (roles.Contains("Admin"))
        {
            return RedirectToAction("Index", "Admin");
        }
        else if (roles.Contains("Doctor"))
        {
            return RedirectToAction("Index", "Doctor");
        }
        else if (roles.Contains("Patient"))
        {
            return RedirectToAction("Index", "Patient");
        }

        // fallback
        return RedirectToAction("Login");
    }

    // Logout
    [AllowAnonymous]
    public async Task<IActionResult> Logout()
    {
        // attempt to identify the user who is logging out
        var user = await _userManager.GetUserAsync(User);
        var uname = user?.UserName ?? "Unknown";
        await _logging.AddAsync($"User '{uname}' logged out");

        await _signInManager.SignOutAsync();
        return RedirectToAction("Login", "Account");
    }
    
    // Base Login Page
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Login()
    {
        if (User?.Identity?.IsAuthenticated == true)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                var roles = await _userManager.GetRolesAsync(user);
                if (roles.Contains("Patient")) return RedirectToAction("Index", "Patient");
                if (roles.Contains("Doctor")) return RedirectToAction("Index", "Doctor");
                if (roles.Contains("Admin")) return RedirectToAction("Index", "Admin");
            }
            // Fallback - redirect to login to avoid loops
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }
        return View();
    }
}