using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace WebCenter.IDAL
{
    /// <summary>
    /// 基仓储实现的方法
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IBaseRepository<T> where T : class,new()
    {

        T Add(T entity);
        void Update(T entity);
        void Save(int? id, T entity);
        void Delete(int id);
        void Delete(T item);
        void Delete(Expression<Func<T, bool>> where);
        T GetById(int id);
        IQueryable<T> GetAll(params string []includes);
        IQueryable<T> GetAll();
        IQueryable<T> GetAll(Expression<Func<T, bool>> where,params string []includes);
        int ExcuteCommand(string sql, string [] pars);


    }
}