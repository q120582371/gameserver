// -------------------------------------------------------
// Copyright (C) 施维串 版权所有。
// 创建标识：2013-11-11 10:53:36 Created by 施维串
// 功能说明：
// 注意事项：
// 
// 更新记录：
// -------------------------------------------------------


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using shiwch.util.FastMember;
using System.IO;

namespace shiwch.util
{
    /// <summary>
    /// DataTable助手类
    /// </summary>
    public static class DataTableExt
    {
        /// <summary>
        /// 将DataTable转成对象列表
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="dt">要转换的DataTable</param>
        /// <exception cref="ArgumentNullException">dt为空</exception>
        /// <returns>如果行数=0，那么返回长度为0的列表</returns>
        public static IList<T> ToObjectList<T>(this DataTable dt) where T : new()
        {
            Check.NotNull(dt, "dt");

            List<T> list = new List<T>();
            TypeAccessor warp = TypeAccessor.Create(typeof(T));
            foreach (DataRow row in dt.Rows)
            {
                T obj = new T();
                foreach (DataColumn col in dt.Columns)
                {
                    warp[obj, col.ColumnName] = row[col];
                }

                list.Add(obj);
            }
            return list;
        }

        /// <summary>
        /// 将DataTable转成对象列表
        /// </summary>
        /// <param name="dt">要转换的DataTable</param>
        /// <param name="type">对象类型</param>
        /// <exception cref="ArgumentNullException">dt为空</exception>
        /// <exception cref="ArgumentNullException">type为空</exception>
        /// <returns>如果行数=0，那么返回长度为0的列表</returns>
        public static IList<object> ToObjectList(this DataTable dt, Type type)
        {
            Check.NotNull(dt, "dt");
            Check.NotNull(type, "type");

            List<object> list = new List<object>();
            TypeAccessor warp = TypeAccessor.Create(type);
            foreach (DataRow row in dt.Rows)
            {
                var obj = FastActivator.Create(type);
                foreach (DataColumn col in dt.Columns)
                {
                    warp[obj, col.ColumnName] = row[col];
                }

                list.Add(obj);
            }
            return list;
        }

        /// <summary>
        /// 将DataTable的第一行转成对象
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="dt">要转换的DataTable</param>
        /// <exception cref="ArgumentNullException">dt为空</exception>
        /// <returns>如果行数=0，那么返回null</returns>
        public static T ToObject<T>(this DataTable dt) where T : new()
        {
            Check.NotNull(dt, "dt");

            if (dt.Rows.Count == 0) return default(T);
            return ToObject<T>(dt.Rows[0]);
        }

        /// <summary>
        /// 将DataTable的第一行转成对象
        /// </summary>
        /// <param name="dt">要转换的DataTable</param>
        /// <param name="type">对象类型</param>
        /// <exception cref="ArgumentNullException">dt为空</exception>
        /// <exception cref="ArgumentNullException">type为空</exception>
        /// <returns>如果行数=0，那么返回null</returns>
        public static object ToObject(this DataTable dt, Type type)
        {
            Check.NotNull(dt, "dt");
            Check.NotNull(type, "type");

            if (dt.Rows.Count == 0) return null;
            return ToObject(dt.Rows[0], type);
        }

        /// <summary>
        /// 将DataRow对象转成对象
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="row">要转换的DataRow</param>
        /// <exception cref="ArgumentNullException">row为空</exception>
        /// <returns></returns>
        public static T ToObject<T>(this DataRow row) where T : new()
        {
            Check.NotNull(row, "row");

            T result = new T();
            ObjectAccessor warp = ObjectAccessor.Create(result);
            foreach (DataColumn col in row.Table.Columns)
            {
                warp[col.ColumnName] = row[col];
            }

            return result;
        }

        /// <summary>
        /// 将DataRow对象转成对象
        /// </summary>
        /// <param name="row">要转换的DataRow</param>
        /// <param name="type">对象类型</param>
        /// <exception cref="ArgumentNullException">row为空</exception>
        /// <exception cref="ArgumentNullException">type为空</exception>
        /// <returns></returns>
        public static object ToObject(this DataRow row, Type type)
        {
            Check.NotNull(row, "row");
            Check.NotNull(type, "type");

            object result = FastActivator.Create(type);
            ObjectAccessor warp = ObjectAccessor.Create(result);
            foreach (DataColumn col in row.Table.Columns)
            {
                warp[col.ColumnName] = row[col];
            }

            return result;
        }

        /// <summary>
        /// 将表格数据导出为csv格式文件存储
        /// </summary>
        /// <param name="dt">要转换的DataTable对象</param>
        /// <exception cref="ArgumentNullException">dt为空</exception>
        /// <returns></returns>
        public static string ToCSV(this DataTable dt)
        {
            Check.NotNull(dt, "dt");

            var result = new StringBuilder();
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                result.Append(dt.Columns[i].ColumnName);
                result.Append(i == dt.Columns.Count - 1 ? Environment.NewLine : ",");
            }

            foreach (DataRow row in dt.Rows)
            {
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    var txt = row[i].ToString();
                    if (txt.IndexOfAny(new char[] { '"', ',' }) != -1) result.AppendFormat("\"{0}\"", txt.Replace("\"", "\"\""));
                    else result.Append(row[i].ToString());
                    result.Append(i == dt.Columns.Count - 1 ? Environment.NewLine : ",");
                }
            }

            return result.ToString();
        }
    }
}
