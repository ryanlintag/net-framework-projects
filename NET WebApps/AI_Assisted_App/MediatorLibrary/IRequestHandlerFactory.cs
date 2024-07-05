using System;

namespace MediatorLibrary
{
    public interface IRequestHandlerFactory
    {
        object CreateHandler(Type handlerType);
        IRequestHandler<TRequest, TResponse> CreateHandler<TRequest, TResponse>(Type handlerType)
            where TRequest : IRequest<TResponse>
            where TResponse : class;
    }
}
