using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TodoListApp.Models;
using TodoListApp.Services;

public class AuthController : Controller
{
    private readonly UserService _userService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly JsonFileService _dataService;

    public AuthController(UserService userService, IHttpContextAccessor httpContextAccessor, JsonFileService dataService)
    {
        _userService = userService;
        _httpContextAccessor = httpContextAccessor;
        _dataService = dataService;
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(string username, string password)
    {
        var user = _userService.GetUsers().FirstOrDefault(u => u.Username == username);

        if (user == null)
        {
            TempData["ErrorMessage"] = "User not found.";
            return View();
        }

        if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
        {
            TempData["ErrorMessage"] = "Invalid password.";
            return View();
        }

        var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Name, user.Username)
    };

        var identity = new ClaimsIdentity(claims, "CookieAuth");
        var principal = new ClaimsPrincipal(identity);
        await HttpContext.SignInAsync("CookieAuth", principal);

        return RedirectToAction("Index", "Todo");
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(string username, string password)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                TempData["ErrorMessage"] = "Username and password are required.";
                return View();
            }

            if (password.Length < 6)
            {
                TempData["ErrorMessage"] = "Password must be at least 6 characters long.";
                return View();
            }

            var user = _userService.CreateUser(username, password);

            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username)
        };

            var identity = new ClaimsIdentity(claims, "CookieAuth");
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync("CookieAuth", principal);

            TempData["SuccessMessage"] = "Registration successful! Welcome, " + user.Username + "!";

            return RedirectToAction("Index", "Todo");
        }
        catch (InvalidOperationException ex)
        {
            TempData["ErrorMessage"] = ex.Message;
            return View();
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = "An error occurred during registration. Please try again.";

            Console.WriteLine($"Registration error: {ex}");
            return View();
        }
    }

    [HttpPost]
    public IActionResult Logout()
    {
        _httpContextAccessor.HttpContext.SignOutAsync("CookieAuth");
        return RedirectToAction("Login");
    }

    [HttpGet]
    public IActionResult Profile()
    {
        return View(new ChangePasswordViewModel());
    }

    [HttpPost]
    public IActionResult ChangePassword(ChangePasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View("Profile", model);
        }

        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        var users = _userService.GetUsers();
        var user = users.FirstOrDefault(u => u.Id == userId);

        if (user == null)
        {
            return NotFound();
        }

        if (!BCrypt.Net.BCrypt.Verify(model.CurrentPassword, user.PasswordHash))
        {
            ModelState.AddModelError("CurrentPassword", "Current password is incorrect.");
            return View("Profile", model);
        }

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
        _userService.SaveUsers(users);

        TempData["SuccessMessage"] = "Password updated successfully!";
        return RedirectToAction("Profile");
    }

    [HttpPost]
    public IActionResult DeleteAccount()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        var users = _userService.GetUsers();
        var user = users.FirstOrDefault(u => u.Id == userId);

        if (user == null)
        {
            return NotFound();
        }

        _dataService.DeleteUserTasks(userId);

        users.Remove(user);
        _userService.SaveUsers(users);

        HttpContext.SignOutAsync("CookieAuth").Wait();

        TempData["SuccessMessage"] = "Your account and all associated tasks have been deleted.";
        return RedirectToAction("Login");
    }
}