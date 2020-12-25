using InternTask2.BLL.Services.Abstract;
using InternTask2.Core.Models;
using InternTask2.Core.Services.Concrete;
using InternTask2.DAL.Services.Abstract;
using System;
using System.Linq;

namespace InternTask2.BLL.Services.Concrete
{
    public class SPAndDBSynchronizer : ISPAndDBSynchronizer
    {
        IUnitOfWork db;
        ISPManager spManager;

        public SPAndDBSynchronizer(IUnitOfWork uow,ISPManager spm)
        {
            db = uow;
            spManager = spm;
        }
        
        public void SyncSPAndDB()
        {
            var documentsSP = spManager.GetAllDocuments();
            var documentsDB = db.Documents.GetAll();
            var toInsertDocs = documentsSP.Except(documentsDB, new DocumentComparer());
            var toDeleteDocs = documentsDB.Except(documentsSP, new DocumentComparer());
            foreach (Document doc in toDeleteDocs)
                db.Documents.Delete(doc.Id); 
            foreach (Document doc in toInsertDocs)
                db.Documents.Create(doc);
            try
            {
                db.Save();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public void Dispose() => db.Dispose();
    }
}
