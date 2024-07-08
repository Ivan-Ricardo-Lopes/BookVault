using BallastLane.BookVault.Application.Common;
using FluentValidation.Results;

namespace BallastLane.BookVault.Application.Common
{
    public record HandlerResult
    {
        public bool IsSuccess { get; }
        public IEnumerable<string> ErrorMessages { get; }

        public HandlerResult(bool isSuccess, IEnumerable<string> errorMessages)
        {
            IsSuccess = isSuccess;
            ErrorMessages = errorMessages;
        }

        public HandlerResult(bool isSuccess, IEnumerable<ValidationFailure> validationFailures)
        {
            IsSuccess = isSuccess;
            ErrorMessages = validationFailures.Select(error => error.ErrorMessage);
        }

        public static HandlerResult Success() => new(true, Enumerable.Empty<string>());
        public static HandlerResult Failure(string errorMessage) => new(false, new[] { errorMessage });
        public static HandlerResult Failure(IEnumerable<string> ErrorMessages) => new(false, ErrorMessages);
        public static HandlerResult Failure(IEnumerable<ValidationFailure> validationFailures) => new(false, validationFailures);
    }
    public record HandlerResult<T> : HandlerResult
    {
        public T? Result { get; }

        public HandlerResult(bool isSuccess, IEnumerable<string> errorMessages, T? result) : base(isSuccess, errorMessages)
        {
            Result = result;
        }

        public HandlerResult(bool isSuccess, IEnumerable<ValidationFailure> validationFailures, T? result) : base(isSuccess, validationFailures)
        {
            Result = result;
        }

        public static HandlerResult<T> Success(T result) => new(true, Enumerable.Empty<string>(), result);
        public static new HandlerResult<T> Failure(string errorMessage) => new(false, new[] { errorMessage }, default);
        public static new HandlerResult<T> Failure(IEnumerable<string> ErrorMessages) => new (false, ErrorMessages, default);
        public static new HandlerResult<T> Failure(IEnumerable<ValidationFailure> validationFailures) => new (false, validationFailures, default);
    }
}



