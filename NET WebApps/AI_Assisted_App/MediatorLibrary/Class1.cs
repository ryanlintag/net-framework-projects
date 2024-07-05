using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MediatorLibrary
{
    public class Mediator
    {
        private readonly Dictionary<Type, Type> _handlerTypes;

        public Mediator()
        {
            _handlerTypes = new Dictionary<Type, Type>();
            ScanAssembliesForHandlers();
        }

        public void RegisterHandler<TRequest, TResponse, THandler>()
            where TRequest : IRequest<TResponse>
            where TResponse : class
            where THandler : IRequestHandler<TRequest, TResponse>, new()
        {
            _handlerTypes[typeof(TRequest)] = typeof(THandler);
        }

        public async Task<Result<TResponse>> Send<TRequest, TResponse>(TRequest request)
            where TRequest : IRequest<TResponse>
            where TResponse : class
        {
            var requestType = typeof(TRequest);
            if (!_handlerTypes.TryGetValue(requestType, out var handlerType))
            {
                throw new InvalidOperationException($"No handler registered for request type: {requestType.Name}");
            }

            var handler = (IRequestHandler<TRequest, TResponse>)Activator.CreateInstance(handlerType);

            try
            {
                return await handler.Handle(request);
            }
            catch (Exception ex)
            {
                return new Result<TResponse>(new Error("500", ex.Message));
            }
        }

        private void ScanAssembliesForHandlers()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var handlerTypes = assemblies
                .SelectMany(a => a.GetTypes())
                .Where(t => t.GetInterfaces().Any(i =>
                    i.IsGenericType &&
                    i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>)))
                .ToList();

            foreach (var handlerType in handlerTypes)
            {
                var interfaces = handlerType.GetInterfaces()
                    .Where(i => i.IsGenericType &&
                                i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>))
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

    public interface IRequest<TResponse>
    {
        // Define the contract for the request
    }

    public interface IRequestHandler<TRequest, TResponse>
                where TRequest : IRequest<TResponse>
                where TResponse : class
    {
        Task<Result<TResponse>> Handle(TRequest request);
    }

    public sealed class Result<TResponse> where TResponse : class
    {
        public bool IsSuccess { get; private set; }
        public TResponse Value { get; private set; }
        public Error Error { get; private set; }

        public Result(Error error)
        {
            Error = error;
            IsSuccess = false;
        }

        public Result(TResponse value)
        {
            Value = value;
            IsSuccess = true;
        }
    }

    public class Error
    {
        public Error(string code, string message)
        {
            Code = code;
            Message = message;
        }
        public string Code { get; private set; }
        public string Message { get; private set; }
        public static Error NotFound => new Error("404", "Not Found");
        public static Error BadRequest => new Error("400", "Bad Request");
        public static Error InternalServerError => new Error("500", "Internal Server Error");
    }
}
