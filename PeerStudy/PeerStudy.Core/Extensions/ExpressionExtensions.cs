using System;
using System.Linq;
using System.Linq.Expressions;

namespace PeerStudy.Core.Extensions
{
    internal static class ExpressionExtensions
    {
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> expression1, Expression<Func<T, bool>> expression2)
        {
            var invocationExpression = Expression.Invoke(expression2, expression1.Parameters.Cast<Expression>());

            return Expression.Lambda<Func<T, bool>>(Expression.AndAlso(expression1.Body, invocationExpression), expression1.Parameters);
        }

        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> expression1, Expression<Func<T, bool>> expression2)
        {
            var invocationExpression = Expression.Invoke(expression2, expression1.Parameters.Cast<Expression>());

            return Expression.Lambda<Func<T, bool>>(Expression.OrElse(expression1.Body, invocationExpression), expression1.Parameters);
        }
    }
}
