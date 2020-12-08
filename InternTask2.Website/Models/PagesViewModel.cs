using System.Collections.Generic;

namespace InternTask2.Website.Models
{
    public class UsersViewModel
    {
        public IEnumerable<User> Users { get; set; } 
        public PageInfo PageInfo { get; set; }
    }

    public class DocumentsViewModel
    {
        public IEnumerable<Document> Documents { get; set; } 
        public PageInfo PageInfo { get; set; }
    }
}