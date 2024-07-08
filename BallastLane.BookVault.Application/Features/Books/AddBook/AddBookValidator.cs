using FluentValidation;

namespace BallastLane.BookVault.Application.Features.Books.AddBook
{
    public class AddBookValidator : AbstractValidator<AddBookCommand>
    {
        public AddBookValidator()
        {
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
