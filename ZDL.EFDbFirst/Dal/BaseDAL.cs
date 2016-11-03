/*----------------------------------------------------------------
 *  Copyright (C) 2016 天下商机（txooo.com）版权所有
 * 
 *  文 件 名：Class2
 *  所属项目：
 *  创建用户：张德良
 *  创建时间：2016/9/23 星期五 下午 16:20:32
 *  
 *  功能描述：
 *          1、
 *          2、
 * 
 *  修改标识：
 *  修改描述：
 *  待 完 善：
 *          1、 
----------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace ZDL.EFDbFirst.Dal
{
    public abstract class BaseDAL
    {
        public abstract DbContext ctx { get; }


        #region 1.0 新增实体，返回受影响的行数 +  int Add(T model)
        /// <summary>
        /// 1.0 新增实体，返回受影响的行数
        /// </summary>
        /// <param name="model"></param>
        /// <returns>返回受影响的行数</returns>
        public int Add<T>(T model) where T : class, new()
        {
            ctx.Set<T>().Add(model);
            //保存成功后，会将自增的id设置给model的主键属性，并返回受影响的行数。
            return ctx.SaveChanges();
        }
        #endregion
        #region 1.1 新增实体，返回对应的实体对象 + T AddReturnModel(T model)
        /// <summary>
        /// 1.1 新增实体，返回对应的实体对象
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public T AddReturnModel<T>(T model) where T : class, new()
        {
            ctx.Set<T>().Add(model);
            ctx.SaveChanges();
            return model;
        }
        #endregion


        #region 2.0 根据id删除 +  int Del(T model)
        /// <summary>
        /// 2.0 根据id删除
        /// </summary>
        /// <param name="model">必须包含要删除id的对象</param>
        /// <returns></returns>
        public int Del<T>(T model) where T : class, new()
        {
            ctx.Set<T>().Attach(model);
            ctx.Set<T>().Remove(model);
            return ctx.SaveChanges();
        }
        #endregion
        #region 2.1 根据条件删除 + int DelBy(Expression<Func<T, bool>> delWhere)
        /// <summary>
        /// 2.1 根据条件删除
        /// </summary>
        /// <param name="delWhere"></param>
        /// <returns>返回受影响的行数</returns>
        public int DelBy<T>(Expression<Func<T, bool>> delWhere) where T : class, new()
        {
            //2.1.1 查询要删除的数据
            List<T> listDeleting = ctx.Set<T>().Where(delWhere).ToList();
            //2.1.2 将要删除的数据 用删除方法添加到 EF 容器中
            listDeleting.ForEach(u =>
            {
                ctx.Set<T>().Attach(u);  //先附加到EF 容器
                ctx.Set<T>().Remove(u); //标识为删除状态
            });
            //2.1.3 一次性生成sql语句 到数据库执行删除
            return ctx.SaveChanges();
        }
        #endregion


        #region 3.0 修改实体 +  int Modify(T model)
        /// <summary>
        /// 修改实体
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Modify<T>(T model) where T : class, new()
        {
            DbEntityEntry entry = ctx.Entry<T>(model);
            entry.State = EntityState.Modified;
            return ctx.SaveChanges();
        }
        #endregion
        #region 3.1 修改实体，可修改指定属性 + int Modify(T model, params string[] propertyNames)
        /// <summary>
        /// 3.1 修改实体，可修改指定属性
        /// </summary>
        /// <param name="model"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public int Modify<T>(T model, params string[] propertyNames) where T : class, new()
        {
            //3.1.1 将对象添加到EF中
            DbEntityEntry entry = ctx.Entry<T>(model);
            //3.1.2 先设置对象的包装状态为 Unchanged
            entry.State = EntityState.Unchanged;
            //3.1.3 循环被修改的属性名数组
            foreach (string propertyName in propertyNames)
            {
                //将每个被修改的属性的状态设置为已修改状态；这样在后面生成的修改语句时，就只为标识为已修改的属性更新
                entry.Property(propertyName).IsModified = true;
            }
            return ctx.SaveChanges();
        }
        #endregion
        #region 3.2 批量修改 + int ModifyBy(T model, Expression<Func<T, bool>> whereLambda, params string[] modifiedPropertyNames)
        /// <summary>
        /// 3.2 批量修改
        /// </summary>
        /// <param name="model"></param>
        /// <param name="whereLambda"></param>
        /// <param name="modifiedPropertyNames"></param>
        /// <returns></returns>
        public int ModifyBy<T>(T model, Expression<Func<T, bool>> whereLambda, params string[] modifiedPropertyNames) where T : class, new()
        {
            //3.2.1 查询要修改的数据
            List<T> listModifing = ctx.Set<T>().Where(whereLambda).ToList();
            //3.2.2 获取实体类类型对象
            Type t = typeof(T);
            //3.2.3 获取实体类所有的公共属性
            List<PropertyInfo> propertyInfos = t.GetProperties(BindingFlags.Instance | BindingFlags.Public).ToList();
            //3.2.4 创建实体属性字典集合
            Dictionary<string, PropertyInfo> dicPropertys = new Dictionary<string, PropertyInfo>();
            //3.2.5 将实体属性中要修改的属性名 添加到字典集合中  键：属性名  值：属性对象
            propertyInfos.ForEach(p =>
            {
                if (modifiedPropertyNames.Contains(p.Name))
                {
                    dicPropertys.Add(p.Name, p);
                }
            });
            //3.2.6 循环要修改的属性名
            foreach (string propertyName in modifiedPropertyNames)
            {
                //判断要修改的属性名是否在实体类的属性集合中存在
                if (dicPropertys.ContainsKey(propertyName))
                {
                    //如果存在，则取出要修改的属性对象
                    PropertyInfo proInfo = dicPropertys[propertyName];
                    //取出要修改的值
                    object newValue = proInfo.GetValue(model, null);
                    //批量设置要修改对象的属性
                    foreach (T item in listModifing)
                    {
                        //为要修改的对象的要修改的属性设置新的值
                        proInfo.SetValue(item, newValue, null);
                    }
                }
            }
            //一次性生成sql语句 到数据库执行
            return ctx.SaveChanges();
        }
        #endregion


        #region 4.0 根据条件查询单个model + T GetModel(Expression<Func<T, bool>> whereLambda)
        /// <summary>
        /// 4.0 根据条件查询单个model
        /// </summary>
        /// <param name="whereLambda"></param>
        /// <returns></returns>
        public T GetModel<T>(Expression<Func<T, bool>> whereLambda) where T : class, new()
        {
            return ctx.Set<T>().Where(whereLambda).AsNoTracking().FirstOrDefault();
        }
        #endregion
        #region 4.1 根据条件查询单个model并排序  +  T GetModel<TKey>(Expression<Func<T, bool>> whereLambda, Expression<Func<T, TKey>> orderLambda, bool isAsc = true)
        /// <summary>
        /// 4.1 根据条件查询单个model并排序
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="whereLambda"></param>
        /// <param name="orderLambda"></param>
        /// <param name="isAsc"></param>
        /// <returns></returns>
        public T GetModel<T, TKey>(Expression<Func<T, bool>> whereLambda, Expression<Func<T, TKey>> orderLambda, bool isAsc = true) where T : class, new()
        {
            if (isAsc)
            {
                return ctx.Set<T>().Where(whereLambda).OrderBy(orderLambda).AsNoTracking().FirstOrDefault();
            }
            else
            {
                return ctx.Set<T>().Where(whereLambda).OrderByDescending(orderLambda).AsNoTracking().FirstOrDefault();
            }
        }
        #endregion


        #region  5.0 根据条件查询 + List<T> GetListBy(Expression<Func<T, bool>> whereLambda)
        /// <summary>
        /// 5.0 根据条件查询
        /// </summary>
        /// <param name="whereLambda"></param>
        /// <returns></returns>
        public List<T> GetListBy<T>(Expression<Func<T, bool>> whereLambda) where T : class, new()
        {
            return ctx.Set<T>().Where(whereLambda).AsNoTracking().ToList();
        }
        #endregion
        #region 5.1 根据条件查询，并排序 +  List<T> GetListBy<TKey>(Expression<Func<T, bool>> whereLambda, Expression<Func<T, TKey>> orderLambda, bool isAsc = true)
        /// <summary>
        /// 5.1 根据条件查询，并排序
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="whereLambda"></param>
        /// <param name="orderLambda"></param>
        /// <param name="isAsc"></param>
        /// <returns></returns>
        public List<T> GetListBy<T, TKey>(Expression<Func<T, bool>> whereLambda, Expression<Func<T, TKey>> orderLambda, bool isAsc = true) where T : class, new()
        {
            if (isAsc)
            {
                return ctx.Set<T>().Where(whereLambda).OrderBy(orderLambda).AsNoTracking().ToList();
            }
            else
            {
                return ctx.Set<T>().Where(whereLambda).OrderByDescending(orderLambda).AsNoTracking().ToList();
            }
        }
        #endregion
        #region 5.2 根据条件查询Top多少个，并排序 + List<T> GetListBy<TKey>(int top, Expression<Func<T, bool>> whereLambda, Expression<Func<T, TKey>> orderLambda, bool isAsc = true)
        /// <summary>
        /// 5.2 根据条件查询Top多少个，并排序
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="top"></param>
        /// <param name="whereLambda"></param>
        /// <param name="orderLambda"></param>
        /// <param name="isAsc"></param>
        /// <returns></returns>
        public List<T> GetListBy<T, TKey>(int top, Expression<Func<T, bool>> whereLambda, Expression<Func<T, TKey>> orderLambda, bool isAsc = true) where T : class, new()
        {
            if (isAsc)
            {
                return ctx.Set<T>().Where(whereLambda).OrderBy(orderLambda).Take(top).AsNoTracking().ToList();
            }
            else
            {
                return ctx.Set<T>().Where(whereLambda).OrderByDescending(orderLambda).Take(top).AsNoTracking().ToList();
            }
        }
        #endregion
        #region  5.3 根据条件排序查询  双排序 + List<T> GetListBy<TKey1, TKey2>(Expression<Func<T, bool>> whereLambda, Expression<Func<T, TKey1>> orderLambda1, Expression<Func<T, TKey2>> orderLambda2, bool isAsc1 = true, bool isAsc2 = true)
        /// <summary>
        /// 5.3 根据条件排序查询  双排序
        /// </summary>
        /// <typeparam name="TKey1"></typeparam>
        /// <typeparam name="TKey2"></typeparam>
        /// <param name="whereLambda"></param>
        /// <param name="orderLambda1"></param>
        /// <param name="orderLambda2"></param>
        /// <param name="isAsc1"></param>
        /// <param name="isAsc2"></param>
        /// <returns></returns>
        public List<T> GetListBy<T, TKey1, TKey2>(Expression<Func<T, bool>> whereLambda, Expression<Func<T, TKey1>> orderLambda1, Expression<Func<T, TKey2>> orderLambda2, bool isAsc1 = true, bool isAsc2 = true) where T : class, new()
        {
            if (isAsc1)
            {
                if (isAsc2)
                {
                    return ctx.Set<T>().Where(whereLambda).OrderBy(orderLambda1).ThenBy(orderLambda2).AsNoTracking().ToList();
                }
                else
                {
                    return ctx.Set<T>().Where(whereLambda).OrderBy(orderLambda1).ThenByDescending(orderLambda2).AsNoTracking().ToList();
                }
            }
            else
            {
                if (isAsc2)
                {
                    return ctx.Set<T>().Where(whereLambda).OrderByDescending(orderLambda1).ThenBy(orderLambda2).AsNoTracking().ToList();
                }
                else
                {
                    return ctx.Set<T>().Where(whereLambda).OrderByDescending(orderLambda1).ThenByDescending(orderLambda2).AsNoTracking().ToList();
                }
            }
        }
        #endregion
        #region 5.3 根据条件排序查询Top个数  双排序 + List<T> GetListBy<TKey1, TKey2>(int top, Expression<Func<T, bool>> whereLambda, System.Linq.Expressions.Expression<Func<T, TKey1>> orderLambda1, Expression<Func<T, TKey2>> orderLambda2, bool isAsc1 = true, bool isAsc2 = true)
        /// <summary>
        ///  5.3 根据条件排序查询Top个数  双排序
        /// </summary>
        /// <typeparam name="TKey1"></typeparam>
        /// <typeparam name="TKey2"></typeparam>
        /// <param name="top"></param>
        /// <param name="whereLambda"></param>
        /// <param name="orderLambda1"></param>
        /// <param name="orderLambda2"></param>
        /// <param name="isAsc1"></param>
        /// <param name="isAsc2"></param>
        /// <returns></returns>
        public List<T> GetListBy<T, TKey1, TKey2>(int top, Expression<Func<T, bool>> whereLambda, Expression<Func<T, TKey1>> orderLambda1, Expression<Func<T, TKey2>> orderLambda2, bool isAsc1 = true, bool isAsc2 = true) where T : class, new()
        {
            if (isAsc1)
            {
                if (isAsc2)
                {
                    return ctx.Set<T>().Where(whereLambda).OrderBy(orderLambda1).ThenBy(orderLambda2).Take(top).AsNoTracking().ToList();
                }
                else
                {
                    return ctx.Set<T>().Where(whereLambda).OrderBy(orderLambda1).ThenByDescending(orderLambda2).Take(top).AsNoTracking().ToList();
                }
            }
            else
            {
                if (isAsc2)
                {
                    return ctx.Set<T>().Where(whereLambda).OrderByDescending(orderLambda1).ThenBy(orderLambda2).Take(top).AsNoTracking().ToList();
                }
                else
                {
                    return ctx.Set<T>().Where(whereLambda).OrderByDescending(orderLambda1).ThenByDescending(orderLambda2).Take(top).AsNoTracking().ToList();
                }
            }
        }
        #endregion


        #region 6.0 分页查询 + List<T> GetPagedList<TKey>
        /// <summary>
        /// 分页查询 + List<T> GetPagedList
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页容量</param>
        /// <param name="whereLambda">条件 lambda表达式</param>
        /// <param name="orderBy">排序 lambda表达式</param>
        /// <returns></returns>
        public List<T> GetPagedList<T, TKey>(int pageIndex, int pageSize, Expression<Func<T, bool>> whereLambda, Expression<Func<T, TKey>> orderByLambda, bool isAsc = true) where T : class, new()
        {
            // 分页 一定注意： Skip 之前一定要 OrderBy
            if (isAsc)
            {
                return ctx.Set<T>().Where(whereLambda).OrderBy(orderByLambda).Skip((pageIndex - 1) * pageSize).Take(pageSize).AsNoTracking().ToList();
            }
            else
            {
                return ctx.Set<T>().Where(whereLambda).OrderByDescending(orderByLambda).Skip((pageIndex - 1) * pageSize).Take(pageSize).AsNoTracking().ToList();
            }
        }
        #endregion
        #region 6.1分页查询 带输出 +List<T> GetPagedList<TKey>
        /// <summary>
        /// 分页查询 带输出
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="rowCount"></param>
        /// <param name="whereLambda"></param>
        /// <param name="orderBy"></param>
        /// <param name="isAsc"></param>
        /// <returns></returns>
        public List<T> GetPagedList<T, TKey>(int pageIndex, int pageSize, ref int rowCount, Expression<Func<T, bool>> whereLambda, Expression<Func<T, TKey>> orderByLambda, bool isAsc = true) where T : class, new()
        {
            rowCount = ctx.Set<T>().Count(whereLambda);
            if (isAsc)
            {
                return ctx.Set<T>().OrderBy(orderByLambda).Where(whereLambda).Skip((pageIndex - 1) * pageSize).Take(pageSize).AsNoTracking().ToList();
            }
            else
            {
                return ctx.Set<T>().OrderByDescending(orderByLambda).Where(whereLambda).Skip((pageIndex - 1) * pageSize).Take(pageSize).AsNoTracking().ToList();
            }
        }
        #endregion
        #region 6.2 分页查询 带输出 并支持双字段排序
        /// <summary>
        /// 分页查询 带输出 并支持双字段排序
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="rowCount"></param>
        /// <param name="whereLambda"></param>
        /// <param name="orderByLambda1"></param>
        /// <param name="orderByLambda2"></param>
        /// <param name="isAsc1"></param>
        /// <param name="isAsc2"></param>
        /// <returns></returns>
        public List<T> GetPagedList<T, TKey1, TKey2>(int pageIndex, int pageSize, ref int rowCount, Expression<Func<T, bool>> whereLambda, Expression<Func<T, TKey1>> orderByLambda1, Expression<Func<T, TKey2>> orderByLambda2, bool isAsc1 = true, bool isAsc2 = true) where T : class, new()
        {
            rowCount = ctx.Set<T>().Count(whereLambda);
            if (isAsc1)
            {
                if (isAsc2)
                {
                    return ctx.Set<T>().OrderBy(orderByLambda1).ThenBy(orderByLambda2).Where(whereLambda).Skip((pageIndex - 1) * pageSize).Take(pageSize).AsNoTracking().ToList();
                }
                else
                {
                    return ctx.Set<T>().OrderBy(orderByLambda1).ThenByDescending(orderByLambda2).Where(whereLambda).Skip((pageIndex - 1) * pageSize).Take(pageSize).AsNoTracking().ToList();
                }
            }
            else
            {
                if (isAsc2)
                {
                    return ctx.Set<T>().OrderByDescending(orderByLambda1).ThenBy(orderByLambda2).Where(whereLambda).Skip((pageIndex - 1) * pageSize).Take(pageSize).AsNoTracking().ToList();
                }
                else
                {
                    return ctx.Set<T>().OrderByDescending(orderByLambda1).ThenByDescending(orderByLambda2).Where(whereLambda).Skip((pageIndex - 1) * pageSize).Take(pageSize).AsNoTracking().ToList();
                }
            }
        }
        #endregion

        #region Join查询

        public IQueryable<TModel> GetInnerJoin<TOuter, TInner, TModel>(
            Expression<Func<TOuter, dynamic>> outerKeySelector,
            Expression<Func<TInner, dynamic>> innerKeySelector,
            Expression<Func<TOuter, TInner, TModel>> resultSelector) where TInner : class where TOuter : class
        {
            var query = ctx.Set<TOuter>().Join(ctx.Set<TInner>(), outerKeySelector, innerKeySelector, resultSelector);
            return query;
        }

        #endregion
    }
}
