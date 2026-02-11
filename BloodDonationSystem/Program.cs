using BloodDonationSystem.Data;
using BloodDonationSystem.Models;
using BloodDonationSystem.Services;
using BloodDonationSystem.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//services
builder.Services.AddScoped<IBloodRequestService, BloodRequestService>();
builder.Services.AddScoped<IBloodCompatibilityService, BloodCompatibilityService>();
builder.Services.AddScoped<IBloodTypeService,BloodTypeService>();
builder.Services.AddScoped<IDonationService,DonationService>();
builder.Services.AddScoped<IDonorService,DonorService>();
builder.Services.AddScoped<IRoleService,RoleService>();
builder.Services.AddScoped<IStatusService,StatusService>();
builder.Services.AddScoped<IUserRoleService,UserRoleService>();
builder.Services.AddScoped<IUserService,UserService>();
builder.Services.AddScoped<IBloodBankService,BloodBankService>();
builder.Services.AddScoped<IBloodBankHistoryService,BloodBankHistoryService>();
builder.Services.AddSingleton<IOtpService,OtpService>();
builder.Services.AddScoped<TokenService>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"]))
    };
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            // If the request has our cookie, use it as the token
            if (context.Request.Cookies.ContainsKey("X-Access-Token"))
            {
                context.Token = context.Request.Cookies["X-Access-Token"];
            }
            return Task.CompletedTask;
        },

        OnChallenge = context =>
        {
            // Stop the default behavior (which is just returning 401)
            context.HandleResponse();

            // Redirect the user to your login page
            //context.Response.Redirect("/Login/Index");

            return Task.CompletedTask;
        },
        OnForbidden = context =>
        {
            context.Response.Redirect("/Account/AccessDenied");
            return Task.CompletedTask;
        }
    };

});

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

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
