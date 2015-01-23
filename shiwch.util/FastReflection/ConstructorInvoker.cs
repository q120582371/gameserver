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
using System.Linq.Expressions;

namespace shiwch.util.FastReflection
{
    public interface IConstructorInvoker
    {
        object Invoke(params object[] parameters);
    }

    public class ConstructorInvoker : IConstructorInvoker
    {
        private Func<object[], object> m_invoker;

        public ConstructorInfo ConstructorInfo { get; private set; }

        public ConstructorInvoker(ConstructorInfo constructorInfo)
        {
            this.ConstructorInfo = constructorInfo;
            this.m_invoker = InitializeInvoker(constructorInfo);
        }

        private Func<object[], object> InitializeInvoker(ConstructorInfo constructorInfo)
        {
            // Target: (object)new T((T0)parameters[0], (T1)parameters[1], ...)

            // parameters to execute
            var parametersParameter = Expression.Parameter(typeof(object[]), "parameters");

            // build parameter list
            var parameterExpressions = new List<Expression>();
            var paramInfos = constructorInfo.GetParameters();
            for (int i = 0; i < paramInfos.Length; i++)
            {
                // (Ti)parameters[i]
                var valueObj = Expression.ArrayIndex(parametersParameter, Expression.Constant(i));
                var valueCast = Expression.Convert(valueObj, paramInfos[i].ParameterType);

                parameterExpressions.Add(valueCast);
            }

            // new T((T0)parameters[0], (T1)parameters[1], ...)
            var instanceCreate = Expression.New(constructorInfo, parameterExpressions);

            // (object)new T((T0)parameters[0], (T1)parameters[1], ...)
            var instanceCreateCast = Expression.Convert(instanceCreate, typeof(object));

            var lambda = Expression.Lambda<Func<object[], object>>(instanceCreateCast, parametersParameter);

            return lambda.Compile();
        }

        public object Invoke(params object[] parameters)
        {
            return this.m_invoker(parameters);
        }

        #region IConstructorInvoker Members

        object IConstructorInvoker.Invoke(params object[] parameters)
        {
            return this.Invoke(parameters);
        }

        #endregion
    }
}
