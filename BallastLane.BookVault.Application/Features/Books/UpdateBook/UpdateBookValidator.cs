using FluentValidation;

namespace BallastLane.BookVault.Application.Features.Books.UpdateBook
{
    public class UpdateBookValidator : AbstractValidator<UpdateBookCommand>
    {
        public UpdateBookValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty();

            RuleFor(x => x.Title)
                .NotEmpty()
                .MaximumLength(150);

            RuleFor(x => x.Author)
                .NotEmpty()
                .MaximumLength(150);

            RuleFor(x => x.ISBN)
                .Matches(@"^\d{10}$|^\d{13}$");
        }
    }
}
