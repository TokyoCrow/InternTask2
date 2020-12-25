using InternTask2.Core.Models;
using System;
using System.Collections.Generic;

namespace InternTask2.Core.Services.Concrete
{
    public class DocumentComparer : IEqualityComparer<Document>
    {
        public bool Equals(Document x, Document y)
        {
            if (Object.ReferenceEquals(x, y)) return true;

            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                return false;

            return x.Name == y.Name && x.Modified.ToString("G") == y.Modified.ToString("G");
        }

        public int GetHashCode(Document document)
        {
            if (Object.ReferenceEquals(document, null)) return 0;

            int hashdocumentName = document.Name == null ? 0 : document.Name.GetHashCode();

            int hashdocumentCode = document.Modified.ToString("G").GetHashCode();

            return hashdocumentName ^ hashdocumentCode;
        }
    }
}
