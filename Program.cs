using EliteSportsAcademy.Data;
using EliteSportsAcademy.Models.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(Options =>
{
    Options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await DataSeeder.SeedSAdministrator(services);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseStatusCodePagesWithReExecute("/Home/NotFound/{0}");
    app.UseHsts();
}

app.UseExceptionHandler("/Home/Error");
// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
app.UseStatusCodePagesWithReExecute("/Home/NotFound/{0}");
app.UseHsts();
//app.UseDeveloperExceptionPage();


//if (!app.Environment.IsDevelopment())
//{
//    // Handle all unhandled exceptions here
//    app.UseExceptionHandler("/Home/Error");

//    // Handle HTTP errors like 404, 403, etc.
//    app.UseStatusCodePagesWithReExecute("/Home/NotFound");
//}
//else
//{
//    app.UseDeveloperExceptionPage();
//}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
