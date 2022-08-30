using API.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class BuildingContext : IdentityDbContext<Account, Role, int>
    {
        public BuildingContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<Bill> Bills { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Favorite> Favorites { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Photo> Photos { get; set; }
        public DbSet<RentRequest> RentRequests { get; set; }
        public DbSet<UnRentRequest> UnRentRequests { get; set; }
        public DbSet<ReportBuilding> ReportBuildings { get; set; }
        public DbSet<ReportPhoto> ReportPhotos { get; set; }
        public DbSet<TypeItem> TypeItems { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                var tableName = entityType.GetTableName();
                if (tableName.StartsWith("AspNet"))
                {
                    entityType.SetTableName(tableName.Substring(6));
                }
            }

            builder.Entity<Member>()
                .HasOne(a => a.PlaceOfOrigin)
                .WithOne()
                .HasForeignKey<UserAddress>(a => a.Id)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Item>()
                .HasOne(a => a.Photos)
                .WithOne()
                .HasForeignKey<Photo>(a => a.Id)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<TypeItem>()
                .HasIndex(i => i.Name).IsUnique();

            builder.Entity<Member>()
                .HasIndex(i => i.CCCD).IsUnique();

            builder.Entity<Vehicle>()
                .HasIndex(i => i.LicensePlates).IsUnique();

            builder.Entity<Account>()
                .HasMany(m => m.Members)
                .WithOne(a => a.Account)
                .HasForeignKey(a => a.AccountId)
                .OnDelete(DeleteBehavior.Cascade);


            builder.Entity<Role>()
                .HasData(
                    new Role { Id = 1, Name = "Member", NormalizedName = "MEMBER" },
                    new Role { Id = 2, Name = "Admin", NormalizedName = "ADMIN" }
                );

            builder.Entity<TypeItem>()
                .HasData(
                new TypeItem { Id = 1, Name = "Studio" },
                new TypeItem { Id = 2, Name = "2PN 1VS" },
                new TypeItem { Id = 3, Name = "2PN 2VS" },
                new TypeItem { Id = 4, Name = "3PN 2VS" });
        }
    }
}
