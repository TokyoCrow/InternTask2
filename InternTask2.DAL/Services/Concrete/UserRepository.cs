using InternTask2.Core.Models;
using InternTask2.DAL.Models;
using InternTask2.DAL.Services.Abstract;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace InternTask2.DAL.Services.Concrete
{
    public class UserRepository : IRepository<User>
    {
        private AppEFContext db;

        public UserRepository(AppEFContext context) => db = context;

        public int Count() => db.Users.Count();

        public void Create(User entity) => db.Users.Add(entity);

        public void Delete(int id)
        {
            var user = db.Users.Find(id);
            if (user != null)
                db.Users.Remove(user);
        }

        public IEnumerable<User> Find(Func<User, bool> predicate) => 
            db.Users.Include(u => u.Role)
                    .Include(u => u.Sex)
                    .Where(predicate)
                    .ToList();

        public User Get(int id) => 
            db.Users.Include(u => u.Role)
                    .Include(u => u.Sex)
                    .FirstOrDefault(u => u.Id == id);

        public IEnumerable<User> GetAll() => 
            db.Users.Include(u => u.Role)
                    .Include(u => u.Sex);

        public void Update(User entity) => db.Entry(entity).State = EntityState.Modified;
    }
}
