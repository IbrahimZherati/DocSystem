using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public DbSet<Document> Documents { get; set; }
    public DbSet<Student> Students { get; set; }
    public DbSet<DocumentProperty> DocumentProperties { get; set; }


    public AppDbContext()
    {

    }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

   
}