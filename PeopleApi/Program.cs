using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PeopleApi.Data;
using PeopleApi.Mapping;
using PeopleApi.Middleware;
using PeopleApi.Repositories;

var builder = WebApplication.CreateBuilder(args);



// Add services to the container.
builder.Services.AddScoped<IFriendshipRepository, FriendshipRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddAutoMapper(typeof(PeopleProfile));

var activeConnection = ResolveConnectionString(builder.Configuration, builder.Environment);
builder.Services.AddDbContext<PeopleApiDbContext>(options =>
    options.UseSqlServer(activeConnection, sqlOptions => sqlOptions.EnableRetryOnFailure()));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = false,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]))
        };
    });
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

builder.Services.AddControllers();

var app = builder.Build();

await MigrateDatabaseAsync<PeopleApiDbContext>(app.Services, app.Logger);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

}
app.UseMiddleware<TokenValidationMiddleware>();
app.UseAuthentication(); // Dodanie middleware do autoryzacji
app.UseAuthorization();
app.MapControllers();
app.UseHttpsRedirection();


app.Run();

static string ResolveConnectionString(IConfiguration configuration, IHostEnvironment environment)
{
    var connectionName = environment.IsDevelopment()
        ? "DefaultConnection"
        : "DockerConnection";

    return configuration.GetConnectionString(connectionName)
        ?? throw new InvalidOperationException($"Missing connection string '{connectionName}'.");
}

static async Task MigrateDatabaseAsync<TContext>(IServiceProvider services, ILogger logger)
    where TContext : DbContext
{
    const int maxAttempts = 10;

    for (var attempt = 1; attempt <= maxAttempts; attempt++)
    {
        try
        {
            using var scope = services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<TContext>();
            await dbContext.Database.MigrateAsync();
            logger.LogInformation("Applied migrations for {DbContext}.", typeof(TContext).Name);
            return;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Migration attempt {Attempt}/{MaxAttempts} failed for {DbContext}.", attempt, maxAttempts, typeof(TContext).Name);

            if (attempt == maxAttempts)
            {
                throw;
            }

            await Task.Delay(TimeSpan.FromSeconds(5));
        }
    }
}