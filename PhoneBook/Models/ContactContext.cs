using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Phone_Book.Models;

namespace Phone_Book;

public class ContactContext : DbContext
{
    public DbSet<Contact> Contacts { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Contact>().HasData(new Contact[] {
            new Contact { Id = 1, Name = "Ava Thompson", Email = "ava.thompson@example.com", PhoneNumber = "9876543210" },
            new Contact { Id = 2, Name = "Liam Patel", Email = "liam.patel@example.com", PhoneNumber = "9823456789" },
            new Contact { Id = 3, Name = "Emma Reddy", Email = "emma.reddy@example.com", PhoneNumber = "9845123480" },
            new Contact { Id = 4, Name = "Noah Mehta", Email = "noah.mehta@example.com", PhoneNumber = "9911223344" },
            new Contact { Id = 5, Name = "Sophia Shah", Email = "sophia.shah@example.com", PhoneNumber = "9887766554" },
        });

        modelBuilder.Entity<Contact>()
            .HasIndex(contact => contact.Email)
            .IsUnique();
        
        modelBuilder.Entity<Contact>()
            .HasIndex(contact => contact.PhoneNumber)
            .IsUnique();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();
        optionsBuilder.UseSqlServer(configuration.GetConnectionString("SchoolDBLocalConnection"));
    }
}