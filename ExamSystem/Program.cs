using ExamSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddScoped<ExamContext, ExamContext>();
//builder.Services.AddScoped<IExamineeRepository,ExamineeRepository>();
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ExamContext>(options => {
    options.UseSqlServer(builder.Configuration.GetConnectionString("MyConnection"),
      o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));
});
//builder.Services.AddControllers().AddNewtonsoftJson();
builder.Services.ConfigureApplicationCookie(options =>
{
    //options.AccessDeniedPath = "/Account/Login"; // Update the path to your custom access denied page
});
builder.Services.AddIdentity<User, IdentityRole>().
    AddEntityFrameworkStores<ExamContext>().AddDefaultTokenProviders();
builder.Services.AddScoped<IExamineeRepository, ExamineeRepository>();
//services.AddAuthentication().AddGoogle(googleOptions =>
//{
//    googleOptions.ClientId = configuration["Authentication:Google:ClientId"];
//    googleOptions.ClientSecret = configuration["Authentication:Google:ClientSecret"];
//});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//app.UseAuthorization();
// app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();
//app.UseSwaggerUI(c =>)

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
    pattern: "{controller=Account}/{action=Login}");
        
});
app.Run();
