using ContactsAPI.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ContactsAPI.Data
{
    public class DataContext : IdentityDbContext<ApplicationUser, ApplicationRole, string, IdentityUserClaim<string>, ApplicationUserRole, IdentityUserLogin<string>, IdentityRoleClaim<string>, IdentityUserToken<string>>
	{
        public DataContext(DbContextOptions<DataContext> options): base(options) { }

		protected override void OnModelCreating(ModelBuilder builder)
        {
			base.OnModelCreating(builder);

			builder.Entity<ApplicationUserRole>(userRole =>
			{
				userRole.HasKey(ur => new { ur.UserId, ur.RoleId });

				userRole.HasOne(ur => ur.Role)
					.WithMany(r => r.UserRoles)
					.HasForeignKey(ur => ur.RoleId)
					.IsRequired();

				userRole.HasOne(ur => ur.User)
					.WithMany(r => r.UserRoles)
					.HasForeignKey(ur => ur.UserId)
					.IsRequired();
			});
			//modelBuilder.Entity<Contact>().Property(p => p.DateUpdated).IsConcurrencyToken();
		}

		public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
		{
			var insertedEntries = ChangeTracker.Entries()
								   .Where(x => x.State == EntityState.Added)
								   .Select(x => x.Entity);
			foreach (var insertedEntry in insertedEntries)
			{
                //If the inserted object is an Auditable. 
                if (insertedEntry is Auditable auditableEntity)
                {
                    auditableEntity.DateCreated = DateTimeOffset.UtcNow;
                }
            }
			var modifiedEntries = ChangeTracker.Entries()
					   .Where(x => x.State == EntityState.Modified)
					   .Select(x => x.Entity);
			foreach (var modifiedEntry in modifiedEntries)
			{
                //If the inserted object is an Auditable. 
                if (modifiedEntry is Auditable auditableEntity)
                {
                    auditableEntity.DateUpdated = DateTimeOffset.UtcNow;
                }
            }
			return base.SaveChangesAsync(cancellationToken);
		}

		public DbSet<Contact> Contacts { get; set; }

		public DbSet<ContactType> ContactTypes { get; set; }

		public virtual DbSet<ApplicationUser> ApplicationUsers { get; set; }

		public virtual DbSet<ApplicationRole> ApplicationRoles { get; set; }

		public virtual DbSet<ApplicationUserRole> ApplicationUsersRoles { get; set; }
	}
}
