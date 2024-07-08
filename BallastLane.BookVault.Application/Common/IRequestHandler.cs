namespace BallastLane.BookVault.Application.Common
{
    public interface IRequestHandler<in TRequest, TResponse> where TRequest : IRequest where TResponse : class?
    {
        Task<HandlerResult<TResponse>> HandleAsync(TRequest request, CancellationToken cancellationToken);
    }

    public interface IRequestHandler<in TRequest> where TRequest : IRequest
    {
        Task<HandlerResult> HandleAsync(TRequest request, CancellationToken cancellationToken);
    }
}
