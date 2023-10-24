using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TeamManageSystem.Data;
using TeamManageSystem.Hubs;
using TeamManageSystem;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<TeamManageContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("TeamManageContext") ?? throw new InvalidOperationException("Connection string 'TeamManageContext' not found.")));
 
// Add services for pdf.


// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(option =>
    {
        option.ExpireTimeSpan = TimeSpan.FromMinutes(60*3);
        option.LoginPath = "/Account/Login";
        option.AccessDeniedPath = "/Account/Login";
    });
builder.Services.AddSession(option =>
{
    option.IdleTimeout = TimeSpan.FromMinutes(5);
    option.Cookie.HttpOnly = true;
    option.Cookie.IsEssential = true;
});
builder.Services.AddSignalR();
//builder.Services.AddIdentity<IdentityUser, IdentityRole>()
//            .AddEntityFrameworkStores<TeamManageContext>()
//            .AddDefaultTokenProviders();
builder.Services.AddScoped<DatabaseExistenceChecker>();
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
    pattern: "{controller=Account}/{action=FrontPage}/{id?}");
app.MapHub<ChatHub>("/chatHub");
using var scope = app.Services.CreateScope();
var dbExistenceChecker = scope.ServiceProvider.GetRequiredService<DatabaseExistenceChecker>();

if (!dbExistenceChecker.DoesDatabaseExist())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<TeamManageContext>();
    dbContext.Database.Migrate(); // Apply migrations if the database is empty.
}

using (var scopes = builder.Services.BuildServiceProvider().CreateScope())
{
    var context = scopes.ServiceProvider.GetRequiredService<TeamManageContext>();
    DbSeeder.SeedData(context);
}
app.Run();
