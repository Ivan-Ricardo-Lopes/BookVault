using BallastLane.BookVault.Application.Common;
using BallastLane.BookVault.Domain.Entities;
using BallastLane.BookVault.Domain.Repositories;
using FluentValidation;
using FluentValidation.Results;

namespace BallastLane.BookVault.Application.Features.Books.AddBook
{
    public class AddBookHandler(IValidator<AddBookCommand> validator, IBookRepository bookRepository) : IRequestHandler<AddBookCommand, AddBookResponse>
    {
        private readonly IValidator<AddBookCommand> _validator = validator;
        private readonly IBookRepository _bookRepository = bookRepository;

        public async Task<HandlerResult<AddBookResponse>> HandleAsync(AddBookCommand request, CancellationToken cancellationToken)
        {
            ValidationResult validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
                return HandlerResult<AddBookResponse>.Failure(validationResult.Errors);

            var book = new Book
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                Author = request.Author,
                ISBN = request.ISBN,
                CreatedBy = request.CreatedBy,
                CreatedDateUtc = DateTime.UtcNow
            };

            await _bookRepository.AddAsync(book, cancellationToken);

            return HandlerResult<AddBookResponse>.Success(new AddBookResponse(book.Id));
        }
    }
}