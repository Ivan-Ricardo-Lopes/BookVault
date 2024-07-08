using BallastLane.BookVault.Api.Endpoints.Books;
using BallastLane.BookVault.Application.Common;
using BallastLane.BookVault.Application.Features.Books.AddBook;
using BallastLane.BookVault.Application.Features.Books.DeleteBook;
using BallastLane.BookVault.Application.Features.Books.GetAllBooks;
using BallastLane.BookVault.Application.Features.Books.GetBook;
using BallastLane.BookVault.Application.Features.Books.UpdateBook;
using BallastLane.BookVault.Application.Features.UpdateBook;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BallastLane.BookVault.Api.Endpoints.Auth
{
    public static class BooksApiEndpoints
    {
        public static void ConfigureEndpoints(WebApplication app)
        {
            app.MapGet("/books/{id}", async (HttpContext httpContext, [FromRoute] Guid id, IRequestHandler<GetBookQuery, GetBookResponse> handler) =>
            {
                HandlerResult<GetBookResponse> response = await handler.HandleAsync(new GetBookQuery(id), httpContext.RequestAborted);

                if (response.IsSuccess)
                    return Results.Ok(response.Result);
                else
                    return Results.BadRequest(response.ErrorMessages);
            })
            .RequireAuthorization()
            .WithOpenApi();

            app.MapPost("/books", async (HttpContext httpContext, [FromBody] AddBookRequest request, IRequestHandler<AddBookCommand, AddBookResponse> handler) =>
            {
                var command = new AddBookCommand(httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!, request.Title, request.Author, request.ISBN);

                HandlerResult<AddBookResponse> response = await handler.HandleAsync(command, httpContext.RequestAborted);

                if (response.IsSuccess)
                    return Results.Created($"/books/{response.Result!.Id}", response.Result);
                else
                    return Results.BadRequest(response.ErrorMessages);
            })
            .RequireAuthorization()
            .WithOpenApi();

            app.MapPut("/books/", async (HttpContext httpContext, [FromBody] UpdateBookRequest request, IRequestHandler<UpdateBookCommand> handler) =>
            {
                var command = new UpdateBookCommand(httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!, request.Id, request.Title, request.Author, request.ISBN);

                HandlerResult response = await handler.HandleAsync(command, httpContext.RequestAborted);

                if (response.IsSuccess)
                    return Results.NoContent();
                else
                    return Results.BadRequest(response.ErrorMessages);
            })
            .RequireAuthorization()
            .WithOpenApi();

            app.MapDelete("/books/{id}", async (HttpContext httpContext, [FromRoute] Guid id, IRequestHandler<DeleteBookCommand> handler) =>
            {
                HandlerResult response = await handler.HandleAsync(new DeleteBookCommand(httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!, id), httpContext.RequestAborted);

                if (response.IsSuccess)
                    return Results.NoContent();
                else
                    return Results.BadRequest(response.ErrorMessages);
            })
            .RequireAuthorization()
            .WithOpenApi();

            app.MapGet("/books/", async (HttpContext httpContext, int? pageNumber, int? pageSize, IRequestHandler<GetAllBooksQuery, GetAllBooksResponse> handler) =>
            {
                HandlerResult<GetAllBooksResponse> response = await handler.HandleAsync(new GetAllBooksQuery(pageNumber, pageSize), httpContext.RequestAborted);

                if (response.IsSuccess)
                    return Results.Ok(response.Result);
                else
                    return Results.BadRequest(response.ErrorMessages);
            })
            .RequireAuthorization()
            .WithOpenApi();
        }
    }
}
