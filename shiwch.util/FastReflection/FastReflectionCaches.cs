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
using System.Reflection;

namespace shiwch.util.FastReflection
{
    public static class FastReflectionCaches
    {
        static FastReflectionCaches()
        {
            MethodInvokerCache = new MethodInvokerCache();
            PropertyAccessorCache = new PropertyAccessorCache();
            FieldAccessorCache = new FieldAccessorCache();
            ConstructorInvokerCache = new ConstructorInvokerCache();
        }

        public static IFastReflectionCache<MethodInfo, IMethodInvoker> MethodInvokerCache { get; set; }

        public static IFastReflectionCache<PropertyInfo, IPropertyAccessor> PropertyAccessorCache { get; set; }

        public static IFastReflectionCache<FieldInfo, IFieldAccessor> FieldAccessorCache { get; set; }

        public static IFastReflectionCache<ConstructorInfo, IConstructorInvoker> ConstructorInvokerCache { get; set; }
    }
}
