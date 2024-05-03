using AccountManagermnet.Domain;
using AccountManagermnet.DTO;
using Microsoft.EntityFrameworkCore;

namespace AccountManagermnet.Data
{
    public class AccountDbContext : DbContext   
    {
        public AccountDbContext(DbContextOptions<AccountDbContext> options) : base(options) 
        {
            
        }
        public DbSet<AccountCategory> AccountCategorys { get; set; }
        public DbSet<Person> Persons { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<GoodsReceivedNoteDetail> GoodsReceivedNoteDetails { get; set; }
        public DbSet<GoodsReceivedNote> GoodsReceivedNotes { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Person>()
                .HasOne(p => p.GoodsReceivedNote)
                .WithOne(g => g.Person)
                .HasForeignKey<GoodsReceivedNote>(g => g.PersonID);

            modelBuilder.Entity<ProductCategory>()
                .HasMany(p => p.GoodsReceivedNoteDetails)
                .WithOne(g => g.ProductCategorys)
                .HasForeignKey(p => p.ProductId);

            modelBuilder.Entity<GoodsReceivedNoteDetail>()
                .HasOne (gd =>  gd.GoodsReceivedNote)
                .WithMany(g => g.GoodsReceivedNoteDetails)
                .HasForeignKey(g => g.GRN_Id);

            //Add Primary Key UserRole
            modelBuilder.Entity<UserRole>()
            .HasKey(ur => new { ur.UserId, ur.RoleId });

            //ManyToMany
            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId);

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId);

        }

    }
}
