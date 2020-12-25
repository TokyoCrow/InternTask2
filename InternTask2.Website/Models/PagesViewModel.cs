using System.Collections.Generic;

namespace InternTask2.Website.Models
{
    public class UsersViewModel
    {
        public IEnumerable<UserView> Users { get; set; } 
        public PageInfo PageInfo { get; set; }
    }

    public class DocumentsViewModel
    {
        public IEnumerable<DocumentView> Documents { get; set; } 
        public PageInfo PageInfo { get; set; }
    }
}