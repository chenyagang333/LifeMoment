using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chen.ASPNETCore
{
    /*
     AttributeUsage 是一个特性类的特性，它指定了该特性的使用方式和适用范围。
     AttributeTargets.Class 表示该特性可以应用到类上。
     AttributeTargets.Method 表示该特性可以应用到方法上。
     AllowMultiple = false 意味着特性在同一个类或方法上只能应用一次，不允许多次应用。
     Inherited = true 表示该特性可被子类继承。
     */
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    // 声明一个自定义特性 UnitOfWorkAttribute，他派生自 Attribute 类。
    public class UnitOfWorkAttribute : Attribute
    {
        // 构造参数，用于初始化 UnitOfWorkAttribute 类的新实例。
        // params 关键字允许将一个可变量的参数传递给构造函数，这些参数将会被作为数组来处理。
        // dbContextTypes 参数是 Type 类型的一个数组，用于接受被传递的 DbContext 类型数组。
        public UnitOfWorkAttribute(params Type[] dbContextTypes)
        {
            // 使用 init 只读初始化器初始化 DbContextTypes 属性。
            DbContextTypes = dbContextTypes;

            // 遍历传递进来的 Type 数组
            foreach (Type dbContextType in dbContextTypes)
            {
                // 验证每个传递的类型是否为 DbContext 类型或其子类
                if (!typeof(DbContext).IsAssignableFrom(dbContextType))
                {
                    // 如果不是 DbContext 类型或其子类，则抛出 ArgumentException 异常
                    throw new ArgumentException($"{dbContextType} must inherit from DbContext");
                }
            }
        }

        // 属性声明，用于存储传递进来的 DbContext 类型数组。
        public Type[] DbContextTypes { get; init; }
    }
}
