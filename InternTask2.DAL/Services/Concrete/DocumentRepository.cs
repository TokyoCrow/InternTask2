using InternTask2.Core.Models;
using InternTask2.DAL.Models;
using InternTask2.DAL.Services.Abstract;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace InternTask2.DAL.Services.Concrete
{
    public class DocumentRepository : IRepository<Document>
    {
        private AppEFContext db;

        public DocumentRepository(AppEFContext context) => db = context;
        public int Count() => db.Documents.Count();

        public void Create(Document entity) => db.Documents.Add(entity);

        public void Delete(int id)
        {
            var Document = db.Documents.Find(id);
            if (Document != null)
                db.Documents.Remove(Document);
        }

        public IEnumerable<Document> Find(Func<Document, bool> predicate) => db.Documents.Where(predicate).ToList();

        public Document Get(int id) => db.Documents.Find(id);

        public IEnumerable<Document> GetAll() => db.Documents;

        public void Update(Document entity) => db.Entry(entity).State = EntityState.Modified;
    }
}
