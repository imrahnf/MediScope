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
    
    public AccountController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }

    // Sign In
    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Login(string username, string password)
    {
        var result = await _signInManager.PasswordSignInAsync(username, password, false, false);

        if (!result.Succeeded)
        {
            ViewBag.Error = "Invalid username or password";
            return View();
        }
        
        var user = await _userManager.FindByNameAsync(username);
        var roles = await _userManager.GetRolesAsync(user);
        
        if (roles.Contains("Patient"))
        {
            return RedirectToAction("Index", "Patient");
        } else if (roles.Contains("Doctor"))
        {
            return RedirectToAction("Index", "Doctor");
        } else if (roles.Contains("Admin"))
        {
            return RedirectToAction("Index", "Admin");
        }
        return RedirectToAction("Login", "Account");
    }
    
    // Logout
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Login");
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
            // Fallback
            return RedirectToAction("Index", "Home");
        }
        return View();
    }
}