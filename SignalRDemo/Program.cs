
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SignalRDemo.Middleware;
using StudentManagment;
using System.Net.WebSockets;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(2); // Set session timeout
});
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddDataLayerServices();
builder.Services.AddHttpContextAccessor();
//builder.Services.AddSignalR().AddHubOptions<CallHub>(options =>
//{
//    options.EnableDetailedErrors = true;
//}); ;

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
else
{
    app.UseDeveloperExceptionPage();
    // This will handle exceptions and redirect to the specified error page.
}
app.UseSession();
app.UseWebSockets();
app.UseMiddleware<WebSocketMiddleware>();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Login}/{id?}");

//app.UseEndpoints(endpoints =>
//{
//    endpoints.MapHub<CallHub>("/callHub");
//});]



app.Run();
