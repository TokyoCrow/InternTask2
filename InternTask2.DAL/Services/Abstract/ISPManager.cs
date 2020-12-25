using InternTask2.Core.Models;
using System;
using System.Collections.Generic;

namespace InternTask2.DAL.Services.Abstract
{
    public interface ISPManager
    {
        IEnumerable<Document> GetAllDocuments();
        DateTime Create(Document document);
        int Create(User user);
        void Update(User user);
        void DeleteUser(int spId);
    }
}
