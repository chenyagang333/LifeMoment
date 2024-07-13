using Chen.DomainCommons.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace MediatR
{
    public static class MediatorExtensions
    {
        public static IServiceCollection AddMediatR(this IServiceCollection services)
        {
            return default;
                //services.AddMediatR(cfg => {
                //cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
                //cfg.AddBehavior<PingPongBehavior>();
                //cfg.AddStreamBehavior<PingPongStreamBehavior>();
                //cfg.AddRequestPreProcessor<PingPreProcessor>();
                //cfg.AddRequestPostProcessor<PingPongPostProcessor>();
                //cfg.AddOpenBehavior(typeof(GenericBehavior<,>));
            //});
        }

        /// <summary>
        /// 发送领域事件
        /// </summary>
        /// <param name="mediator"></param>
        /// <param name="ctx"></param>
        /// <returns></returns>
        public static async Task DispatchDomainEventsAsync(this IMediator mediator,DbContext ctx)
        {
            var domainEntities = ctx.ChangeTracker
                .Entries<IDomainEvents>()
                .Where(x => x.Entity.GetDomainEvents().Any());

            var domainEvents = domainEntities
                .SelectMany(x => x.Entity.GetDomainEvents())
                .ToList(); // 加 ToList() 是为了立即加载，否则会延迟执行，到 Foreach 的时候已经被 ClearDomainEvents 了

            domainEntities.ToList().ForEach(e => e.Entity.ClearDomainEvents());

            foreach (var domainEvent in domainEvents)
            {
                await mediator.Publish(domainEvent);
            }
        }
    }
}
