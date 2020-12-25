using AutoMapper;
using InternTask2.BLL.Models;
using InternTask2.BLL.Services.Abstract;
using InternTask2.Core.Models;
using InternTask2.DAL.Services.Abstract;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace InternTask2.BLL.Services.Concrete
{
    public class UserService : IUserService
    {
        IUnitOfWork db;
        ISPManager spManager;

        public UserService(IUnitOfWork db, ISPManager spm)
        {
            this.db = db;
            spManager = spm;
        }

        public void Dispose() => db.Dispose();

        public IEnumerable<DocumentDTO> GetDocumentsPage(int page, int pageSize)
        {
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<Document, DocumentDTO>()).CreateMapper();
            return mapper.Map<IEnumerable<Document>, List<DocumentDTO>>(
                db.Documents
                    .GetAll()
                    .OrderBy(d => d.Id)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                );
        }

        public DocumentDTO GetDocumentByName(string name)
        {
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<Document, DocumentDTO>()).CreateMapper();
            return mapper.Map<Document, DocumentDTO>(db.Documents.Find(doc => doc.Name == name).FirstOrDefault());
        }

        public void UploadDocument(HttpPostedFileBase document)
        {
            var fileName = Path.GetFileName(document.FileName);
            var documentUpload = db.Documents.Find(doc => doc.Name == fileName).FirstOrDefault();
            if (documentUpload == null)
            {
                byte[] content;
                using (var binaryReader = new BinaryReader(document.InputStream))
                    content = binaryReader.ReadBytes(document.ContentLength);
                documentUpload = new Document
                {
                    Content = content,
                    Name = fileName
                };
                documentUpload.Modified = spManager.Create(documentUpload);
                db.Documents.Create(documentUpload);

                db.Save();
            }
            else
                throw new ValidationException("Document already exists", "");
        }

        public DocumentDTO GetDocumentById(int id)
        {
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<Document, DocumentDTO>()).CreateMapper();
            return mapper.Map<Document, DocumentDTO>(db.Documents.Find(doc => doc.Id == id).FirstOrDefault());
        }

        public int DocumentsCount() => db.Documents.Count();
    }
}
