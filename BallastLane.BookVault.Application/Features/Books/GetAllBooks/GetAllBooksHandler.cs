using BallastLane.BookVault.Application.Common;
using BallastLane.BookVault.Domain.Entities;
using BallastLane.BookVault.Domain.Repositories;
using FluentValidation;
using FluentValidation.Results;

namespace BallastLane.BookVault.Application.Features.Books.GetAllBooks
{
    public class GetAllBooksHandler(IValidator<GetAllBooksQuery> validator, IBookRepository bookRepository) : IRequestHandler<GetAllBooksQuery, GetAllBooksResponse>
    {
        private readonly IValidator<GetAllBooksQuery> _validator = validator;
        private readonly IBookRepository _bookRepository = bookRepository;

        public async Task<HandlerResult<GetAllBooksResponse>> HandleAsync(GetAllBooksQuery request, CancellationToken cancellationToken)
        {
            ValidationResult validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
                return HandlerResult<GetAllBooksResponse>.Failure(validationResult.Errors);

            IEnumerable<Book> books = await _bookRepository.GetAllAsync(request.PageNumber, request.PageSize, cancellationToken);

            return HandlerResult<GetAllBooksResponse>.Success(new GetAllBooksResponse(books));
        }
    }
}