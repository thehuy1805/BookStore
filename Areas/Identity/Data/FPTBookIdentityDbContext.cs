using FPTBook.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using FPTBook.Models;

namespace FPTBook.Areas.Identity.Data;

public class FPTBookIdentityDbContext : IdentityDbContext<FPTBookUser>
{
    public FPTBookIdentityDbContext(DbContextOptions<FPTBookIdentityDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<FPTBook.Models.Book>().Property(p => p.Price).HasColumnType("decimal(18,4)");
    }
    public DbSet<FPTBook.Models.Author> Author { get; set; } = default!;

    public DbSet<FPTBook.Models.Category>? Category { get; set; }

    public DbSet<FPTBook.Models.Publisher>? Publisher { get; set; }

    public DbSet<FPTBook.Models.Book>? Book { get; set; }
     public DbSet<FPTBook.Models.Order>? Order { get; set; }

    public DbSet<FPTBook.Models.OrderDetail>? OrderDetail { get; set; }
}


