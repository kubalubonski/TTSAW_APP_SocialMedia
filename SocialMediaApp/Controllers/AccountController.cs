using Microsoft.AspNetCore.Mvc;
using SocialMediaApp.Models;

public class AccountController : Controller
{
    private readonly IHttpClientFactory _clientFactory;

    public AccountController(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    public IActionResult Login() => View();

    [HttpPost]
    public async Task<IActionResult> Login(string username, string password)
    {
        var client = _clientFactory.CreateClient("IdentityApi");
        var response = await client.PostAsJsonAsync("api/Auth/login", new { Username = username, Password = password });

        if (!response.IsSuccessStatusCode)
        {
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View();
        }

        var result = await response.Content.ReadFromJsonAsync<TokenResponse>();
        HttpContext.Session.SetString("JwtToken", result.Token);

        return RedirectToAction("Index", "Home");
    }

    public IActionResult Register() => View();

    [HttpPost]
    public async Task<IActionResult> Register(string username, string password)
    {
        var client = _clientFactory.CreateClient("IdentityApi");
        var response = await client.PostAsJsonAsync("api/Auth/register", new { Username = username, Password = password });
        if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
        {
            ModelState.AddModelError("", "A user with this username already exists.");
            return View();
        }
        if (!response.IsSuccessStatusCode)
        {
            ModelState.AddModelError(string.Empty, "Error registering account.");
            return View();
        }

        return RedirectToAction("Login");
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Remove("JwtToken");
        return RedirectToAction("Login");
    }
}
