using MediatorLibrary.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MediatorLibrary
{
    public class Mediator : IMediator
    {
        private Dictionary<Type, Type> _handlerTypes { get; set; }
        private IRequestHandlerFactory _handlerFactory { get; set; }

        public Mediator(IRequestHandlerFactory handlerFactory)
        {
            _handlerTypes = new Dictionary<Type, Type>();
            _handlerFactory = handlerFactory;
            ScanRequestAssemblies();
        }
        public void RegisterHandler<TRequest, TResponse>(IRequestHandler<TRequest, TResponse> handler)
            where TRequest : IRequest<TResponse>
            where TResponse : class
        {
            var requestType = typeof(TRequest);
            var responseType = typeof(TResponse);

            _handlerTypes[requestType] = handler.GetType();
        }

        public async Task<Result<TResponse>> Send<TRequest, TResponse>(TRequest request)
            where TRequest : IRequest<TResponse>
            where TResponse : class
        {
            var requestType = typeof(TRequest);
            if (!_handlerTypes.TryGetValue(requestType, out var handlerType))
            {
                return new Result<TResponse>(new Error("Mediator.RequestMapping", $"No handler registered for request type: {requestType.Name}"));
            }

            IRequestHandler<TRequest, TResponse> handler;
            try
            {
                handler = _handlerFactory.CreateHandler<TRequest, TResponse>(handlerType);
            }
            catch(Exception ex)
            {
                return new Result<TResponse>(new Error("Mediator.HandlerCreationException", ex.Message));
            }

            try
            {
                return await handler.Handle(request);
            }
            catch (Exception ex)
            {
                return new Result<TResponse>(new Error("Mediator.InvokingUnhandledException", ex.Message));
            }
        }

        private void ScanRequestAssemblies()
        {
            var executingAssembly = Assembly.GetExecutingAssembly();
            var handlerTypes = executingAssembly.GetTypes()
                .Where(t => t.IsAssignableToGenericType(typeof(IRequestHandler<,>)))
                .ToList();

            foreach (var handlerType in handlerTypes)
            {
                var interfaces = handlerType.GetInterfaces()
                    .Where(i => i.IsAssignableToGenericType(typeof(IRequestHandler<,>)))
                    .ToList();

                foreach (var @interface in interfaces)
                {
                    var requestType = @interface.GetGenericArguments()[0];
                    var responseType = @interface.GetGenericArguments()[1];

                    _handlerTypes[requestType] = @interface;
                }
            }
        }
    }
}
