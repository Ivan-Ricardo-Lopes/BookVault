using BallastLane.BookVault.Application.Common;

namespace BallastLane.BookVault.Application.Features.Users.SignUp
{
    public sealed record SignUpCommand(string Username, string Password, string ConfirmPassword) : IRequest;
}
