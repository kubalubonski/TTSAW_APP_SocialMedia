using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using SocialMediaApp.Models;


public class PostController : Controller
{
    private readonly IHttpClientFactory _clientFactory;

    public PostController(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    public async Task<IActionResult> Index(string filter = "all")
    {
        var client = _clientFactory.CreateClient("PostApi");

        var token = HttpContext.Session.GetString("JwtToken");
        if (string.IsNullOrEmpty(token))
        {
            return RedirectToAction("Login", "Account");
        }

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        List<PostDto> posts = new List<PostDto>();

        if (filter == "my")
        {
            var response = await client.GetAsync("/api/posts/my-posts");

            if (response.IsSuccessStatusCode)
            {
                posts = await response.Content.ReadFromJsonAsync<List<PostDto>>() ?? new List<PostDto>();
            }
        }
        else if (filter == "friends")
        {
            var response = await client.GetAsync("/api/posts/friends-posts");

            if (response.IsSuccessStatusCode)
            {
                posts = await response.Content.ReadFromJsonAsync<List<PostDto>>() ?? new List<PostDto>();
            }
        }
        else
        {
            var response = await client.GetAsync("/api/posts");

            if (response.IsSuccessStatusCode)
            {
                posts = await response.Content.ReadFromJsonAsync<List<PostDto>>() ?? new List<PostDto>();
            }
        }

        if (posts == null)
        {
            return View("Error");
        }

        return View(posts);
    }


    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CreatePost(PostCreateDto post)
    {
        var client = _clientFactory.CreateClient("PostApi");
        
        var token = HttpContext.Session.GetString("JwtToken");
        if (!string.IsNullOrEmpty(token))
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
        if (string.IsNullOrEmpty(token))
        {
            return RedirectToAction("Login", "Account");
        }

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        
        
        var response = await client.PostAsJsonAsync("/api/posts", post);

        if (!response.IsSuccessStatusCode)
            return View("Error");

        return RedirectToAction("Index");
    }
    public async Task<IActionResult> MyPosts()
    {
        var client = _clientFactory.CreateClient("PostApi");

        var token = HttpContext.Session.GetString("JwtToken");
        if (string.IsNullOrEmpty(token))
        {
            return RedirectToAction("Login", "Account");
        }

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.GetAsync("/api/posts/my-posts");

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            var emptyPosts = new List<PostDto>();
            return View(emptyPosts);
        }

        if (!response.IsSuccessStatusCode)
            return View("Error");

        var posts = await response.Content.ReadFromJsonAsync<List<PostDto>>() ?? new List<PostDto>();
        return View(posts);
    }

    public async Task<IActionResult> FriendsPosts()
    {
        var client = _clientFactory.CreateClient("PostApi");

        var token = HttpContext.Session.GetString("JwtToken");
        if (string.IsNullOrEmpty(token))
        {
            return RedirectToAction("Login", "Account");
        }

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.GetAsync("/api/posts/friends-posts");

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            var emptyPosts = new List<PostDto>();
            return View(emptyPosts);
        }

        if (!response.IsSuccessStatusCode)
            return View("Error");

        var posts = await response.Content.ReadFromJsonAsync<List<PostDto>>() ?? new List<PostDto>();

        return View(posts);
    }

    [HttpPost]
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

        return RedirectToAction("MyPosts");
    }
}
