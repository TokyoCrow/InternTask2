using InternTask1.Website.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternTask2.ConsoleApp.Models
{
    public class DocumentContext : DbContext
    {
        public DbSet<Document> Documents { get; set; }
        public DocumentContext() : base("DefaultConnection") { }
    }
}
