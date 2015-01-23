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
    public class MethodInvokerCache : FastReflectionCache<MethodInfo, IMethodInvoker>
    {
        protected override IMethodInvoker Create(MethodInfo key)
        {
            return FastReflectionFactories.MethodInvokerFactory.Create(key);
        }
    }
}
