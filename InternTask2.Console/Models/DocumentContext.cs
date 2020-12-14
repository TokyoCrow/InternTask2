using InternTask1.Website.Models;
using System.Data.Entity;

namespace InternTask2.ConsoleApp.Models
{
    public class DocumentContext : DbContext
    {
        public DbSet<Document> Documents { get; set; }
        public DocumentContext() : base("DefaultConnection") { }
    }
}
