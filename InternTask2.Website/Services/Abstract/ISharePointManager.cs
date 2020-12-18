using InternTask2.Website.Models;
using System;
namespace InternTask2.Website.Services.Abstract
{
    public interface ISharePointManager
    {
        void CreateListNLibrary();
        DateTime AddNewDocument(Document document);
        int AddUserToCustomList(User user);
        void ApproveUser(User user);
        void UpdateUser(User user);
        void RejectUser(User user);
    }
}