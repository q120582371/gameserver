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
using System.Linq.Expressions;

namespace shiwch.util
{
    /// <summary>
    /// <para>Implements a very fast object factory for dynamic object creation. Dynamically generates a factory class which will use the new() constructor of the requested type.</para>
    /// <para>Much faster than using Activator at the price of the first invocation being significantly slower than subsequent calls.</para>
    /// </summary>
    public static class FastActivator
    {
        private static readonly Dictionary<Type, Func<object>> factoryCache = new Dictionary<Type, Func<object>>();

        /// <summary>
        /// Creates an instance of the specified type using a generated factory to avoid using Reflection.
        /// </summary>
        /// <typeparam Name="T">The type to be created.</typeparam>
        /// <returns>The newly created instance.</returns>
        public static T Create<T>()
        {
            return TypeFactory<T>.Create();
        }

        /// <summary>
        /// Creates an instance of the specified type using a generated factory to avoid using Reflection.
        /// </summary>
        /// <param Name="type">The type to be created.</param>
        /// <exception cref="ArgumentNullException">type is null</exception>
        /// <returns>The newly created instance.</returns>
        public static object Create(Type type)
        {
            Check.NotNull(type, "type");

            Func<object> f;

            if (!factoryCache.TryGetValue(type, out f))
                lock (factoryCache)
                    if (!factoryCache.TryGetValue(type, out f))
                    {
                        factoryCache[type] = f = Expression.Lambda<Func<object>>(Expression.New(type)).Compile();
                    }

            return f();
        }

        private static class TypeFactory<T>
        {
            public static readonly Func<T> Create = Expression.Lambda<Func<T>>(Expression.New(typeof(T))).Compile();
        }
    }
}
