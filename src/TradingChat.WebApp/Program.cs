using TradingChat.Infrastructure;
using TradingChat.WebApp.Configurations;
using TradingChat.WebApp.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.AddSerilog();

builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();

builder.Services.AddAuth().AddDbContext(builder.Configuration);

builder.Services.AddSignalR();
builder.Services.InjectServices();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapHub<ChatHub>("/Chat/Hub");

app.MapRazorPages();

app.Run();
