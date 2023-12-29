﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Azure.Messageing.ServiceBus.Invoker.Helpers
{
    internal static class ExpressionHelpers
    {
        public static string GetMethodName<TClass>(Expression<Action<TClass>> expression)
        {
            return ((MethodCallExpression)expression.Body).Method.Name;
        }

        public static string GetCallers<TClass>(Expression<Action<TClass>> expression)
        {
            var methodName = GetMethodName(expression);
            return expression.Body.ToString().TrimOnKeyword(methodName).RemoveFirstCaller();
        }

        public static KeyValuePair<Type, object>[] ResolveArgs(LambdaExpression expression)
        {
            var body = (MethodCallExpression)expression.Body;
            var values = new List<KeyValuePair<Type, object>>();

            var parameterExpressions = body.Arguments;

            foreach (var parameterExpression in parameterExpressions)
            {
                var paramValueAccessor = Expression.Lambda(parameterExpression);
                var paramValue = paramValueAccessor.Compile().DynamicInvoke();

                values.Add(new KeyValuePair<Type, object>(paramValue?.GetType(), paramValue));
            }

            return values.ToArray();
        }

        public static MemberExpression ResolveMemberExpression(Expression expression)
        {

            if (expression is MemberExpression)
            {
                return (MemberExpression)expression;
            }
            else if (expression is UnaryExpression)
            {
                return (MemberExpression)((UnaryExpression)expression).Operand;
            }
            else
            {
                return null;
            }
        }
    }
}