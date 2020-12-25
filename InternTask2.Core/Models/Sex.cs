using System.Collections.Generic;

namespace InternTask2.Core.Models
{
    public class Sex
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<User> Users { get; set; }

        public Sex()
        {
            Users = new List<User>();
        }
    }
}
