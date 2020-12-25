using System;
using System.Collections.Generic;

namespace InternTask2.DAL.Services.Abstract
{
    public interface IRepository<T> where T: class
    {
        IEnumerable<T> GetAll();
        T Get(int id);
        IEnumerable<T> Find(Func<T, Boolean> predicate);
        int Count();
        void Create(T entity);
        void Update(T entity);
        void Delete(int id);
    }
}
