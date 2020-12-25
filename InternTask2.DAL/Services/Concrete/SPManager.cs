using InternTask2.Core.Models;
using InternTask2.DAL.Services.Abstract;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security;
using User = InternTask2.Core.Models.User;

namespace InternTask2.DAL.Services.Concrete
{
    public class SPManager : ISPManager
    {
        readonly string SiteUrl;
        readonly string DocLibName;
        readonly string ListName;
        readonly NetworkCredential Credentials;

        public SPManager(string siteUrl,string docLibName,string listName,string login,string password)
        {
            SiteUrl = siteUrl;
            DocLibName = docLibName;
            ListName = listName;
            var securePassword = new SecureString();
            foreach (char c in password) securePassword.AppendChar(c);
            Credentials = new NetworkCredential(login, securePassword);
        }

        public DateTime Create(Document document)
        {
            try
            {
                using (var clientContext = new ClientContext(SiteUrl))
                {
                    clientContext.Credentials = Credentials;
                    var web = clientContext.Web;
                    var newFile = new FileCreationInformation
                    {
                        Content = document.Content,
                        Url = document.Name
                    };
                    List docs = web.Lists.GetByTitle(DocLibName);
                    Microsoft.SharePoint.Client.File uFile = docs.RootFolder.Files.Add(newFile);
                    docs.Update();
                    clientContext.Load(docs);
                    clientContext.ExecuteQuery();
                    return uFile.TimeLastModified;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return DateTime.UtcNow;
        }

        public int Create(User user)
        {
            try
            {
                using (var clientContext = new ClientContext(SiteUrl))
                {
                    clientContext.Credentials = Credentials;
                    var web = clientContext.Web;
                    List users = web.Lists.GetByTitle(ListName);
                    var listItemCreationInformation = new ListItemCreationInformation();
                    ListItem newUser = users.AddItem(listItemCreationInformation);
                    newUser["Name"] = user.Name;
                    newUser["Surname"] = user.Surname;
                    newUser["Patronymic"] = user.Patronymic;
                    newUser["Email"] = user.Email;
                    newUser["BirthDate"] = user.BirthDate;
                    newUser["Sex"] = user.Sex.Name;
                    newUser["Workplace"] = user.Workplace;
                    newUser["Position"] = user.Position;
                    newUser["Country"] = user.Country;
                    newUser["City"] = user.City;
                    newUser["IsApprove"] = user.IsApproved;

                    newUser.Update();
                    clientContext.Load(newUser);
                    clientContext.ExecuteQuery();
                    return newUser.Id;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return 0;
        }

        public void DeleteUser(int spId)
        {
            try
            {
                using (var clientContext = new ClientContext(SiteUrl))
                {
                    clientContext.Credentials = Credentials;
                    var web = clientContext.Web;
                    List users = web.Lists.GetByTitle(ListName);
                    ListItem userItem = users.GetItemById(spId);
                    userItem.DeleteObject();

                    userItem.Update();
                    clientContext.ExecuteQuery();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public IEnumerable<Document> GetAllDocuments()
        {
            var documentsSP = new List<Document>();
            using (var context = new ClientContext(SiteUrl))
            {
                context.Credentials = Credentials;
                var web = context.Web;

                FileCollection docLibSPFiles = web.Folders.GetByUrl(DocLibName).Files;
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

        public void Update(User user)
        {
            try
            {
                using (var clientContext = new ClientContext(SiteUrl))
                {
                    clientContext.Credentials = Credentials;
                    var web = clientContext.Web;
                    List users = web.Lists.GetByTitle(ListName);
                    ListItem userItem = users.GetItemById(user.SPId);
                    userItem["Name"] = user.Name;
                    userItem["Surname"] = user.Surname;
                    userItem["Patronymic"] = user.Patronymic;
                    userItem["Email"] = user.Email;
                    userItem["BirthDate"] = user.BirthDate;
                    userItem["Sex"] = user.Sex.Name;
                    userItem["Workplace"] = user.Workplace;
                    userItem["Position"] = user.Position;
                    userItem["Country"] = user.Country;
                    userItem["City"] = user.City;
                    userItem["IsApprove"] = user.IsApproved;

                    userItem.Update();
                    clientContext.Load(userItem);
                    clientContext.ExecuteQuery();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
