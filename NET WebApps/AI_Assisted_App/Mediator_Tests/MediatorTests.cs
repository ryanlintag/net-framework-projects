using System;
using System.Threading.Tasks;
using MediatorLibrary;
using Moq;
using Xunit;

public class MediatorTests
{
    [Fact]
    public async Task Send_ValidRequest_ReturnsSuccess()
    {
        // Arrange
        var mediator = SetupMediatorWithHandler<ValidRequest, string>(new ValidRequestHandler());
        mediator.RegisterHandler(new ValidRequestHandler());
        var request = new ValidRequest();

        // Act
        var result = await mediator.Send<ValidRequest, string>(request);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
    }

    [Fact]
    public async Task Send_NoHandlerRegistered_ReturnsError()
    {
        // Arrange
        var handlerFactoryMock = new Mock<IRequestHandlerFactory>();
        var mediator = new Mediator(handlerFactoryMock.Object);

        // Act
        var result = await mediator.Send<UnregisteredRequest, string>(new UnregisteredRequest());

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Mediator.RequestMapping", result.Error.Code);
    }

    [Fact]
    public async Task Send_HandlerCreationException_ReturnsError()
    {
        // Arrange
        var mediator = SetupMediatorWithFaultyHandler<ValidRequest, string>();
        mediator.RegisterHandler(new FaultyRequestHandler());

        // Act
        var result = await mediator.Send<ValidRequest, string>(new ValidRequest());

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Mediator.HandlerCreationException", result.Error.Code);
    }

    [Fact]
    public async Task Send_HandlerExecutionException_ReturnsError()
    {
        // Arrange
        var mediator = SetupMediatorWithHandler<ValidRequest, string>(new FaultyRequestHandler());
        mediator.RegisterHandler(new FaultyRequestHandler());

        // Act
        var result = await mediator.Send<ValidRequest, string>(new ValidRequest());

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Mediator.InvokingUnhandledException", result.Error.Code);
    }

    // Helper methods to setup mediator with different scenarios
    private Mediator SetupMediatorWithHandler<TRequest, TResponse>(IRequestHandler<TRequest, TResponse> handler)
        where TRequest : IRequest<TResponse>
        where TResponse : class
    {
        // Mock IRequestHandlerFactory and setup to return the provided handler
        var handlerFactoryMock = new Mock<IRequestHandlerFactory>();
        handlerFactoryMock.Setup(f => f.CreateHandler<TRequest, TResponse>(It.IsAny<Type>())).Returns(handler);

        // Initialize Mediator with the mocked IRequestHandlerFactory
        return new Mediator(handlerFactoryMock.Object);
    }

    private Mediator SetupMediatorWithFaultyHandler<TRequest, TResponse>()
        where TRequest : IRequest<TResponse>
        where TResponse : class
    {
        // Mock IRequestHandlerFactory and setup to throw an exception when creating a handler
        var handlerFactoryMock = new Mock<IRequestHandlerFactory>();
        handlerFactoryMock.Setup(f => f.CreateHandler<TRequest, TResponse>(It.IsAny<Type>())).Throws(new Exception("Handler creation failed"));

        // Initialize Mediator with the mocked IRequestHandlerFactory
        return new Mediator(handlerFactoryMock.Object);
    }

}


public class ValidRequest : IRequest<string>
{
}

public class UnregisteredRequest : IRequest<string>
{
}

public class ValidRequestHandler : IRequestHandler<ValidRequest, string>
{
    public Task<Result<string>> Handle(ValidRequest request)
    {
        return Task.FromResult(new Result<string>("Success"));
    }
}

public class FaultyRequestHandler : IRequestHandler<ValidRequest, string>
{
    public Task<Result<string>> Handle(ValidRequest request)
    {
        throw new Exception("Handler execution failed");
    }
}