using InternTask2.DAL.Services.Abstract;
using Microsoft.SharePoint.Client;
using System;
using System.Net;
using System.Security;

namespace InternTask2.DAL.Services.Concrete
{
    public class SPInitializer : ISPInitializer
    {
        readonly string SiteUrl;
        readonly string DocLibName;
        readonly string ListName;
        readonly NetworkCredential Credentials;

        public SPInitializer(string siteUrl, string docLibName, string listName, string login, string password)
        {
            SiteUrl = siteUrl;
            DocLibName = docLibName;
            ListName = listName;
            var securePassword = new SecureString();
            foreach (char c in password) securePassword.AppendChar(c);
            Credentials = new NetworkCredential(login, securePassword);
        }

        public void Initialize()
        {
            CreateUsersList();
            CreateDocLibNamerary();
        }

        private void CreateUsersList()
        {
            try
            {
                using (var clientContext = new ClientContext(SiteUrl))
                {
                    clientContext.Credentials = Credentials;
                    var web = clientContext.Web;
                    if (!ListExist(web, clientContext, ListName))
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

        private void CreateDocLibNamerary()
        {
            try
            {
                using (var clientContext = new ClientContext(SiteUrl))
                {
                    clientContext.Credentials = Credentials;
                    var web = clientContext.Web;
                    if (!ListExist(web, clientContext, DocLibName))
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

        bool ListExist(Web web, ClientContext clientContext, string listName)
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
    }
}
