using InternTask1.Website.Models;
using InternTask2.ConsoleApp.Models;
using InternTask2.ConsoleApp.Properties;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security;
using System.Threading;
using System.Threading.Tasks;

namespace InternTask2.ConsoleApp
{
    class Program
    {
        static readonly string SiteUrl = Settings.Default.SPSiteUrl;
        static readonly string DocumentLib = Settings.Default.SPDocLibName;
        static readonly string UserName = Settings.Default.SPLogin;
        static readonly string Password = Settings.Default.SPPass;
        static readonly int SleepTime = Settings.Default.SleepTimeInMinutes;
        static bool isWorking = false;
        static void Main(string[] args)
        {
            int choose;
            do
            {
                Console.Clear();
                if (isWorking)
                    Console.WriteLine("Programm is running");
                else
                    Console.WriteLine("Programm is waiting");
                Console.WriteLine("1 - Start");
                Console.WriteLine("2 - Stop");
                Console.WriteLine("3 - Exit");
                if (int.TryParse(Console.ReadLine(), out choose))
                    switch (choose)
                    {
                        case 1:
                            {
                                if (!isWorking)
                                {
                                    isWorking = !isWorking;
                                    StartProgramm();
                                }
                                else
                                {
                                    Console.WriteLine("Programm is working");
                                }
                                break;
                            }
                        case 2:
                            {
                                isWorking = false;
                                break;
                            }
                        case 3: break;
                        default:
                            {
                                Console.WriteLine("Select an action");
                                Thread.Sleep(1000);
                                break;
                            }
                    }
            } while (choose != 3);
            Console.ReadKey();
        }

        static async void StartProgramm()
        {
            SecureString pass = new SecureString();
            foreach (char c in Password) pass.AppendChar(c);
            SharePointOnlineCredentials credential = new SharePointOnlineCredentials(
    UserName, pass);
            await Task.Run(() =>
            {
                while (isWorking)
                {
                    SyncingWithSharaPointNDatabase(credential, SiteUrl, DocumentLib);
                    Thread.Sleep(SleepTime * 60 * 1000);
                }
            });
        }

        static void SyncingWithSharaPointNDatabase(SharePointOnlineCredentials credentialsSP, string siteUrlSP, string dlNameSP)
        {
            using (ClientContext context = new ClientContext(siteUrlSP))
            {
                context.Credentials = credentialsSP;
                Web web = context.Web;

                FileCollection docLibSPFiles = web.Folders.GetByUrl(dlNameSP).Files;
                context.Load(docLibSPFiles);
                context.ExecuteQuery();
                var documentsSP = new List<Document>();
                var documentsDB = new List<Document>();
                foreach (Microsoft.SharePoint.Client.File file in docLibSPFiles)
                {
                    context.Load(file);
                    var fileInfo = file.OpenBinaryStream();
                    context.ExecuteQuery();
                    byte[] content = null;
                    using (BinaryReader br = new BinaryReader(fileInfo.Value))
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
                using (var db = new DocumentContext())
                {
                    documentsDB = db.Documents.ToList();
                    var toInsertDocs = documentsSP.Except(documentsDB, new DocumentComparer());
                    var toDeleteDocs = documentsDB.Except(documentsSP, new DocumentComparer());
                    foreach (Document docs in toDeleteDocs)
                        db.Entry(docs).State = System.Data.Entity.EntityState.Deleted;
                    db.Documents.AddRange(toInsertDocs);
                    db.SaveChanges();
                }
            }
        }
    }
}
