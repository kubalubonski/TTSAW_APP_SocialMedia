using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SocialMediaApp.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddHttpContextAccessor();

builder.Services.AddHttpClient("IdentityApi", client =>
        {
            var identityApiUrl = builder.Configuration["IdentityApiUrl"]
                ?? (builder.Environment.IsDevelopment() ? "http://localhost:5057/api" : "http://identityapi:8080/api");
            client.BaseAddress = new Uri(identityApiUrl);
        }).AddHttpMessageHandler<AuthenticationDelegatingHandler>();

        builder.Services.AddHttpClient("PostApi", client =>
        {
            var postApiUrl = builder.Configuration["PostApiUrl"]
                ?? (builder.Environment.IsDevelopment() ? "http://localhost:5035/api" : "http://postapi:8080/api");
            client.BaseAddress = new Uri(postApiUrl);
        }).AddHttpMessageHandler<AuthenticationDelegatingHandler>();

        builder.Services.AddHttpClient("PeopleApi", client =>
        {
            var peopleApiUrl = builder.Configuration["PeopleApiUrl"]
                ?? (builder.Environment.IsDevelopment() ? "http://localhost:5096/api" : "http://peopleapi:8080/api");
            client.BaseAddress = new Uri(peopleApiUrl);
        }).AddHttpMessageHandler<AuthenticationDelegatingHandler>();

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

builder.Services.AddAuthorization();

builder.Services.AddTransient<AuthenticationDelegatingHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
