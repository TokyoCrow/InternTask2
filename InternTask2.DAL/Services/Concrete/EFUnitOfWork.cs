using InternTask2.Core.Models;
using InternTask2.DAL.Models;
using InternTask2.DAL.Services.Abstract;
using System;

namespace InternTask2.DAL.Services.Concrete
{
    public class EFUnitOfWork : IUnitOfWork
    {
        private AppEFContext db;
        private DocumentRepository documentRepository;
        private UserRepository userRepository;
        private SexRepository sexRepository;
        private RoleRepository roleRepository;
        private bool disposed = false;

        public EFUnitOfWork(string connectionString)
        {
            db = new AppEFContext(connectionString);
        }

        public IRepository<User> Users
        {
            get
            {
                if (userRepository == null)
                    userRepository = new UserRepository(db);
                return userRepository;
            }
        }

        public IRepository<Document> Documents
        {
            get
            {
                if (documentRepository == null)
                    documentRepository = new DocumentRepository(db);
                return documentRepository;
            }
        }

        public IRepository<Sex> Sexes
        {
            get
            {
                if (sexRepository == null)
                    sexRepository = new SexRepository(db);
                return sexRepository;
            }
        }

        public IRepository<Role> Roles
        {
            get
            {
                if (roleRepository == null)
                    roleRepository = new RoleRepository(db);
                return roleRepository;
            }
        }

        public virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                    db.Dispose();
                disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Save() => db.SaveChanges();
    }
}
