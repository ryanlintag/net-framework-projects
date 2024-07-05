using MediatorLibrary.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MediatorLibrary
{
    /// <summary>
    /// Represents a mediator implementation that handles sending requests and returning responses.
    /// </summary>
    public class Mediator : IMediator
    {
        private readonly Dictionary<Type, Type> _handlerTypes;

        /// <summary>
        /// Initializes a new instance of the <see cref="Mediator"/> class.
        /// </summary>
        public Mediator()
        {
            _handlerTypes = new Dictionary<Type, Type>();
            ScanRequestAssemblies();
        }

        /// <summary>
        /// Sends a request and returns the response.
        /// </summary>
        /// <typeparam name="TRequest">The type of the request.</typeparam>
        /// <typeparam name="TResponse">The type of the response.</typeparam>
        /// <param name="request">The request to send.</param>
        /// <returns>A task representing the asynchronous operation that returns the response.</returns>
        public async Task<Result<TResponse>> Send<TRequest, TResponse>(TRequest request)
            where TRequest : IRequest<TResponse>
            where TResponse : class
        {
            var requestType = typeof(TRequest);
            if (!_handlerTypes.TryGetValue(requestType, out var handlerType))
            {
                return new Result<TResponse>(new Error("Mediator.RequestMapping", $"No handler registered for request type: {requestType.Name}"));
            }

            var handler = (IRequestHandler<TRequest, TResponse>)Activator.CreateInstance(handlerType);

            try
            {
                return await handler.Handle(request);
            }
            catch (Exception ex)
            {
                return new Result<TResponse>(new Error("Mediator.InvokingUnhandledException", ex.Message));
            }
        }

        /// <summary>
        /// Scans the assemblies to find request handlers and registers them.
        /// </summary>
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

                    _handlerTypes[requestType] = handlerType;
                }
            }
        }
    }

}
