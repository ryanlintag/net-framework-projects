using System.Threading.Tasks;

namespace MediatorLibrary
{
    /// <summary>
    /// Represents a handler for a request of type <typeparamref name="TRequest"/> that returns a response of type <typeparamref name="TResponse"/>.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request.</typeparam>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    public interface IRequestHandler<TRequest, TResponse>
                where TRequest : IRequest<TResponse>
                where TResponse : class
    {
        /// <summary>
        /// Handles the specified request and returns a result.
        /// </summary>
        /// <param name="request">The request to handle.</param>
        /// <returns>A task representing the asynchronous operation that returns the result of the request handling.</returns>
        Task<Result<TResponse>> Handle(TRequest request);
    }

}
