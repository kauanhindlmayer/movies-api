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
    }
}