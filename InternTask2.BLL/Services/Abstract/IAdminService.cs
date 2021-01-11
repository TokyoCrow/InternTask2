using InternTask2.BLL.Models;
using System;
using System.Collections.Generic;

namespace InternTask2.BLL.Services.Abstract
{
    public interface IAdminService : IDisposable
    {
        void SendApproveEmail(UserDTO user);
        void SendRejectEmail(UserDTO user);
        IEnumerable<UserDTO> GetPageUsers(int page,int pageSize);
        IEnumerable<UserDTO> GetUnChecktedPageUser(int page, int pageSize);
        IEnumerable<RoleDTO> GetAllRoles();
        void UpdateUserRole(UserDTO user);
        UserDTO GetUserById(int id);
        bool IsUserExists(int id);
        int UsersCount();
    }
}
