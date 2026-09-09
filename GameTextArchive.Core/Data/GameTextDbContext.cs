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
    
    // more entities for frequency analysis.
    public DbSet<Lexeme> Lexemes => Set<Lexeme>();
    public DbSet<TextRecordLexeme> TextRecordLexemes => Set<TextRecordLexeme>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // map metadata dictionary to jsonb.
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
        
        // primary keys.
        modelBuilder.Entity<TextRecord>()
            .HasKey(x => x.recordId);
        
        modelBuilder.Entity<Lexeme>()
            .HasKey(x => x.lexemeId);
        
        // each lexeme value should exist only once.
        modelBuilder.Entity<Lexeme>()
            .HasIndex(x => x.Value)
            .IsUnique();

        // key for text record lexeme join model is composite key of external id values for record and lexeme.
        // points back to original record and lexeme for webpage query.
        modelBuilder.Entity<TextRecordLexeme>().HasKey(x => new
        {
            x.recordId,
            x.lexemeId
        });
        
        // text record lexeme has one record associated with many lexemes and one lexeme associated with many 
        // records. 
        modelBuilder.Entity<TextRecordLexeme>()
            .HasOne(x => x.record)
            .WithMany()
            .HasForeignKey(x => x.recordId);

        modelBuilder.Entity<TextRecordLexeme>()
            .HasOne(x => x.lexeme)
            .WithMany()
            .HasForeignKey(x => x.lexemeId);
    }
}