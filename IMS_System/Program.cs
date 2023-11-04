using IMS_System.Models.Entities;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using NToastNotify;
using System.Configuration;
using System.Text.Encodings.Web;
using System.Text.Unicode;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
builder.Services.AddDbContext<ImsSystemContext>(options => options.UseSqlServer(config.GetConnectionString("dbIMSsystem")));
builder.Services.AddSingleton<HtmlEncoder>(HtmlEncoder.Create(allowedRanges: new[] { UnicodeRanges.All }));
builder.Services.AddControllersWithViews();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(p =>
                {
                    p.LoginPath = "/Login.html";
                    p.AccessDeniedPath = "/";
                });
builder.Services.AddAuthorization();
builder.Services.AddSession();
builder.Services.AddMvc().AddNToastNotifyToastr(new ToastrOptions()
{
	ProgressBar = false,
	PositionClass = ToastPositions.TopCenter
});

var app = builder.Build();



// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();
app.UseSession();
app.UseNToastNotify();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
