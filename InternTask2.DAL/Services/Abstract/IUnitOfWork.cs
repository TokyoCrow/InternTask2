using InternTask2.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace InternTask2.DAL.Services.Abstract
{
    public interface IUnitOfWork: IDisposable
    {
        IRepository<User> Users { get; }
        IRepository<Document> Documents { get; }
        IRepository<Sex> Sexes { get; }
        IRepository<Role> Roles { get; }
        void Save();
    }
}
