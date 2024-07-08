using BallastLane.BookVault.Application.Common;
using BallastLane.BookVault.Application.Features.Users.Helpers;
using BallastLane.BookVault.Application.Features.Users.SignUp;
using BallastLane.BookVault.Domain.Entities;
using BallastLane.BookVault.Domain.Repositories;
using FluentValidation;
using FluentValidation.Results;

namespace BallastLane.BookVault.Application.Features.Users.SignIn
{
    public class SignUpHandler(IValidator<SignUpCommand> validator, IUserRepository userRepository) : IRequestHandler<SignUpCommand>
    {
        private readonly IValidator<SignUpCommand> _validator = validator;
        private readonly IUserRepository _userRepository = userRepository;

        public async Task<HandlerResult> HandleAsync(SignUpCommand request, CancellationToken cancellationToken)
        {
            ValidationResult validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
                return HandlerResult<SignInResponse>.Failure(validationResult.Errors);

            bool isUsernameInUse = await _userRepository.ExistsByUsername(request.Username, cancellationToken);

            if (isUsernameInUse)
                return HandlerResult<SignInResponse>.Failure("Username in use.");

            var salt = PasswordHelper.GenerateRandomSalt();
            var passwordHash = PasswordHelper.ComputeHash(request.Password, salt);

            User user = new()
            {
                Id = Guid.NewGuid(),
                PasswordHash = passwordHash,
                Salt = salt,
                Username = request.Username
            };

            await _userRepository.AddUser(user, cancellationToken);

            return HandlerResult<SignInResponse>.Success(new SignInResponse(true));
        }
    }
}
