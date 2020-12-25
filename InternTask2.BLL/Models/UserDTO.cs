using InternTask2.Core.Models;
using System;

namespace InternTask2.BLL.Models
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Patronymic { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public DateTime? BirthDate { get; set; }
        public int? RoleId { get; set; }
        public Role Role { get; set; }
        public int? SexId { get; set; }
        public Sex Sex { get; set; }
        public string Workplace { get; set; }
        public string Position { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public bool IsApproved { get; set; }
        public int SPId { get; set; }
    }
}
