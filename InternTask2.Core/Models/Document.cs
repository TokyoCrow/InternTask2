using System;
using System.ComponentModel.DataAnnotations;

namespace InternTask2.Core.Models
{
    public class Document
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public DateTime Modified { get; set; }
        [Required]
        public byte[] Content { get; set; }
    }
}
