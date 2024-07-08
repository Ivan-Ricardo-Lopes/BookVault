using BallastLane.BookVault.Application.Features.Users.SignIn;
using FluentValidation;

namespace BallastLane.BookVault.Application.Features.Books.AddBook
{
    public class SignInValidator : AbstractValidator<SignInCommand>
    {
        public SignInValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty()
                .MaximumLength(50);

            RuleFor(x => x.Password)
                .NotEmpty()
                .MaximumLength(50);
        }
    }
}
