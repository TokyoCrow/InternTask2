using InternTask2.Website.Models;
using InternTask2.Website.Properties;
using Microsoft.SharePoint.Client;
using System;
using System.Security;
using User = InternTask2.Website.Models.User;

namespace InternTask2.Website.Helpers
{
    public static class SharePointManager
    {
        static readonly string SiteUrl;
        static readonly string DocLibName;
        static readonly string ListName;
        static readonly string Login;
        static readonly SecureString SecurePassword;
        static readonly SharePointOnlineCredentials Credentials;

        static SharePointManager()
        {
            SiteUrl = Settings.Default.SPSiteUrl;
            DocLibName = Settings.Default.SPDocLibName;
            ListName = Settings.Default.SPListName;
            Login = Settings.Default.SPLogin;
            SecurePassword = new SecureString();
            foreach (char c in Settings.Default.SPPass) SecurePassword.AppendChar(c);
            Credentials = new SharePointOnlineCredentials(Login, SecurePassword);
        }
        public static void CreateListNLibrary()
        {
            CreateUsersList();
            CreateDocLibNamerary();
        }

        static bool ListExist(this Web web, ClientContext clientContext, string listName)
        {
            try
            {
                var list = web.Lists.GetByTitle(listName);
                clientContext.Load(list);
                clientContext.ExecuteQuery();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static DateTime AddNewDocument(Document document)
        {
            try
            {
                using(var clientContext = new ClientContext(SiteUrl))
                {
                    clientContext.Credentials = Credentials;
                    Web web = clientContext.Web;
                    var newFile = new FileCreationInformation
                    {
                        Content = document.Content,
                        Url = document.Name
                    };
                    List docs = web.Lists.GetByTitle(DocLibName);
                    File uFile = docs.RootFolder.Files.Add(newFile);
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

        public static int AddUserToCustomList(User user)
        {
            try
            {
                using (var clientContext = new ClientContext(SiteUrl))
                {
                    clientContext.Credentials = Credentials;
                    Web web = clientContext.Web;
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

        public static void ApproveUser(User user)
        {
            try
            {
                using (var clientContext = new ClientContext(SiteUrl))
                {
                    clientContext.Credentials = Credentials;
                    Web web = clientContext.Web;
                    List users = web.Lists.GetByTitle(ListName);
                    ListItem userItem = users.GetItemById(user.SPId);
                    userItem["IsApprove"] = true;

                    userItem.Update();
                    clientContext.ExecuteQuery();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        
        public static void UpdateUser(User user)
        {
            try
            {
                using (var clientContext = new ClientContext(SiteUrl))
                {
                    clientContext.Credentials = Credentials;
                    Web web = clientContext.Web;
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
                    userItem["IsApprove"] = user.IsApproved ? true : false;

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

        public static void RejectUser(User user)
        {
            try
            {
                using (var clientContext = new ClientContext(SiteUrl))
                {
                    clientContext.Credentials = Credentials;
                    Web web = clientContext.Web;
                    List users = web.Lists.GetByTitle(ListName);
                    ListItem userItem = users.GetItemById(user.SPId);
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

        private static void CreateDocLibNamerary()
        {
            try
            {
                using (var clientContext = new ClientContext(SiteUrl))
                {
                    clientContext.Credentials = Credentials;
                    Web web = clientContext.Web;
                    if (!web.ListExist(clientContext, DocLibName))
                    {
                        var createDL = new ListCreationInformation
                        {
                            Title = DocLibName,
                            TemplateType = (int)ListTemplateType.DocumentLibrary
                        };
                        web.Lists.Add(createDL);
                        clientContext.ExecuteQuery();
                    }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private static void CreateUsersList()
        {
            try
            {
                using (var clientContext = new ClientContext(SiteUrl))
                {
                    clientContext.Credentials = Credentials;
                    Web web = clientContext.Web;
                    if (!web.ListExist(clientContext, ListName))
                    {
                        var listCreationInfo = new ListCreationInformation
                        {
                            Title = ListName,
                            TemplateType = (int)ListTemplateType.GenericList
                        };

                        List newList = web.Lists.Add(listCreationInfo);

                        Field nameField = newList.Fields.AddFieldAsXml("<Field DisplayName='Name' Type='Text' />",
                            true, AddFieldOptions.DefaultValue);
                        FieldText fieldName = clientContext.CastTo<FieldText>(nameField);
                        fieldName.Required = true;
                        fieldName.Update();

                        Field surnameField = newList.Fields.AddFieldAsXml("<Field DisplayName='Surname' Type='Text' />",
                            true, AddFieldOptions.DefaultValue);
                        FieldText fieldSurname = clientContext.CastTo<FieldText>(surnameField);
                        fieldSurname.Required = true;
                        fieldSurname.Update();

                        Field patronymicField = newList.Fields.AddFieldAsXml("<Field DisplayName='Patronymic' Type='Text' />",
                            true, AddFieldOptions.DefaultValue);
                        FieldText fieldPatronymic = clientContext.CastTo<FieldText>(patronymicField);
                        fieldPatronymic.Update();

                        Field emailField = newList.Fields.AddFieldAsXml("<Field DisplayName='Email' Type='Text' />",
                            true, AddFieldOptions.DefaultValue);
                        FieldText fieldEmail = clientContext.CastTo<FieldText>(emailField);
                        emailField.Required = true;
                        fieldEmail.Update();

                        Field BirthDateField = newList.Fields.AddFieldAsXml("<Field DisplayName='BirthDate' Type='DateTime' Format='DateOnly' />",
                            true, AddFieldOptions.DefaultValue);
                        FieldDateTime fieldBirthDate = clientContext.CastTo<FieldDateTime>(BirthDateField);
                        fieldBirthDate.Update();

                        Field sexField = newList.Fields.AddFieldAsXml("<Field DisplayName='Sex' Type='Choice' />",
                            true, AddFieldOptions.DefaultValue);
                        FieldChoice fieldSex = clientContext.CastTo<FieldChoice>(sexField);
                        fieldSex.Choices = new string[2] { "Male", "Female" };
                        fieldSex.Update();

                        Field workplaceField = newList.Fields.AddFieldAsXml("<Field DisplayName='Workplace' Type='Text' />",
                            true, AddFieldOptions.DefaultValue);
                        FieldText fieldWorkplace = clientContext.CastTo<FieldText>(workplaceField);
                        workplaceField.Update();

                        Field positionField = newList.Fields.AddFieldAsXml("<Field DisplayName='Position' Type='Text' />",
                            true, AddFieldOptions.DefaultValue);
                        FieldText fieldPositions = clientContext.CastTo<FieldText>(positionField);
                        positionField.Update();

                        Field countryField = newList.Fields.AddFieldAsXml("<Field DisplayName='Country' Type='Text' />",
                            true, AddFieldOptions.DefaultValue);
                        FieldText fieldCountry = clientContext.CastTo<FieldText>(countryField);
                        fieldCountry.Update();

                        Field cityField = newList.Fields.AddFieldAsXml("<Field DisplayName='City' Type='Text' />",
                            true, AddFieldOptions.DefaultValue);
                        FieldText fieldCity = clientContext.CastTo<FieldText>(cityField);
                        fieldCity.Update();

                        Field isApproveField = newList.Fields.AddFieldAsXml("<Field DisplayName='IsApprove' Type='Boolean' />",
                            true, AddFieldOptions.DefaultValue);
                        FieldChoice fieldIsApprove = clientContext.CastTo<FieldChoice>(isApproveField);
                        fieldIsApprove.Update();

                        clientContext.ExecuteQuery();

                        Field titleField = newList.Fields.GetByInternalNameOrTitle("Title");
                        clientContext.Load(titleField);
                        clientContext.ExecuteQuery();
                        titleField.Required = false;
                        titleField.Hidden = true;
                        titleField.Update();

                        clientContext.ExecuteQuery();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}