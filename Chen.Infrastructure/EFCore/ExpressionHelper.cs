using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using static System.Linq.Expressions.Expression;
using System.Text;
using System.Threading.Tasks;

namespace Chen.Infrastructure.EFCore;
public class ExpressionHelper
{
    /// <summary>
    /// Users.SingleOrDefaultAsync(MakeEqual((User u) => u.PhoneNumber, phoneNumber))
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    /// <typeparam name="TProp"></typeparam>
    /// <param name="propAccessor"></param>
    /// <param name="other"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static Expression<Func<TItem,bool>> MakeEqual<TItem,TProp>(Expression<Func<TItem,TProp>> propAccessor,TProp? other)
        where TItem : class
        where TProp : class
    {
        // 提取出来参数
        var el = propAccessor.Parameters.Single();
        
        // 初始化条件表达式
        BinaryExpression? conditionalExpr = null;

        // 遍历其他对象属性
        foreach (var prop in typeof(TProp).GetProperties())
        {
            BinaryExpression equalExpr;

            // other 的 prop 属性的值
            object? otherValue = null;
            if (other != null)
            {
                otherValue = prop.GetValue(other);
            }

            // 获取属性的类型
            Type propType = prop.PropertyType;

            // 获取属性的访问表达式
            var leftExpr = MakeMemberAccess(propAccessor.Body, prop);

            // 将 otherValue 转化为与属性类型相同的表达式
            Expression rightExpr = Convert(Constant(otherValue), propType);

            // 基本数据类型和复杂类型比较方法不一样
            if (propType.IsPrimitive) 
            {
                // 对于基本数据类型，使用 Equal 方法创建相等比较表达式 
                equalExpr = Equal(leftExpr, rightExpr);
            }
            else
            {
                // 对于复杂类型，使用 MakeBinary 方法创建相等比较表达式
                equalExpr = MakeBinary(ExpressionType.Equal,
                    leftExpr, rightExpr, false,
                    prop.PropertyType.GetMethod("op_Equality"));
            }
            if (conditionalExpr == null)
            {
                conditionalExpr = equalExpr;
            }
            else
            {
                conditionalExpr = AndAlso(conditionalExpr, equalExpr);
            }
        }
        if (conditionalExpr == null)
        {
            throw new ArgumentException("There should be at least one property.");
        }
        return Lambda<Func<TItem, bool>>(conditionalExpr,el);
    }
}
