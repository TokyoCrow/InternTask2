using InternTask2.Core.Models;
using System.Collections.Generic;

namespace InternTask2.BLL.Models
{
    public class RoleDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<User> Users { get; set; }

        public RoleDTO()
        {
            Users = new List<User>();
        }
    }
}
