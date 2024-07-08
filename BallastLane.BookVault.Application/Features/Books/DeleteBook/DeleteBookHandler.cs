using BallastLane.BookVault.Application.Common;
using BallastLane.BookVault.Domain.Repositories;
using FluentValidation;
using FluentValidation.Results;

namespace BallastLane.BookVault.Application.Features.Books.DeleteBook
{
    public class DeleteBookHandler(IValidator<DeleteBookCommand> validator, IBookRepository bookRepository) : IRequestHandler<DeleteBookCommand>
    {
        private readonly IValidator<DeleteBookCommand> _validator = validator;
        private readonly IBookRepository _bookRepository = bookRepository;

        public async Task<HandlerResult> HandleAsync(DeleteBookCommand request, CancellationToken cancellationToken)
        {
            ValidationResult validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
                return HandlerResult.Failure(validationResult.Errors);

            var rowsEffected = await _bookRepository.DeleteAsync(request.Id, request.DeletedBy, cancellationToken);

            if (rowsEffected == 0)
                return HandlerResult.Failure("Book not found.");

            return HandlerResult.Success();
        }
    }
}
