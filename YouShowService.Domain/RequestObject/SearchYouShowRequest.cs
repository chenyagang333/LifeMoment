using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouShowService.Domain.RequestObject
{
    public record SearchYouShowRequest(string sort, string keyword, int pageIndex, int pageSize);
    public class SearchYouShowRequestValidator : AbstractValidator<SearchYouShowRequest>
    {
        public SearchYouShowRequestValidator()
        {
            RuleFor(e => e.keyword).NotNull().MinimumLength(1).MaximumLength(100);
            RuleFor(e => e.pageIndex).GreaterThan(0);//页号从1开始
            RuleFor(e => e.pageSize).GreaterThanOrEqualTo(5);
        }
    }
}
