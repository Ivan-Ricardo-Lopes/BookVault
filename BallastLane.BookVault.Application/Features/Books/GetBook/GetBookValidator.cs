using FluentValidation;

namespace BallastLane.BookVault.Application.Features.Books.GetBook
{
    public class GetBookValidator : AbstractValidator<GetBookQuery>
    {
        public GetBookValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty();
        }
    }
}
