using Bookstore.Data;
using Bookstore.Repository;
using Bookstore.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Bookstore.Utility;
using Microsoft.Extensions.DependencyInjection;
using Stripe;

var builder = WebApplication.CreateBuilder(args);
var ConnectionString = builder.Configuration.GetConnectionString("dbConnection");
var Stripe = builder.Configuration.GetSection("Stripe");
var StripeSecret = builder.Configuration.GetSection("Stripe:SecretKey").Get<string>();

builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<Context>(data => data.UseSqlServer(ConnectionString));
builder.Services.Configure<StripeSetting>(Stripe);

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddDefaultTokenProviders()
    .AddEntityFrameworkStores<Context>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IEmailSender, EmailSender>();
builder.Services.AddRazorPages().AddRazorRuntimeCompilation();

builder.Services.ConfigureApplicationCookie(data => {
    data.LoginPath = $"/Identity/Account/Login";
    data.LogoutPath = $"/Identity/Account/Logout";
    data.AccessDeniedPath = $"/Identity/Account/AccessDenied";
});

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(data => {
    data.IdleTimeout = TimeSpan.FromMinutes(100);
    data.Cookie.HttpOnly = true;
    data.Cookie.IsEssential = true;
});

var app = builder.Build();

if (!app.Environment.IsDevelopment()) {
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

StripeConfiguration.ApiKey = StripeSecret;

app.UseAuthentication();
app.UseAuthorization();
app.UseSession();
app.MapRazorPages();
app.MapControllerRoute(
	name: "default",
	pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");

app.Run();
