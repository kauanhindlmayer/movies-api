using FluentValidation;
using Movies.Application.Models;

namespace Movies.Application.Validators;

public class GetAllMoviesOptionsValidator : AbstractValidator<GetAllMoviesOptions>
{
    private static readonly string[] AcceptableSortFields = ["Title", "YearOfRelease"];
    
    public GetAllMoviesOptionsValidator()
    {
        RuleFor(x => x.YearOfRelease)
            .LessThanOrEqualTo(DateTime.UtcNow.Year);
        
        RuleFor(x => x.SortField)
            .Must(x => AcceptableSortFields.Contains(x, StringComparer.OrdinalIgnoreCase))
            .When(x => x.SortField is not null)
            .WithMessage($"Invalid sort field. Must be one of: {string.Join(", ", AcceptableSortFields)}");
        
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Page must be greater than or equal to 1");
        
        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage("Page size must be between 1 and 100");
    }
}