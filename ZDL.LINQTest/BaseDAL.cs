//using System;
//using System.Collections.Generic;
//using System.Data.Linq;
//using System.Linq;
//using System.Linq.Expressions;
//using System.Text;
//using System.Threading.Tasks;
//using Txooo.Data;

//namespace ZDL.LINQTest
//{
//    class BaseDAL<T> where T : class
//    {
//        static IDictionary<string, TxDataContext> dbContext = new Dictionary<string, TxDataContext>(StringComparer.OrdinalIgnoreCase);

//        public static TxDataContext GetContext(string dbName)
//        {
//            if (!dbContext.ContainsKey(dbName))
//            {
//                var dbInfo = DatabaseConfig.Instance.GetDatabaseInfoByCache(dbName);
//                var dataContext = new TxDataContext(dbInfo.ConnString);
//                dbContext[dbName] = dataContext;
//            }
//            return dbContext[dbName];
//        }

//        public static IQueryable<T> Where(string dbNode, Expression<Func<T,bool>> lambda)
//        {
//            var db = GetContext(dbNode);

//            var query = db.GetTable<T>().Where(lambda);

//            return query;
//        }

     
//    }

//    public class TxDataContext : DataContext
//    {     
//        public TxDataContext(string conn) : base(conn) { }
//    }
//}
