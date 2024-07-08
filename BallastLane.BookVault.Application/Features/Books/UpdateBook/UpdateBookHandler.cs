using BallastLane.BookVault.Application.Common;
using BallastLane.BookVault.Domain.Entities;
using BallastLane.BookVault.Domain.Repositories;
using FluentValidation;
using FluentValidation.Results;

namespace BallastLane.BookVault.Application.Features.Books.UpdateBook
{
    public class UpdateBookHandler(IValidator<UpdateBookCommand> validator, IBookRepository bookRepository) : IRequestHandler<UpdateBookCommand>
    {
        private readonly IValidator<UpdateBookCommand> _validator = validator;
        private readonly IBookRepository _bookRepository = bookRepository;

        public async Task<HandlerResult> HandleAsync(UpdateBookCommand request, CancellationToken cancellationToken)
        {
            ValidationResult validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
                return HandlerResult.Failure(validationResult.Errors);

            Book book = new()
            {
                Id = request.Id,
                Title = request.Title,
                Author = request.Author,
                ISBN = request.ISBN,
                ModifiedBy = request.ModifiedBy,
                ModifiedDateUtc = DateTime.UtcNow,
            };

            int rowsEffected = await _bookRepository.UpdateAsync(book, cancellationToken);

            if (rowsEffected == 0)
                return HandlerResult.Failure("Book not found.");

            return HandlerResult.Success();
        }
    }
}
