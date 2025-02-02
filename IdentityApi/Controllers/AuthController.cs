using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using IdentityApi.Data;
using IdentityApi.Models;
using IdentityApi.Models.Dtos;
using IdentityApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace IdentityApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IdentityApiDbContext _context;
    private readonly IMapper _mapper;
    private readonly ITokenService _tokenService;
    public AuthController(IdentityApiDbContext context, IMapper mapper, IConfiguration configuration, ITokenService tokenService)
    {
        _context = context;
        _mapper = mapper;
        _tokenService = tokenService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserDto userDto)
    {
        if( await _context.Users.AnyAsync(u => u.Username == userDto.Username))
            return BadRequest("Username is already used");

        var user = _mapper.Map<User>(userDto);

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var token = _tokenService.GenerateJwtToken(user);

        return Ok(new { Token = token });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserDto request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return Unauthorized("Invalid credentials");

        var token = _tokenService.GenerateJwtToken(user);
        return Ok(new { Token = token });
    }
}