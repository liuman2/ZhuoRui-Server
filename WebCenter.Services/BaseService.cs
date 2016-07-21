using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCenter.IServices;
using WebCenter.IDAL;
using WebCenter.Services;
using WebCenter.DAL;

namespace WebCenter.Services
{
    public class BaseService<T> where T : class,new()
    {
        public IDAL.IBaseRepository<T> CurrentRepository { get; set; }

        public IDbSession _dbSession = DbSessionFactory.GetCurrentDbSession();

        /// <summary>
        /// 基类的构造函数
        /// </summary>
        public BaseService()
        {
            SetCurrentRepository();  //构造函数里面调用了此设置当前仓储的抽象方法
        }

        /// <summary>
        /// 构造方法实现赋值
        /// </summary>
        public virtual void SetCurrentRepository() { }  //约束子类必须实现这个抽象方法

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public T AddEntity(T entity)
        {
            //如果在这里操作多个表的话，实现批量的操作
            //调用T对应的仓储来添加
            CurrentRepository.Add(entity);

            _dbSession.SaveChanges();
            return entity;
        }

        /// <summary>
        /// 批量新增
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public int AddEntities(IList<T> list)
        {
            foreach (var entity in list)
            {
                CurrentRepository.Add(entity);
            }
            return _dbSession.SaveChanges();
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool UpdateEntity(T entity)
        {
            CurrentRepository.Update(entity);
            int count = _dbSession.SaveChanges();
            return count > 0;
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public bool UpdateEntities(IList<T> list)
        {
            foreach (T entity in list)
            {
                CurrentRepository.Update(entity);

            }
            return _dbSession.SaveChanges() > 0;
        }
        public bool DeleteEntity(int id)
        {
            CurrentRepository.Delete(id);
            return _dbSession.SaveChanges() > 0;
        }

        public bool DeleteEntity(T entity)
        {
            CurrentRepository.Delete(entity);
            return _dbSession.SaveChanges() > 0;
        }
        public int DeleteEntity(System.Linq.Expressions.Expression<Func<T, bool>> where)
        {
            CurrentRepository.Delete(where);

            int effectCount = _dbSession.SaveChanges();
            return effectCount;
        }

        public IQueryable<T> GetAll(System.Linq.Expressions.Expression<Func<T, bool>> where = null, params string[] includes)
        {
            if (where == null)
            {
                return CurrentRepository.GetAll();
            }
            else
            {
                return CurrentRepository.GetAll(where);
            }
        }

        public IQueryable<T> GetAll(params string[] includes)
        {
            return CurrentRepository.GetAll(includes);
        } 
        public IQueryable<T> GetAll()
        {
            return CurrentRepository.GetAll();
        }

        public T GetById(int id)
        {
            return CurrentRepository.GetById(id);
        }

        public int Save(int? id, T entity)
        {
            CurrentRepository.Save(id, entity);
            int effectCount = _dbSession.SaveChanges();
            return effectCount;
        }








    }
}
