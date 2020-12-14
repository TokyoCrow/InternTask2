using System;

namespace InternTask1.Website.Models
{
    public class Document
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Modified { get; set; }
        public byte[] Content { get; set; }
    }
}
