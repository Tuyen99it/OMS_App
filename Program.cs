using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OMS_Webapp.Data;
using OMS_Webapp.Models;

var builder = WebApplication.CreateBuilder(args);


// Add DbContext
builder.Services.AddDbContext<OMSDBContext>(options =>
{
    // Fretch connectstring from appsettings.json file
    string connectionString = builder.Configuration.GetConnectionString("OMSContext");
    // Connect to Mssql server
    options.UseSqlServer(connectionString);
});



//Add Identity Services to container
builder.Services.AddDefaultIdentity<AppUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
}).AddRoles<IdentityRole>().AddEntityFrameworkStores<OMSDBContext>().AddDefaultTokenProviders();;
//configure IdentityOption
builder.Services.Configure<IdentityOptions>(options =>
{
    // Password setting
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 8;
    options.Password.RequiredUniqueChars = 1;

    //Lock settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;


    //User settings
    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = false;

});
builder.Services.AddAuthentication().AddFacebook(facebookOptions=>{
    facebookOptions.AppId=builder.Configuration["Authentication:Facebook:AppId"];
    facebookOptions.AppSecret=builder.Configuration["Authentication:Facebook:AppSecret"];
    facebookOptions.CallbackPath="/dang-nhap-tu-facebook";
    facebookOptions.AccessDeniedPath="/Account/Login";
});

//Configure cookie for Identity 
builder.Services.ConfigureApplicationCookie(options => {
//cookie setting
options.Cookie.HttpOnly = true;                          // Ensure the cookie can not be accessed from client-side cripts.
options.ExpireTimeSpan = TimeSpan.FromMinutes(30);       // Setting expire time of cookie is 30 minutes
options.LoginPath = "/Identity/Account/AccessDenied";    // Sets the path to the login page when access is denied
options.SlidingExpiration = true;                        // Reset cookie expire time when each request of client. Ensuring that the user remains authenticated as long as they continue to interact with the application within the specified time span ( 30 minutes in this case )

});



// Add services to the container.
builder.Services.AddControllersWithViews();
// Add Razor page
//builder.Services.AddRazorPages();
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
//app.MapRazorPages();
app.UseAuthorization();
app.MapControllerRoute(
    name: "MyArea",
    pattern: "{area:exists}/{controller=Home}/{Action=Index}/{id?}"
    );
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
