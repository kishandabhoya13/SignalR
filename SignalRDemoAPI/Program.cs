using SignalRDemoAPI.Hubs;
using StudentManagement_API.Services;
using StudentManagment_API;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDistributedMemoryCache();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(2);
    options.Cookie.HttpOnly = true; 
    options.Cookie.IsEssential = true;
});
builder.Services.AddDataProtection();
builder.Services.AddDataLayerServices();
builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IJwtServices, JwtServices>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
            builder =>
            {
                builder.WithOrigins("https://localhost:7137") // Specify your MVC app's origin
                       .AllowAnyMethod()
                       .AllowAnyHeader()
                       .AllowCredentials(); // Allow credentials
            });
});
builder.Services.AddSignalR().AddHubOptions<NotificationHub>(options =>
{
    options.EnableDetailedErrors = true;
}); ;
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseRouting();
app.UseSession();
app.UseHttpsRedirection();
app.UseAuthorization();
app.UseCors("AllowSpecificOrigin");

app.MapControllers();
app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<NotificationHub>("/notificationHub");
});
app.Run();
