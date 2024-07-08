using BallastLane.BookVault.Application.Common;
using BallastLane.BookVault.Domain.Entities;
using BallastLane.BookVault.Domain.Repositories;
using FluentValidation;
using FluentValidation.Results;

namespace BallastLane.BookVault.Application.Features.Books.GetBook
{
    public class GetBookHandler(IValidator<GetBookQuery> validator, IBookRepository bookRepository) : IRequestHandler<GetBookQuery, GetBookResponse>
    {
        private readonly IValidator<GetBookQuery> _validator = validator;
        private readonly IBookRepository _bookRepository = bookRepository;

        public async Task<HandlerResult<GetBookResponse>> HandleAsync(GetBookQuery request, CancellationToken cancellationToken)
        {
            ValidationResult validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
                return HandlerResult<GetBookResponse>.Failure(validationResult.Errors);

            Book? book = await _bookRepository.GetByIdAsync(request.Id, cancellationToken);

            if (book == null)
                return HandlerResult<GetBookResponse>.Failure("Book not found.");

            return HandlerResult<GetBookResponse>.Success(new GetBookResponse(book));
        }
    }
}
