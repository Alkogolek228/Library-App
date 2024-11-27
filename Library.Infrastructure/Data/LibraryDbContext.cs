using Library.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Library.Infrastructure.Data
{
    public class LibraryDbContext : DbContext
    {
        public DbSet<Book> Books { get; set; } = null!;
        public DbSet<Author> Authors { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<BookPick> BookPicks { get; set; } = null!;

        public LibraryDbContext(DbContextOptions<LibraryDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Book>(entity =>
            {
                entity.Property(e => e.ISBN)
                    .IsRequired()
                    .HasMaxLength(13)
                    .HasColumnType("character varying(13)");
            });

            modelBuilder.Entity<BookPick>(entity =>
            {
                entity.HasKey(bp => bp.Id);
                entity.HasOne(bp => bp.User)
                      .WithMany(u => u.BookPicks)
                      .HasForeignKey(bp => bp.UserId);
                entity.HasOne(bp => bp.Book)
                      .WithMany(b => b.BookPicks)
                      .HasForeignKey(bp => bp.BookId);
            });

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(LibraryDbContext).Assembly);
        }
    }
}