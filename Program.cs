var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseStatusCodePagesWithReExecute("/Home/NotFound/{0}");
    app.UseHsts();
}

if (app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseStatusCodePagesWithReExecute("/Home/NotFound/{0}");
    app.UseHsts();
    //app.UseDeveloperExceptionPage();
}

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
