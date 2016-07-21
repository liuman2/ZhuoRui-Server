using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCenter.IServices
{
    public interface IBaseService<T> where T : class,new()
    {
        T AddEntity(T entity);

        /// <summary>
        /// 批量新增
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        int AddEntities(IList<T> list);


        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        bool UpdateEntity(T entity);


        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        bool UpdateEntities(IList<T> list);
        bool DeleteEntity(int id);
        bool DeleteEntity(T entity);
        int DeleteEntity(System.Linq.Expressions.Expression<Func<T, bool>> where);
         

        IQueryable<T> GetAll();
        IQueryable<T> GetAll(params string[] includes);


        IQueryable<T> GetAll(System.Linq.Expressions.Expression<Func<T, bool>> where = null,params string []includes);


        T GetById(int id);
        int Save(int? id, T entity);


    }
}
