using IdentitySample.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IdentitySample.Context
{
	public class ApplicationDbContext : IdentityDbContext
	{
		public ApplicationDbContext(DbContextOptions options) : base(options) { }

		public DbSet<Employee> Employees { get; set; }

		public DbSet<SiteSetting> SiteSettings { get; set; }


	}
}
