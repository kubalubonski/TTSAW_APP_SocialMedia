using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PostApi.Configuration;
using PostApi.Data;
using PostApi.Dtos;
using PostApi.Models;
using PostApi.Repositories;

var builder = WebApplication.CreateBuilder(args);

var activeConnection = StartupSupport.ResolveConnectionString(builder.Configuration, builder.Environment);
builder.Services.AddDbContext<PostApiDbContext>(options =>
    options.UseSqlServer(activeConnection, sqlOptions => sqlOptions.EnableRetryOnFailure()));
builder.Services.AddDbContext<FriendshipDbContext>(options =>
    options.UseSqlServer(activeConnection, sqlOptions => sqlOptions.EnableRetryOnFailure()));


builder.Services.AddScoped<IPostRepository, PostRepository>();
builder.Services.AddControllers();

builder.Services.AddAutoMapper(typeof(PostMapperProfile));

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

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Wprowadź token JWT w formacie 'Bearer {twoj-token}'"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

var app = builder.Build();

await StartupSupport.MigrateDatabaseAsync<PostApiDbContext>(app.Services, app.Logger);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();


app.Run();

