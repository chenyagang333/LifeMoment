using FluentValidation;

namespace SearchService.Request;

public record SearchUsersRequest(string sort,string keyword, int pageIndex, int pageSize);

public class SearchUsersRequestValidator : AbstractValidator<SearchUsersRequest>
{
    public SearchUsersRequestValidator()
    {
        RuleFor(e => e.keyword).NotNull().MinimumLength(1).MaximumLength(100);
        RuleFor(e => e.pageIndex).GreaterThan(0);//页号从1开始
        RuleFor(e => e.pageSize).GreaterThanOrEqualTo(5);
    }
}
