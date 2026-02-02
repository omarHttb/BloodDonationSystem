using BloodDonationSystem.Data;
using BloodDonationSystem.Models;
using BloodDonationSystem.Services;
using BloodDonationSystem.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//services
builder.Services.AddScoped<IBloodRequestBloodTypeService,BloodRequestBloodTypeService>();
builder.Services.AddScoped<IBloodRequestService, BloodRequestService>();
builder.Services.AddScoped<IBloodTypeService,BloodTypeService>();
builder.Services.AddScoped<IDonationService,DonationService>();
builder.Services.AddScoped<IDonorService,DonorService>();
builder.Services.AddScoped<IRoleService,RoleService>();
builder.Services.AddScoped<IStatusService,StatusService>();
builder.Services.AddScoped<IUserRoleService,UserRoleService>();
builder.Services.AddScoped<IUserService,UserService>();


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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
