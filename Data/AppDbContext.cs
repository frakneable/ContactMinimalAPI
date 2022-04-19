using ContactMinimalAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ContactMinimalAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Person> People { get; set; }
        public DbSet<Contact> Contacts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Contact>().HasKey(c => c.Id);

            modelBuilder.Entity<Contact>().Property(c => c.Type)
                .IsRequired();

            modelBuilder.Entity<Contact>().Property(c => c.Value)
                .IsRequired()
                .HasColumnType("varchar(200)");

            modelBuilder.Entity<Contact>()
                .HasOne(c => c.Person)
                .WithMany(p => p.Contacts)
                .HasForeignKey("PersonId")
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Contact>().ToTable("Contacts");


            modelBuilder.Entity<Person>().HasKey(c => c.Id);

            modelBuilder.Entity<Person>().Property(c => c.Name)
                .IsRequired()
                .HasColumnType("varchar(200)");

            modelBuilder.Entity<Person>().ToTable("People");
        }
    }
}
