using InternTask2.Core.Models;
using System.Collections.Generic;

namespace InternTask2.BLL.Models
{
    public class SexDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<User> Users { get; set; }

        public SexDTO()
        {
            Users = new List<User>();
        }
    }
}
