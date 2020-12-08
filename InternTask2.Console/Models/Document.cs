using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace InternTask1.Website.Models
{
    public class Document
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Modified { get; set; }
        public byte[] Content { get; set; }
    }

    public class DocumentComparer : IEqualityComparer<Document>
    {
        // documents are equal if their names and document numbers are equal.
        public bool Equals(Document x, Document y)
        {

            //Check whether the compared objects reference the same data.
            if (Object.ReferenceEquals(x, y)) return true;

            //Check whether any of the compared objects is null.
            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                return false;

            //Check whether the documents' properties are equal.
            return x.Name == y.Name && x.Modified.ToString("G") == y.Modified.ToString("G");
        }

        // If Equals() returns true for a pair of objects
        // then GetHashCode() must return the same value for these objects.

        public int GetHashCode(Document document)
        {
            //Check whether the object is null
            if (Object.ReferenceEquals(document, null)) return 0;

            //Get hash code for the Name field if it is not null.
            int hashdocumentName = document.Name == null ? 0 : document.Name.GetHashCode();

            //Get hash code for the Code field.
            int hashdocumentCode = document.Modified.ToString("G").GetHashCode();

            //Calculate the hash code for the document.
            return hashdocumentName ^ hashdocumentCode;
        }
    }
}
