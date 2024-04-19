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
            //modelBuilder.Entity<Role>()
            //    .HasOne(u => u.User)
            //    .WithMany(r => r.Roles)
            //    .HasForeignKey(r => r.UserId);

        }

    }
}
