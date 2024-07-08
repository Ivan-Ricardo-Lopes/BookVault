using BallastLane.BookVault.Application.Features.Users.SignUp;
using FluentValidation;

namespace BallastLane.BookVault.Application.Features.Books.AddBook
{
    public class SignUpValidator : AbstractValidator<SignUpCommand>
    {
        public SignUpValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty()
                .MaximumLength(50);

            RuleFor(x => x.Password)
                .NotEmpty()
                .MaximumLength(50);

            RuleFor(x => x.ConfirmPassword)
                .NotEmpty()
                .MaximumLength(50)
                .Equal(x => x.Password);
        }
    }
}
