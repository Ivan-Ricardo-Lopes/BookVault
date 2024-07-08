using FluentValidation;

namespace BallastLane.BookVault.Application.Features.Books.GetAllBooks
{
    public class GetAllBooksValidator : AbstractValidator<GetAllBooksQuery>
    {
        public GetAllBooksValidator()
        {
            RuleFor(x => x.PageNumber)
                .GreaterThan(0);

            RuleFor(x => x.PageSize)
                .GreaterThan(0);
        }
    }
}
