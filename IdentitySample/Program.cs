using IdentitySample.Context;
using IdentitySample.Security.Default;
using IdentitySample.Services;
using IdentitySample.Utitlities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Persian_Identity_Translate;

namespace IdentitySample
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.
			builder.Services.AddControllersWithViews();



			builder.Services.AddDbContext<ApplicationDbContext>(options =>
			{
				options.UseSqlServer(builder.Configuration.GetConnectionString("default"));
			});


			builder.Services.AddMemoryCache();
			builder.Services.AddScoped<IUtilities, Utitlities.Utitlities>();
			builder.Services.AddScoped<IMessageSender, MessageSender>();

			builder.Services.AddIdentity<IdentityUser, IdentityRole>()
				.AddEntityFrameworkStores<ApplicationDbContext>()
				.AddDefaultTokenProviders()
				.AddErrorDescriber<PersianIdentityErrorDescriber>();


			builder.Services.Configure<IdentityOptions>(options =>
			{

				options.Password.RequireNonAlphanumeric = false;
				options.Password.RequiredUniqueChars = 0;
				options.Password.RequireUppercase = false;
				options.Password.RequireLowercase = false;
				options.Password.RequiredLength = 8;

				options.Lockout.MaxFailedAccessAttempts = 5;
				options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);

				options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@";
			});

			builder.Services.AddAuthorization(options =>
			{
				options.AddPolicy("EmployeeListPolicy",
					policy => policy.RequireClaim(ClaimTypeStore.ViewEmployeeList, true.ToString()));

				options.AddPolicy("ClaimRequirement", policy =>
				policy.Requirements
				.Add(new ClaimRequirement(ClaimTypeStore.ViewEmployeeList, true.ToString())));
			});

			builder.Services.AddSingleton<IAuthorizationHandler, ClaimHandler>();

			builder.Services.ConfigureApplicationCookie(options =>
			{
				options.AccessDeniedPath = "/Home/AccessDenied";
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

			//app.UseRoleClaimMiddlewareForPath(
			//path: "/Employee/Index",
			//requiredRole: "Admin",
			//requiredClaimType: ClaimTypeStore.ViewEmployeeList,
			//requiredClaimValue: true.ToString()
			//);


			app.MapControllerRoute(
				name: "default",
				pattern: "{controller=Home}/{action=Index}/{id?}");


			app.Run();
		}
	}
}
