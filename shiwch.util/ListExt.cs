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
using System.Data;
using System.Reflection;
using shiwch.util.FastMember;

namespace shiwch.util
{
    /// <summary>
    /// IList&lt;T&gt;的扩展方法
    /// </summary>
    public static class ListExt
    {
        /// <summary>
        /// 将列表对象转成DataTable对象
        /// </summary>
        /// <typeparam name="T">列表成员对象类型</typeparam>
        /// <param name="list">要转换的列表</param>
        /// <exception cref="ArgumentNullException">list为空</exception>
        /// <returns></returns>
        public static DataTable ToDataTable<T>(this IList<T> list)
        {
            Check.NotNull(list, "list");

            DataTable dt = new DataTable();
            PropertyInfo[] propertys = typeof(T).GetProperties();
            foreach (PropertyInfo pi in propertys)
            {
                dt.Columns.Add(pi.Name, pi.PropertyType);
            }
            TypeAccessor warp = TypeAccessor.Create(typeof(T));
            for (int i = 0; i < list.Count; i++)
            {
                List<object> tempList = new List<object>(propertys.Length);
                foreach (PropertyInfo pi in propertys)
                {
                    tempList.Add(warp[list[i], pi.Name]);
                }
                object[] array = tempList.ToArray();
                dt.LoadDataRow(array, true);
            }

            return dt;
        }
    }
}
