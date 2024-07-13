using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chen.DomainCommons.Models
{
    public record AggregateRootEntity : BaseEntity, IAggregateRoot, ISoftDelete, IHasCreateTime, IHasDeleteTime, IHasModificationTime
    {
        public bool IsDeleted { get;  set; }

        public DateTime CreateTime { get;  set; } = DateTime.Now;

        public DateTime? DeletionTime { get;  set; }

        public DateTime? LastModificationTime { get;  set; }

        public void SoftDelete()
        {
            IsDeleted = true;
            DeletionTime = DateTime.Now;
        }

        public void NotifyModified()
        {
            LastModificationTime = DateTime.Now;
        }
    }
}
