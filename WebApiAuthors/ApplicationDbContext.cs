using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApiAuthors.Entities;

namespace WebApiAuthors
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Author_Book>().HasKey(ab => new { ab.AuthorId, ab.BookId });
        }

        public DbSet<Author> Author { get; set; }
        public DbSet<Book> Book { get; set; }
        public DbSet<Comment> Comment { get; set; }
        public DbSet<Author_Book> Author_Book { get; set; }
        public DbSet<APIKey> APIKey { get; set; }
        public DbSet<Request> Request { get; set; }
        public DbSet<DomainRestriction> DomainRestriction { get; set; }
        public DbSet<IPRestriction> IPRestriction { get; set; }
    }
}
