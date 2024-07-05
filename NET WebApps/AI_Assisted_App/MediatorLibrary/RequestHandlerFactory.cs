using System;

namespace MediatorLibrary
{
    public class RequestHandlerFactory : IRequestHandlerFactory
    {
        public object CreateHandler(Type handlerType)
        {
            return Activator.CreateInstance(handlerType);
        }

        public IRequestHandler<TRequest, TResponse> CreateHandler<TRequest, TResponse>(Type handlerType)
            where TRequest : IRequest<TResponse>
            where TResponse : class
        {
            return (IRequestHandler<TRequest, TResponse>)Activator.CreateInstance(handlerType);
        }
    }
}
