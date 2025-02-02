using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using SocialMediaApp.Models;

public class PeopleController : Controller
{
    private readonly IHttpClientFactory _clientFactory;

    public PeopleController(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    public async Task<IActionResult> Index()
    {
        var client = _clientFactory.CreateClient("PeopleApi");

        var token = HttpContext.Session.GetString("JwtToken");
        if (string.IsNullOrEmpty(token))
        {
            return RedirectToAction("Login", "Account");
        }

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.GetAsync("/api/people/available");
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            var emptyUsers = new List<UserDto>();
            return View(emptyUsers);
        }

        if (!response.IsSuccessStatusCode)
        {
            ModelState.AddModelError("", "Failed to load users.");
            return View("Error");
        }

        var users = await response.Content.ReadFromJsonAsync<List<UserDto>>() ?? new List<UserDto>();
        return View(users);
    }

    [HttpPost]
    public async Task<IActionResult> AddFriend(int friendId)
    {
        var client = _clientFactory.CreateClient("PeopleApi");

        var token = HttpContext.Session.GetString("JwtToken");
        if (string.IsNullOrEmpty(token))
        {
            return RedirectToAction("Login", "Account");
        }

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.PostAsJsonAsync("/api/people/friends", new { FriendId = friendId });

        if (!response.IsSuccessStatusCode)
        {
            ModelState.AddModelError("", "Failed to add friend.");
            return View("Error");
        }

        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Friends()
    {
        var client = _clientFactory.CreateClient("PeopleApi");

        var token = HttpContext.Session.GetString("JwtToken");
        if (string.IsNullOrEmpty(token))
        {
            return RedirectToAction("Login", "Account");
        }

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.GetAsync("/api/people/friends");
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            var emptyFriends = new List<UserDto>();
            return View(emptyFriends);
        }

        if (!response.IsSuccessStatusCode)
        {
            ModelState.AddModelError("", "Failed to load friends.");
            return View("Error");
        }

        var friends = await response.Content.ReadFromJsonAsync<List<UserDto>>();
        return View(friends);
    }

    [HttpPost]
    public async Task<IActionResult> RemoveFriend(int friendId)
    {
        var client = _clientFactory.CreateClient("PeopleApi");

        var token = HttpContext.Session.GetString("JwtToken");
        if (string.IsNullOrEmpty(token))
        {
            return RedirectToAction("Login", "Account");
        }

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.DeleteAsync($"/api/people/friend/{friendId}");

        if (!response.IsSuccessStatusCode)
        {
            ModelState.AddModelError("", "Failed to remove friend.");
            return View("Error");
        }

        return RedirectToAction("Friends");
    }

    public async Task<IActionResult> DeletePost(int id)
    {
        var client = _clientFactory.CreateClient("PostApi");

        var token = HttpContext.Session.GetString("JwtToken");
        if (string.IsNullOrEmpty(token))
        {
            return RedirectToAction("Login", "Account");
        }

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.DeleteAsync($"/api/posts/{id}");

        if (!response.IsSuccessStatusCode)
        {
            return View("Error");
        }

        return RedirectToAction("Index", new { filter = "my" });
    }
}
