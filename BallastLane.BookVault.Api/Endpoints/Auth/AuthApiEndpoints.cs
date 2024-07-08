using BallastLane.BookVault.Api.Jwt;
using BallastLane.BookVault.Application.Common;
using BallastLane.BookVault.Application.Features.Books.GetBook;
using BallastLane.BookVault.Application.Features.Users.SignIn;
using BallastLane.BookVault.Application.Features.Users.SignUp;

namespace BallastLane.BookVault.Api.Endpoints.Auth
{
    public static class AuthApiEndpoints
    {
        public static void ConfigureEndpoints(WebApplication app)
        {
            app.MapPost("/auth/signin", async (HttpContext httpContext, SignInCommand request, IRequestHandler<SignInCommand, SignInResponse> handler, JwtTokenGenerator jwtGenerator) =>
            {
                HandlerResult<SignInResponse> response = await handler.HandleAsync(request, httpContext.RequestAborted);

                if (response.IsSuccess)
                {
                    var token = jwtGenerator.GenerateToken(request.Username);
                    return Results.Ok(token);
                }
                else
                    return Results.Unauthorized();
            })
            .WithOpenApi();

            app.MapPost("/auth/signup", async (HttpContext httpContext, SignUpCommand request, IRequestHandler<SignUpCommand> handler) =>
            {
                HandlerResult response = await handler.HandleAsync(request, httpContext.RequestAborted);

                if (response.IsSuccess)
                    return Results.NoContent();
                else
                    return Results.BadRequest(response.ErrorMessages);
            })
            .WithOpenApi();
        }
    }
}
