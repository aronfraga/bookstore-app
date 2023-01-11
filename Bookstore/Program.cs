using Bookstore.Data;
using Bookstore.Repository;
using Bookstore.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Bookstore.Utility;

var builder = WebApplication.CreateBuilder(args);
var ConnectionString = builder.Configuration.GetConnectionString("dbConnection");

builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<Context>(data => data.UseSqlServer(ConnectionString));

builder.Services.AddIdentity<IdentityUser, IdentityRole>(/*data => data.SignIn.RequireConfirmedAccount = true*/)
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

var app = builder.Build();

if (!app.Environment.IsDevelopment()) {
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllerRoute(
	name: "default",
	pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");

app.Run();
