
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PeopleApi.Models.Dtos;
using PeopleApi.Repositories;

namespace PeopleApi.Controllers;

[ApiController]
[Route("api/people")]
[Authorize]
public class PeopleController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly IFriendshipRepository _friendshipRepository;
    private readonly IMapper _mapper;

    public PeopleController(IUserRepository userRepository, IFriendshipRepository friendshipRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _friendshipRepository = friendshipRepository;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _userRepository.GetAllUsersAsync();
        var usersDto = _mapper.Map<List<UserDto>>(users);
        return Ok(usersDto);
    }

    [HttpGet("available")] // Ci, którze nie są jeszcze przyjaciółmi
    public async Task<IActionResult> GetAvailableUsers()
    {
        var userId = int.Parse(User.FindFirst("userId")?.Value); 
        
        var friends = await _friendshipRepository.GetFriendsByUserIdAsync(userId);
        var friendIds = friends.Select(f => f.Id).ToList();

        var users = await _userRepository.GetUsersNotInListAsync(friendIds);
        var usersDto = _mapper.Map<List<UserDto>>(users);
        
        return Ok(usersDto);
    }

    [HttpGet("friends")]
    public async Task<IActionResult> GetFriends()
    {
        var userId = int.Parse(User.FindFirst("userId")?.Value);
        var friends = await _friendshipRepository.GetFriendsByUserIdAsync(userId);

        if((friends is null) || !friends.Any())
            return NotFound("No friends found");

        var friendsDto = _mapper.Map<List<UserDto>>(friends);
        return Ok(friends);
    }

    [HttpPost("friends")]
    public async Task<IActionResult> AddFriend([FromBody] FriendRequestDto request)
    {
        var userId = int.Parse(User.FindFirst("userId")?.Value);
        var result = await _friendshipRepository.AddFriendAsync(userId, request.FriendId);

        if(!result)
            return BadRequest("Unable to add friend");

        return Ok("Friend added successfully");
    }

    [HttpDelete("friend/{id}")]
    public async Task<IActionResult> RemoveFriend(int id)
    {
        var userId = int.Parse(User.FindFirst("userId")?.Value);
        var result = await _friendshipRepository.RemoveFriendAsync(userId, id);

        if(!result)
            return BadRequest("Unable to remove friend");

        return Ok("Friend removed successfully");
    }
}