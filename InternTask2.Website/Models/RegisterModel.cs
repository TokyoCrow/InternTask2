using InternTask2.Core.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InternTask2.Website.Models
{
    public class RegisterModel
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Surname { get; set; }
        public string Patronymic { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Column(TypeName = "date")]
        public DateTime? BirthDate { get; set; }
        public int? RoleId { get; set; }
        public Role Role { get; set; }
        public int? SexId { get; set; }
        public Sex Sex { get; set; }
        public string Workplace { get; set; }
        public string Position { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
    }
}
