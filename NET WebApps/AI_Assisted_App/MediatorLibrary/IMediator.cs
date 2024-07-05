using System.Threading.Tasks;

namespace MediatorLibrary
{
    /// <summary>
    /// Represents a mediator that handles the communication between different components.
    /// </summary>
    public interface IMediator
    {
        /// <summary>
        /// Sends a request and returns the result asynchronously.
        /// </summary>
        /// <typeparam name="TRequest">The type of the request.</typeparam>
        /// <typeparam name="TResponse">The type of the response.</typeparam>
        /// <param name="request">The request to be sent.</param>
        /// <returns>A task representing the asynchronous operation. The task result contains the response.</returns>
        Task<Result<TResponse>> Send<TRequest, TResponse>(TRequest request)
            where TRequest : IRequest<TResponse>
            where TResponse : class;
    }
}
