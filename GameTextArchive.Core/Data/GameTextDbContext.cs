using Microsoft.EntityFrameworkCore;

namespace GameTextArchive.Data;

// inherits from ef core db context base class.
public class GameTextDbContext : DbContext
{
    // constructor passes ef core config options to base db context constructor.
    public GameTextDbContext(
        DbContextOptions<GameTextDbContext> options) 
        : base(options) {}

    // creates a set of db entities of type text record and exposes a getter.
    public DbSet<TextRecord> TextRecords => Set<TextRecord>();
    
    // map metadata dictionary to jsonb.
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TextRecord>()
                    .Property(x => x.Metadata)
                    .HasColumnType("jsonb");
        
        modelBuilder.Entity<TextRecord>()
                    .HasGeneratedTsVectorColumn(
                        p => p.SearchVector,
                        "english", // dictionary config for normalization, etc.
                        p => new { p.Text })
                    .HasIndex(p => p.SearchVector)
                    .HasMethod("GIN");
    }
}