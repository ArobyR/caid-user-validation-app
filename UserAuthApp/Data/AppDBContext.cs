using Microsoft.EntityFrameworkCore;
using UserAuthApp.Models;

namespace UserAuthApp.Data
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options)
        {

        }

        // definition of tables
        public DbSet<User> Users { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>(tb =>
            {
                tb.HasKey(col => col.IdUser);
                tb.Property(col => col.IdUser)
                .UseIdentityColumn()
                .ValueGeneratedOnAdd();

                tb.Property(col => col.FullName).HasMaxLength(50);
                tb.Property(col => col.Email).HasMaxLength(50);
                tb.Property(col => col.Password).HasMaxLength(100);
            });

            modelBuilder.Entity<User>().ToTable("Usuario");
        }
    }
}