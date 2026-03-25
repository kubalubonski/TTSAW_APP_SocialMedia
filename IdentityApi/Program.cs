using System.Globalization;
using System.Text;
using IdentityApi.Data;
using IdentityApi.Profiles;
using IdentityApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

var connectionString = ResolveConnectionString(builder.Configuration, builder.Environment);

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
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]))
        };
    });

builder.Services.AddScoped<ITokenService, TokenService>();    

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers();

var app = builder.Build();

await MigrateDatabaseAsync<IdentityApiDbContext>(app.Services, app.Logger);

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

