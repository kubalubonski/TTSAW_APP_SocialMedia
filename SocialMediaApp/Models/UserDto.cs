namespace SocialMediaApp.Models;

public class UserDto
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string PasswordHash { get; set; }
}