using BallastLane.BookVault.Application.Common;

namespace BallastLane.BookVault.Application.Features.Users.SignIn
{
    public sealed record SignInCommand(string Username, string Password): IRequest;
}
