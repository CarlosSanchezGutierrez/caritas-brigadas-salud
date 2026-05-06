using Caritas.Brigadas.Api.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Caritas.Brigadas.Api.Tests.Middleware;

public sealed class RequestTelemetryMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_CallsNextMiddleware()
    {
        var wasCalled = false;
        var context = new DefaultHttpContext();
        context.Request.Method = "GET";
        context.Request.Path = "/api/v1/health";

        var middleware = new RequestTelemetryMiddleware(
            _ =>
            {
                wasCalled = true;
                return Task.CompletedTask;
            },
            NullLogger<RequestTelemetryMiddleware>.Instance);

        await middleware.InvokeAsync(context);

        Assert.True(wasCalled);
    }

    [Fact]
    public async Task InvokeAsync_DoesNotReadOrMutateRequestBody()
    {
        var context = new DefaultHttpContext();
        context.Request.Method = "POST";
        context.Request.Path = "/api/v1/patients";
        context.Response.StatusCode = StatusCodes.Status204NoContent;

        await using var bodyStream = new MemoryStream();
        context.Request.Body = bodyStream;

        var middleware = new RequestTelemetryMiddleware(
            _ => Task.CompletedTask,
            NullLogger<RequestTelemetryMiddleware>.Instance);

        await middleware.InvokeAsync(context);

        Assert.Equal(0, context.Request.Body.Position);
        Assert.Equal(StatusCodes.Status204NoContent, context.Response.StatusCode);
    }

    [Fact]
    public async Task InvokeAsync_PreservesExceptionsFromNextMiddleware()
    {
        var context = new DefaultHttpContext();
        context.Request.Method = "GET";
        context.Request.Path = "/api/v1/health";

        var middleware = new RequestTelemetryMiddleware(
            _ => throw new InvalidOperationException("Expected failure."),
            NullLogger<RequestTelemetryMiddleware>.Instance);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => middleware.InvokeAsync(context));

        Assert.Equal("Expected failure.", exception.Message);
    }
}
