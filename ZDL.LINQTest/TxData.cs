using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ZDL.LINQTest
{
    /// <summary>
    /// 数据交互封装测试
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TxData<T> where T : class,new()
    {
        #region 基本用法
        public void Test()
        {
            var stateInfo = new StateInfo { Mname = "asdf", Fname = "owieu", AddTime = DateTime.Now, UserId = 21 };
            TxData<StateInfo>.Insert(stateInfo);

            TxData<StateInfo>.Delete("WHERE f_name=@f_name", "二分法");

            TxData<StateInfo>.Update("f_name=@f_name WHERE m_name=@m_name", "测测测测", "违法");

            var list = TxData<StateInfo>.Query("WHERE f_name=@f_name order by user_id desc", paramValue: new object[] { "owieu" });
        }
        #endregion

        static Hashtable tableInfo = new Hashtable();

        static TxTable GetTableInfo()
        {
            Type type = typeof(T);
            string typeName = type.ToString();

            var table = tableInfo[typeName] as TxTable;
            if (table == null)
            {
                var attributes = type.GetCustomAttributes(false);
                table = attributes[0] as TxTable;
                if (table != null) tableInfo[typeName] = table;
            }
            return table;
        }

        public static int Insert(params T[] array)
        {
            var table = GetTableInfo();

            if (table != null)
            {
                var proper = typeof(T).GetProperties();
                string _sql = "INSERT INTO [dbo].[" + table.Name + "]({0}) VALUES({1});";

                Hashtable paramData = new Hashtable();
                var fieldList = new List<string>();

                using (Txooo.TxDataHelper helper = Txooo.TxDataHelper.GetDataHelper(table.Base))
                {
                    foreach (var entity in array)
                    {
                        foreach (var item in proper)
                        {
                            if (item.CanRead)
                            {
                                var propAttr = item.GetCustomAttributes(true);
                                var columnInfo = propAttr[0] as ColumnAttribute;
                                if (!columnInfo.IsDbGenerated)
                                {
                                    string pName = "@" + columnInfo.Name;
                                    paramData[pName] = item.GetValue(entity);
                                    fieldList.Add(pName);
                                }
                            }
                        }
                        _sql = string.Format(_sql,
                            string.Join(",", fieldList).Replace("@", string.Empty),
                            string.Join(",", fieldList));

                        return helper.SqlExecute(_sql, paramData);
                    }
                }
            }
            return 0;
        }

        public static int Update(string fieldsWhere, params object[] paramValue)
        {
            var table = GetTableInfo();

            if (table != null)
            {
                string _sql = string.Format("UPDATE [dbo].["+table.Name+"] SET {0} ", fieldsWhere);

                Hashtable param = new Hashtable();
                //Regex paramReg = new Regex(@"[^@@](?<p>@\w+)"); 
                Regex paramReg = new Regex(@"@\w*");
                MatchCollection matches = paramReg.Matches(_sql);
                for (var i = 0; i < matches.Count; i++)
                {
                    var key = matches[i].Value;
                    param[key] = paramValue[i];
                }
                using (Txooo.TxDataHelper helper = Txooo.TxDataHelper.GetDataHelper(table.Base))
                {
                    return helper.SqlExecute(_sql, param);
                }
            }
            return 0;
        }

        public static int Delete(string sqlWhere, params object[] paramValue)
        {
            var table = GetTableInfo();

            if (table != null)
            {
                string _sql = "DELETE FROM [dbo].[" + table.Name + "] " + sqlWhere;

                Hashtable param = new Hashtable();
                //Regex paramReg = new Regex(@"[^@@](?<p>@\w+)"); 
                Regex paramReg = new Regex(@"@\w*");
                MatchCollection matches = paramReg.Matches(_sql);
                for (var i = 0; i < matches.Count; i++)
                {
                    var key = matches[i].Value;
                    param[key] = paramValue[i];
                }

                using (Txooo.TxDataHelper helper = Txooo.TxDataHelper.GetDataHelper(table.Base))
                {
                    return helper.SqlExecute(_sql, param);
                }
            }
            return 0;
        }

        public static IList<T> Query(string sqlWhere, int pageSize = 20, int currentPage = 1, object[] paramValue = null)
        {
            var table = GetTableInfo();

            if (table != null)
            {
                if (sqlWhere.ToLower().IndexOf("order by") == -1)
                {
                    throw new Exception("sqlWhere参数必须包含order by");
                }
                string orderby = sqlWhere.Substring(sqlWhere.ToLower().IndexOf("order"));

                int currentRow = (currentPage - 1) * pageSize;

                string _sql = string.Format("SELECT * FROM (SELECT ROW_NUMBER() OVER ({0}) AS RowNum, * FROM {1} {2}) AS T WHERE RowNum BETWEEN {3} AND {4}",
                    orderby, table.Name, sqlWhere.Replace(orderby, string.Empty), currentRow + 1, currentRow + pageSize);

                Hashtable param = new Hashtable();
                //Regex paramReg = new Regex(@"[^@@](?<p>@\w+)"); 
                Regex paramReg = new Regex(@"@\w*");
                MatchCollection matches = paramReg.Matches(_sql);
                for (var i = 0; i < matches.Count; i++)
                {
                    var key = matches[i].Value;
                    param[key] = paramValue[i];
                }

                DataTable data;
                using (Txooo.TxDataHelper helper = Txooo.TxDataHelper.GetDataHelper(table.Base))
                {
                    data = helper.SqlGetDataTable(_sql, param);
                }

                if (data.Rows.Count > 0)
                {
                    return GetEntityList(data);
                }
            }
            return null;
        }


        private static IList<T> GetEntityList(DataTable dataTable)
        {
            IList<T> enties = new List<T>();
            T model;
            var properties = typeof(T).GetProperties();
            foreach (DataRow row in dataTable.Rows)
            {
                model = new T();
                foreach (var item in properties)
                {
                    var propAttr = item.GetCustomAttributes(true);
                    var columnInfo = propAttr[0] as ColumnAttribute;

                    if (row.Table.Columns.Contains(columnInfo.Name))
                    {
                        if (DBNull.Value != row[columnInfo.Name])
                        {
                            item.SetValue(model, Convert.ChangeType(row[columnInfo.Name], item.PropertyType), null);
                        }

                    }
                }
                enties.Add(model);
            }
            return enties;
        }
    }


    public class TxTable : Attribute
    {
        public TxTable() { }

        /// <summary>
        /// 数据库表名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 数据库连接配置节点名称
        /// </summary>
        public string Base { get; set; }
    }


    /// <summary>
    /// 实体映射规则
    /// </summary>
    [TxTable(Name = "StateInfo", Base = "TxoooMy")]
    public class StateInfo
    {
        [Column(Name = "user_id", IsPrimaryKey = true, IsDbGenerated = true)]
        public int UserId { get; set; }

        [Column(Name = "f_name")]
        public string Fname { get; set; }

        [Column(Name = "m_name")]
        public string Mname { get; set; }

        [Column(Name = "addtime", IsDbGenerated = true)]
        public DateTime AddTime { get; set; }
    }
}