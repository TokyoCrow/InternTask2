using InternTask2.Core.Models;
using InternTask2.DAL.Models;
using InternTask2.DAL.Services.Abstract;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace InternTask2.DAL.Services.Concrete
{
    public class RoleRepository : IRepository<Role>
    {
        private AppEFContext db;

        public RoleRepository(AppEFContext context) => db = context; 
        public int Count() => db.Roles.Count();

        public void Create(Role entity) => db.Roles.Add(entity);

        public void Delete(int id)
        {
            var role = db.Roles.Find(id);
            if (role != null)
                db.Roles.Remove(role);
        }

        public IEnumerable<Role> Find(Func<Role, bool> predicate) => db.Roles.Where(predicate).ToList();

        public Role Get(int id) => db.Roles.Find(id);

        public IEnumerable<Role> GetAll() => db.Roles;

        public void Update(Role entity) => db.Entry(entity).State = EntityState.Modified;
    }
}
