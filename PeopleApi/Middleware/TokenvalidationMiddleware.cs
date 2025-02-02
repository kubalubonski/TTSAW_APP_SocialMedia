using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace PeopleApi.Middleware
{
    public class TokenValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;

        public TokenValidationMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            var token = httpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (string.IsNullOrEmpty(token))
            {
                Console.WriteLine("Brak tokenu");
            }
            else
            {
                Console.WriteLine($"Token: {token}");

                try
                {
                    // Sprawdzamy, czy token jest poprawny
                    var key = Encoding.ASCII.GetBytes(_configuration["Jwt:SecretKey"]);
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var validationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = false,  // Wyłączamy sprawdzanie daty ważności, jeśli to chcesz
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidIssuer = _configuration["Jwt:Issuer"],
                        ValidAudience = _configuration["Jwt:Audience"]
                    };

                    var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);

                    // Jeśli token jest prawidłowy, ustawiamy użytkownika w HttpContext
                    if (principal != null)
                    {
                        httpContext.User = principal;
                        Console.WriteLine("Token jest prawidłowy");
                    }
                    else
                    {
                        Console.WriteLine("Token jest nieprawidłowy");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Błąd podczas walidacji tokenu: {ex.Message}");
                }
            }

            // Przechodzimy dalej w pipeline
            await _next(httpContext);
        }
    }
}
