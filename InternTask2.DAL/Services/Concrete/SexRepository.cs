using InternTask2.Core.Models;
using InternTask2.DAL.Models;
using InternTask2.DAL.Services.Abstract;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace InternTask2.DAL.Services.Concrete
{
    public class SexRepository : IRepository<Sex>
    {
        private AppEFContext db;

        public SexRepository(AppEFContext context) => db = context;
        public int Count() => db.Sexes.Count();
        public void Create(Sex entity) => db.Sexes.Add(entity);

        public void Delete(int id)
        {
            var sex = db.Sexes.Find(id);
            if (sex != null)
                db.Sexes.Remove(sex);
        }

        public IEnumerable<Sex> Find(Func<Sex, bool> predicate) => db.Sexes.Where(predicate).ToList();

        public Sex Get(int id) => db.Sexes.Find(id);

        public IEnumerable<Sex> GetAll() => db.Sexes;

        public void Update(Sex entity) => db.Entry(entity).State = EntityState.Modified;
    }
}
