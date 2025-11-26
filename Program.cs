using MediScope.Models;
using MediScope.Data;
using MediScope.Identity;
using MediScope.Repositories;
using MediScope.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Repo DI
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<AppointmentRepository>();
builder.Services.AddScoped<AdminRepository>();
builder.Services.AddScoped<DoctorRepository>();
builder.Services.AddScoped<PatientRepository>();
builder.Services.AddScoped<DepartmentRepository>();
builder.Services.AddScoped<ResourceRepository>();

// Service DI
builder.Services.AddScoped<AuthenticationService>();
builder.Services.AddScoped<AppointmentService>();
builder.Services.AddScoped<AnalyticsService>();
builder.Services.AddScoped<FeedbackService>();
builder.Services.AddScoped<ValidationService>();

// Sessions
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(5);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


// Connect EF
builder.Services.AddDbContext<MediScopeContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => { options.Lockout.AllowedForNewUsers = false; })
    .AddEntityFrameworkStores<MediScopeContext>()
    .AddDefaultTokenProviders();


builder.Services.ConfigureApplicationCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseSession();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
    
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<MediScopeContext>();
    
    // apply migrations
    context.Database.Migrate();
    
    // seed data
    // await SeedData.Initialize(services, context);
}

app.Run();