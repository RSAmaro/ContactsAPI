using Microsoft.EntityFrameworkCore;

namespace ContactsAPI.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options): base(options) { }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
			//modelBuilder.Entity<Contact>().Property(p => p.DateUpdated).IsConcurrencyToken();
        }

		public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
		{
			var insertedEntries = ChangeTracker.Entries()
								   .Where(x => x.State == EntityState.Added)
								   .Select(x => x.Entity);
			foreach (var insertedEntry in insertedEntries)
			{
				var auditableEntity = insertedEntry as Auditable;
				//If the inserted object is an Auditable. 
				if (auditableEntity != null)
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
				var auditableEntity = modifiedEntry as Auditable;
				if (auditableEntity != null)
				{
					auditableEntity.DateUpdated = DateTimeOffset.UtcNow;
				}
			}
			return base.SaveChangesAsync(cancellationToken);
		}

		public DbSet<Contact> Contacts { get; set; }

		public DbSet<ContactType> ContactTypes { get; set; }
	}
}
