using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.Transactions;

namespace Chen.ASPNETCore
{
    public class UnitOfWorkFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // 获取当 Action 或 Controller 上的UnitOfWorkAttribute
            var unitOfWorkAttribute = GetUnitOfWorkAttribute(context.ActionDescriptor);
            // 如果没有则执行下一个 Action
            if (unitOfWorkAttribute == null)
            {
                await next();
                return;
            }
            // 创建事务范围对象，启动异步流转。
            using TransactionScope transactionScope = new(TransactionScopeAsyncFlowOption.Enabled);
            // 用于储存 DbContext 实例的列表。
            List<DbContext> dbContexts = new List<DbContext>();
            // 遍历 unitOfWorkAttribute 中的 DbContextTypes
            foreach (var dbContextType in unitOfWorkAttribute.DbContextTypes)
            {
                // 通过 HttpContext 的 RequestServices 获取服务提供程序实例
                // 确保获取的是与当前请求相关的 Scope 实例
                var sp = context.HttpContext.RequestServices;
                // 从服务程序中获取指定类型的 DbContext 实例
                DbContext dbContext = (DbContext)sp.GetRequiredService(dbContextType);
                // 将 dnContext 添加到列表中
                dbContexts.Add(dbContext);
            }
            // 执行下一个 Action
            var result = await next();
            // 如果执行结果没有异常，则保存更改完成事务。
            if (result.Exception == null)
            {
                foreach (var dbContext in dbContexts)
                {
                    await dbContext.SaveChangesAsync();
                }
                transactionScope.Complete(); // 通知事务管理器可以接受提交事务
            }
        }
        private static UnitOfWorkAttribute? GetUnitOfWorkAttribute(ActionDescriptor actionDescriptor)
        {
            var controllerActionDescriptor = actionDescriptor as ControllerActionDescriptor;
            if (controllerActionDescriptor == null)
            {
                return null;
            }
            // try to get UnitOfWorkAttribute from controller
            // if there is no UnitOrWorkAttribute on controller
            // try to get UnitOrWorkAttribute from action
            return controllerActionDescriptor.ControllerTypeInfo.GetCustomAttribute<UnitOfWorkAttribute>() ??
                controllerActionDescriptor.MethodInfo.GetCustomAttribute<UnitOfWorkAttribute>();
        }
    }
}
