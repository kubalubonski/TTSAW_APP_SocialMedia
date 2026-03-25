using System.Globalization;
using System.Text;
using IdentityApi.Configuration;
using IdentityApi.Data;
using IdentityApi.Profiles;
using IdentityApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

var connectionString = StartupSupport.ResolveConnectionString(builder.Configuration, builder.Environment);

builder.Services.AddDbContext<IdentityApiDbContext>(options =>
    options.UseSqlServer(connectionString, sqlOptions => sqlOptions.EnableRetryOnFailure()));
        
builder.Services.AddAutoMapper(typeof(UserProfile));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = false,
            ValidateIssuerSigningKey = true,
            ValidIssuer = StartupSupport.GetRequiredSetting(builder.Configuration, "Jwt:Issuer"),
            ValidAudience = StartupSupport.GetRequiredSetting(builder.Configuration, "Jwt:Audience"),
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(StartupSupport.GetRequiredSetting(builder.Configuration, "Jwt:SecretKey")))
        };
    });

builder.Services.AddScoped<ITokenService, TokenService>();    

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers();

var app = builder.Build();

await StartupSupport.MigrateDatabaseAsync<IdentityApiDbContext>(app.Services, app.Logger);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseAuthentication();
app.UseAuthorization(); 
 
app.UseHttpsRedirection();
app.MapControllers();


app.Run();

