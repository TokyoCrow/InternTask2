using System;

namespace InternTask2.BLL.Models
{
    public class DocumentDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Modified { get; set; }
        public byte[] Content { get; set; }
    }
}
