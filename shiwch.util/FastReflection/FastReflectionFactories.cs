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
    public static class FastReflectionFactories
    {
        static FastReflectionFactories()
        {
            MethodInvokerFactory = new MethodInvokerFactory();
            PropertyAccessorFactory = new PropertyAccessorFactory();
            FieldAccessorFactory = new FieldAccessorFactory();
            ConstructorInvokerFactory = new ConstructorInvokerFactory();
        }

        public static IFastReflectionFactory<MethodInfo, IMethodInvoker> MethodInvokerFactory { get; set; }

        public static IFastReflectionFactory<PropertyInfo, IPropertyAccessor> PropertyAccessorFactory { get; set; }

        public static IFastReflectionFactory<FieldInfo, IFieldAccessor> FieldAccessorFactory { get; set; }

        public static IFastReflectionFactory<ConstructorInfo, IConstructorInvoker> ConstructorInvokerFactory { get; set; }
    }
}
