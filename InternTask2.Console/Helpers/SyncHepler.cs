using InternTask1.Website.Models;
using InternTask2.ConsoleApp.Models;
using InternTask2.ConsoleApp.Properties;
using InternTask2.ConsoleApp.Services.Concrete;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;

namespace InternTask2.ConsoleApp.Helpers
{
    public static class SyncHepler
    {
        static readonly string SiteUrl;
        static readonly string DocumentLibName;
        static readonly string UserName;
        static readonly SharePointOnlineCredentials credentials;

        static SyncHepler()
        {
            SiteUrl = Settings.Default.SPSiteUrl;
            DocumentLibName = Settings.Default.SPDocLibName;
            UserName = Settings.Default.SPLogin;
            var Password = Settings.Default.SPPass;
            var securePassword = new SecureString();
            foreach (char c in Password) securePassword.AppendChar(c);
            credentials = new SharePointOnlineCredentials(UserName, securePassword);
        }

        public static void SyncingWithSharePointNDatabase()
        {
            var documentsSP = GetSPDocuments();
            SyncWithDB(documentsSP);
        }

        private static IEnumerable<Document>  GetSPDocuments()
        {
            var documentsSP = new List<Document>();
            using (var context = new ClientContext(SiteUrl))
            {
                context.Credentials = credentials;
                var web = context.Web;

                FileCollection docLibSPFiles = web.Folders.GetByUrl(DocumentLibName).Files;
                context.Load(docLibSPFiles);
                context.ExecuteQuery();
                foreach (Microsoft.SharePoint.Client.File file in docLibSPFiles)
                {
                    context.Load(file);
                    var fileInfo = file.OpenBinaryStream();
                    context.ExecuteQuery();
                    byte[] content = null;
                    using (var br = new BinaryReader(fileInfo.Value))
                    {
                        content = br.ReadBytes((int)fileInfo.Value.Length);
                    }
                    documentsSP.Add(new Document
                    {
                        Name = file.Name,
                        Content = content,
                        Modified = file.TimeLastModified
                    });
                }
            }
            return documentsSP;
        }

        private static void SyncWithDB(IEnumerable<Document> documentsSP)
        {
            using (var db = new DocumentContext())
            {
                var documentsDB = db.Documents.ToList();
                var toInsertDocs = documentsSP.Except(documentsDB, new DocumentComparer());
                var toDeleteDocs = documentsDB.Except(documentsSP, new DocumentComparer());
                foreach (Document docs in toDeleteDocs)
                    db.Entry(docs).State = System.Data.Entity.EntityState.Deleted;
                db.Documents.AddRange(toInsertDocs);
                try
                {
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }
    }
}
