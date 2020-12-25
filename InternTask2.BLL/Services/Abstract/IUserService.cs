using InternTask2.BLL.Models;
using System;
using System.Collections.Generic;
using System.Web;

namespace InternTask2.BLL.Services.Abstract
{
    public interface IUserService : IDisposable
    {
        void UploadDocument(HttpPostedFileBase document);
        DocumentDTO GetDocumentById(int id);
        DocumentDTO GetDocumentByName(string name);
        IEnumerable<DocumentDTO> GetDocumentsPage(int page, int pageSize);
        int DocumentsCount();
    }
}
