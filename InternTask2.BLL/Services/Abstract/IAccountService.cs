using InternTask2.BLL.Models;
using System;
using System.Collections.Generic;

namespace InternTask2.BLL.Services.Abstract
{
    public interface IAccountService : IDisposable
    {
        UserDTO FindUser(string email,string password);
        UserDTO FindUser(string email);
        IEnumerable<SexDTO> GetAllSexes();
        void Registration(UserDTO user);
    }
}
