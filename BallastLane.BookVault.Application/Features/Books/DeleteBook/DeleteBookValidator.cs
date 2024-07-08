using FluentValidation;

namespace BallastLane.BookVault.Application.Features.Books.DeleteBook
{
    public class DeleteBookValidator : AbstractValidator<DeleteBookCommand>
    {
        public DeleteBookValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty();
        }
    }
}