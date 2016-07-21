using System;
using System.Collections.Generic;
using System.Data;

using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

using System.Reflection;
using System.Linq.Expressions;
using WebCenter.Entities;


namespace WebCenter.DAL
{
    public partial class BaseRepository<T> where T : class
    {


        private DbContext db = EFContextFactory.GetCurrentDbContext();
        protected DbContext CurrentDb
        {
            get
            {
                return db;
            }
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="entity"></param>
        public virtual T Add(T entity)
        {
            AssignPrimeKey(entity);
            db.Set<T>().Add(entity);
            return entity;
        }

        private void AssignPrimeKey(T entity)
        {

            string tableName = string.Empty;
            IList<PropertyInfo> primeKeys = GetPrimeKeys(entity, out tableName);
            if (null == primeKeys)
                return;
            foreach (var item in primeKeys)
            {
                var itemValue = item.GetValue(entity);
                string sql = string.Format("INSERT INTO `sequence` (`name`) VALUES('{0}') ON DUPLICATE KEY UPDATE `id` = LAST_INSERT_ID(`id` + 1); SELECT LAST_INSERT_ID();", tableName);
                var obj = db.Database.SqlQuery<int>(sql, new string[0]).FirstOrDefault();
                if (itemValue != null)
                {
                    if (string.IsNullOrEmpty(itemValue.ToString()) || itemValue.ToString() == "0")//增加判断当为空值时也生成相应的主键值  add by zhengpsh 2014-12-01
                    {

                        ///INSERT INTO `sequence` (`name`) VALUES('new_business') ON DUPLICATE KEY UPDATE `id` = LAST_INSERT_ID(`id` + 1); SELECT LAST_INSERT_ID();
                        item.SetValue(entity, obj, null);
                        //item.SetValue(entity, Guid.NewGuid().ToString("N"), null);
                    }
                }
                else
                {
                    item.SetValue(entity, obj, null);
                }

            }
        }

        private IList<PropertyInfo> GetPrimeKeys(T entity, out string tableName)
        {
            tableName = string.Empty;
            IList<PropertyInfo> primeKeys = new List<PropertyInfo>();
            foreach (var prop in entity.GetType().GetProperties())
            {
                var attr = prop.GetCustomAttributes(typeof(EntityPrimKey), true).FirstOrDefault() as EntityPrimKey;
                if (attr != null)
                {
                    primeKeys.Add(prop);
                    tableName = attr.TableName;
                    break;
                }
            }
            return primeKeys;
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity"></param>
        public virtual void Update(T entity)
        {
          
            db.Set<T>().Attach(entity);
            CurrentDb.Entry(entity).State = System.Data.Entity.EntityState.Modified;
          
        }

        /// <summary>
        /// 添加或者更新
        /// </summary>
        /// <param name="id"></param>
        /// <param name="entity"></param>
        public virtual void Save(int? id, T entity)
        {

            if (entity != null)
            {

                if (id.HasValue)
                {
                    Update(entity as T);
                }
                else
                {
                    Add(entity as T);
                }
            }
            else
            {
                if (id.HasValue)
                {
                    Update(entity);
                }
                else
                {
                    Add(entity);
                }
            }
        }

        /// <summary>
        /// 标记删除
        /// </summary>
        /// <param name="id"></param>
        public virtual void Delete(int id)
        {
            var item = GetById(id);
            Delete(item);
        }

        /// <summary>
        /// 标记删除
        /// </summary>
        /// <param name="item"></param>
        public virtual void Delete(T item)
        {
            //var entity = item as IDbSetBase;
            //if (entity != null && entity.EnterpriseId.Equals(_userInfo.EnterpriseId))
            //    entity.Deleted = true;
            if (item != null)
            {
                CurrentDb.Entry(item).State = System.Data.Entity.EntityState.Deleted;
            }

        }

        /// <summary>
        /// 标记删除
        /// </summary>
        /// <param name="where"></param>
        public virtual void Delete(Expression<Func<T, bool>> where)
        {
            foreach (var item in GetAll(where))
            {
                Delete(item);
            }
        }

        /// <summary>
        /// 物理删除
        /// </summary>
        /// <param name="item"></param>
        public virtual void Remove(T item)
        {
            db.Set<T>().Remove(item);
        }

        /// <summary>
        /// 获取单个记录
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual T GetById(int id)
        {
            return db.Set<T>().Find(id);
        }



        /// <summary>
        /// 获取用户所在企业数据
        /// </summary>
        /// <returns></returns>
        public virtual IQueryable<T> GetAll(params string[] includes)
        {
            var query = db.Set<T>().AsQueryable<T>().AsNoTracking();
            
            if (includes != null && includes.Count() > 0)
            {
                foreach (var item in includes)
                {
                    query = query.Include(item);
                }
            }
            return query;
        }

        /// <summary>
        /// 获取用户所在企业数据
        /// </summary>
        /// <returns></returns>
        public virtual IQueryable<T> GetAll()
        {           
            var query = db.Set<T>().AsQueryable<T>();
           
            return query;
        }


        /// <summary>
        /// 获取符合条件的用户所在企业数据
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public virtual IQueryable<T> GetAll(Expression<Func<T, bool>> where, params string[] includes)
        {
            IQueryable<T> query = db.Set<T>().Where(where);
            if (includes != null && includes.Count() > 0)
            {
                foreach (var item in includes)
                {
                    query = query.Include(item);
                }
            }
            return query;
        }
        public virtual int ExcuteCommand(string sql, string[] pars)
        {
            return db.Database.ExecuteSqlCommand(sql, pars);
        }


    }
}
