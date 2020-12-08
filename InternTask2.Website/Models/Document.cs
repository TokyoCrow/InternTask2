using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace InternTask2.Website.Models
{
    public class Document
    {
        public int Id { get; set; }
        [Required]
        [Remote(action:"IsDocumentNameUnique", controller:"User",AdditionalFields = nameof(Name))]
        public string Name { get; set; }
        [Required]
        public DateTime Modified { get; set; }
        [Required]
        public byte[] Content { get; set; }
    }
}
