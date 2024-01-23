using ExamSystem.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddScoped<ExamContext, ExamContext>();
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ExamContext>(options => {
    options.UseSqlServer(builder.Configuration.GetConnectionString("MyConnection"));
});
builder.Services.AddScoped<IExamineeRepository, ExamineeRepository>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();
app.MapRazorPages();

//builder.Services.AddScoped<ExamContext, ExamContext>();
//builder.Services.AddDbContext<ExamContext>(options => {
//    options.UseSqlServer(builder.Configuration.GetConnectionString("MyConnection"), builder =>
//    {
//        builder.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
//    });

//});

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
    name: "default",
    pattern: "{controller=Examinee}/{action=User_Home}");
        
});
app.Run();
