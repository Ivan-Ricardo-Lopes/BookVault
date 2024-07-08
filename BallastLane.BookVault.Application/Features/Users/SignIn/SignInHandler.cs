using BallastLane.BookVault.Application.Common;
using BallastLane.BookVault.Application.Features.Users.Helpers;
using BallastLane.BookVault.Domain.Entities;
using BallastLane.BookVault.Domain.Repositories;
using FluentValidation;
using FluentValidation.Results;

namespace BallastLane.BookVault.Application.Features.Users.SignIn
{
    public class SignInHandler(IValidator<SignInCommand> validator, IUserRepository userRepository) : IRequestHandler<SignInCommand, SignInResponse>
    {
        private readonly IValidator<SignInCommand> _validator = validator;
        private readonly IUserRepository _userRepository = userRepository;

        public async Task<HandlerResult<SignInResponse>> HandleAsync(SignInCommand request, CancellationToken cancellationToken)
        {
            ValidationResult validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
                return HandlerResult<SignInResponse>.Failure(validationResult.Errors);

            User? user = await _userRepository.GetUserByUsername(request.Username, cancellationToken);

            if (user == null)
                return HandlerResult<SignInResponse>.Failure(string.Empty);

            bool isAuthenticated = PasswordHelper.VerifyPassword(request.Password, user.Salt, user.PasswordHash);

            if (!isAuthenticated)
                return HandlerResult<SignInResponse>.Failure(string.Empty);

            return HandlerResult<SignInResponse>.Success(new SignInResponse(true));
        }
    }
}
