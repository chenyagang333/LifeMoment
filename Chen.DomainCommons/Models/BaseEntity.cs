using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chen.DomainCommons.Models
{
    public record BaseEntity : DomainEvents, IEntity
    {
        public long Id { get; set; }

        //public Guid LogicId { get; protected set; } = Guid.NewGuid();

    }
}
