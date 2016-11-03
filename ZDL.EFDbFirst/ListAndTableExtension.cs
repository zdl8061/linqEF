/*----------------------------------------------------------------
 *  Copyright (C) 2016 天下商机（txooo.com）版权所有
 * 
 *  文 件 名：Class1
 *  所属项目：
 *  创建用户：张德良
 *  创建时间：2016/10/28 星期五 下午 13:31:27
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
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net;
using System.IO;
using System.Text;
using System.Collections;
using System.Text.RegularExpressions;
using Match = System.Text.RegularExpressions.Match;
using Microsoft.CSharp;

namespace ZDL.EFDbFirst
{
    public static class ListAndTableExtension
    {
        #region 集合转为对应的DataTable

        /// <summary>
        ///List转换为DataTable
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static DataTable ToDataTable<T>(this List<T> list) where T : new()
        {
            DataTable table = new DataTable();
            PropertyInfo[] ps = typeof(T).GetProperties();
            foreach (PropertyInfo p in ps)
            {
                if (!p.PropertyType.IsGenericType)
                {
                    table.Columns.Add(ConvertToTableColumnName(p.Name), p.PropertyType);

                }
                else
                {
                    Type GenericTypeDefinition = p.PropertyType.GetGenericTypeDefinition();
                    if (GenericTypeDefinition == typeof(Nullable<>))
                    {
                        table.Columns.Add(ConvertToTableColumnName(p.Name), Nullable.GetUnderlyingType(p.PropertyType));

                    }
                }
            }
            foreach (T obj in list)
            {
                DataRow row = table.NewRow();
                foreach (PropertyInfo p in ps)
                {

                    row[ConvertToTableColumnName(p.Name)] = p.GetValue(obj, null);


                }
                table.Rows.Add(row);

            }
            return table;

        }

        #endregion

        #region 扩展方法 DataTable转为对应实体的List集合

        public static List<T> ToList<T>(this DataTable table) where T : new()
        {
            List<T> list = new List<T>();
            PropertyInfo[] ps = typeof(T).GetProperties();
            foreach (DataRow row in table.Rows)
            {
                T obj = new T();
                foreach (DataColumn col in table.Columns)
                {
                    foreach (PropertyInfo p in ps)
                    {
                        if (p.Name == ConvertToEntityColumnName(col.ColumnName))
                        {
                            if (!p.PropertyType.IsGenericType)
                            {
                                p.SetValue(obj,
                                string.IsNullOrEmpty(row[col.ColumnName].ToString())
                                ? null
                                : Convert.ChangeType(row[col.ColumnName].ToString(), p.PropertyType), null);

                            }
                            else
                            {
                                if (p.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                                {
                                    p.SetValue(obj,
                                    string.IsNullOrEmpty(row[col.ColumnName].ToString())
                                    ? null
                                    : Convert.ChangeType(row[col.ColumnName],
                                    Nullable.GetUnderlyingType(p.PropertyType)), null);

                                }
                            }

                        }
                    }

                }
                list.Add(obj);
            }
            return list;
        }

        #endregion

        #region 列名转为对应实体类的属性名

        public static string ConvertToEntityColumnName(string name)
        {

            List<string> strList = name.Split('_').ToList();
            StringBuilder sb = new StringBuilder();
            foreach (string s2 in strList)
            {
                sb.Append(ReplaceString(s2));
            }
            return sb.ToString();
        }

        public static string ReplaceString(string s)
        {
            return Regex.Replace(s, (string)@"([A-Za-z]{1})([A-Za-z]*)", (MatchEvaluator)MathcEval);


        }

        private static string MathcEval(Match match)
        {
            return match.Groups[1].Value.ToUpper() + match.Groups[2].Value.ToLower();
        }

        #endregion
        public static IList ToGenericList(this DataTable dataTable)
        {
            Type GenericType = ListAndTableExtension.InitEntityType(dataTable);
            Type typeMaster = typeof(List<>);
            Type listType = typeMaster.MakeGenericType(GenericType);
            IList list = Activator.CreateInstance(listType) as IList;
            if (dataTable == null || dataTable.Rows.Count == 0)
                return list;
            var constructor = GenericType.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
            .OrderBy(c => c.GetParameters().Length).First();
            var parameters = constructor.GetParameters();
            var values = new object[parameters.Length];
            foreach (DataRow dr in dataTable.Rows)
            {
                int index = 0;
                foreach (ParameterInfo item in parameters)
                {
                    object itemValue = null;
                    if (dr[item.Name] != null && dr[item.Name] != DBNull.Value)
                    {
                        itemValue = Convert.ChangeType(dr[item.Name], item.ParameterType);
                    }
                    values[index++] = itemValue;
                }

                list.Add(constructor.Invoke(values));

            }
            return list;
        }
        #region 属性名转换为数据库对应的列名

        //属性名转换为数据库对应的列名
        public static string ConvertToTableColumnName(string name)
        {
            name = Regex.Replace(name, @"([A-Z]{1})([a-z]*)", MatchEval);
            name = name.TrimEnd('_');
            return name;
        }

        private static string MatchEval(Match match)
        {
            return match.Groups[1].Value.ToUpper() + match.Groups[2].Value.ToUpper() + "_";
        }

        private static Type InitEntityType(DataTable table)
        {
            CSharpCodeProvider p = new CSharpCodeProvider();
            CompilerParameters param = new CompilerParameters();
            string s = "namespace __ns" +
            "{" +
            "public class AAA";
            //"{ public AAA(string Name,int ID,string GuidType){this.Name=Name;this.ID=ID;this.GuidType=GuidType;}" +
            //" public string Name{ get;set; }" +
            //" public int ID{get;set;}" +
            //" public string GuidType{get;set;}" +
            s += "{ " +
            "public AAA(";
            for (int i = 0; i < table.Columns.Count; i++)
            {

                s += "string " + table.Columns[i].ColumnName + ",";

            }
            s = s.TrimEnd(',') + "){";
            for (int i = 0; i < table.Columns.Count; i++)
            {
                s += "this." + table.Columns[i].ColumnName + "=" + table.Columns[i].ColumnName + ";";
            }
            s += "}";
            for (int i = 0; i < table.Columns.Count; i++)
            {
                string columnName = table.Columns[i].ColumnName;
                s += " public string " + columnName + " {get;set;}\r\n";
            }
            s += " }}\r\n";

            List<object> objs = new List<object>();
            for (int i = 0; i < table.Columns.Count; i++)
            {
                objs.Add("");
            }

            CompilerResults rel = p.CompileAssemblyFromSource(param, s);
            Type t = rel.CompiledAssembly.GetType("__ns.AAA");

            object o = t.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
            .OrderBy(c => c.GetParameters().Length).First().Invoke(objs.ToArray());

            return o.GetType();
        }

        public static void TestMethod()
        {
            var entity = new { Name = "item", ID = 0, GuidType = "" };


            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("Name", typeof(string));
            dataTable.Columns.Add("ID", typeof(int));
            dataTable.Columns.Add("GuidType", typeof(string));

            for (int i = 0; i < 10; i++)
            {
                DataRow dr = dataTable.NewRow();
                dr["Name"] = "STRING" + i;
                dr["ID"] = i;
                if (i % 2 == 0)
                    dr["GuidType"] = "GuidType" + i;
                else
                {
                    dr["GuidType"] = DBNull.Value;
                }
                dataTable.Rows.Add(dr);
            }

            IList list = dataTable.ToGenericList();
        }

        #endregion

    }
}