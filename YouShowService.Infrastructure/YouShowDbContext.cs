using Chen.Infrastructure.EFCore;
using MediatR;
using Microsoft.EntityFrameworkCore;
using YouShowService.Domain.Entities;

namespace YouShowService.Infrastructure
{
    public class YouShowDbContext : BaseDbContext
    {
        public DbSet<YouShow> YouShows { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Reply> Replys { get; set; }
        public DbSet<CommentLikeUser> CommentLikeUsers { get; set; }
        public DbSet<ReplyLikeUser> ReplyLikeUsers { get; set; }
        public DbSet<YouShowLikeUser> YouShowLikeUsers { get; set; }
        public DbSet<YouShowStarUser> YouShowStarUsers { get; set; }
        public DbSet<YouShowFile> YouShowFiles { get; set; }

        public YouShowDbContext(DbContextOptions options, IMediator mediator) : base(options, mediator)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(GetType().Assembly);
            builder.EnableSoftDeletionGlobalFilter(); // 添加软删除的过滤条件
        }
    }
}
