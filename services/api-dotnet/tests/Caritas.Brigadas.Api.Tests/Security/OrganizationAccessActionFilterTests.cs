using Caritas.Brigadas.Api.Security;
using Caritas.Brigadas.Application.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Xunit;

namespace Caritas.Brigadas.Api.Tests.Security;

public sealed class OrganizationAccessActionFilterTests
{
    [Fact]
    public async Task OnActionExecutionAsync_WhenEndpointAllowsAnonymous_CallsNext()
    {
        var organizationId = Guid.NewGuid();
        var authorizer = new FakeOrganizationAccessAuthorizer(canAccess: false);
        var filter = new OrganizationAccessActionFilter(authorizer);

        var context = CreateContext(organizationId);
        context.HttpContext.SetEndpoint(new Endpoint(
            _ => Task.CompletedTask,
            new EndpointMetadataCollection(new AllowAnonymousAttribute()),
            "anonymous-endpoint"));

        var nextCalled = false;

        await filter.OnActionExecutionAsync(
            context,
            CreateNextDelegate(context, () => nextCalled = true));

        Assert.True(nextCalled);
        Assert.Null(context.Result);
    }

    [Fact]
    public async Task OnActionExecutionAsync_WhenNoOrganizationId_CallsNext()
    {
        var authorizer = new FakeOrganizationAccessAuthorizer(canAccess: false);
        var filter = new OrganizationAccessActionFilter(authorizer);

        var context = CreateContext(organizationId: null);
        var nextCalled = false;

        await filter.OnActionExecutionAsync(
            context,
            CreateNextDelegate(context, () => nextCalled = true));

        Assert.True(nextCalled);
        Assert.Null(context.Result);
    }

    [Fact]
    public async Task OnActionExecutionAsync_WhenOrganizationAccessIsAllowed_CallsNext()
    {
        var organizationId = Guid.NewGuid();
        var authorizer = new FakeOrganizationAccessAuthorizer(canAccess: true);
        var filter = new OrganizationAccessActionFilter(authorizer);

        var context = CreateContext(organizationId);
        var nextCalled = false;

        await filter.OnActionExecutionAsync(
            context,
            CreateNextDelegate(context, () => nextCalled = true));

        Assert.True(nextCalled);
        Assert.Null(context.Result);
        Assert.Equal(organizationId, authorizer.LastOrganizationId);
    }

    [Fact]
    public async Task OnActionExecutionAsync_WhenOrganizationAccessIsDenied_ReturnsForbid()
    {
        var organizationId = Guid.NewGuid();
        var authorizer = new FakeOrganizationAccessAuthorizer(canAccess: false);
        var filter = new OrganizationAccessActionFilter(authorizer);

        var context = CreateContext(organizationId);
        var nextCalled = false;

        await filter.OnActionExecutionAsync(
            context,
            CreateNextDelegate(context, () => nextCalled = true));

        Assert.False(nextCalled);
        Assert.IsType<ForbidResult>(context.Result);
        Assert.Equal(organizationId, authorizer.LastOrganizationId);
    }

    private static ActionExecutingContext CreateContext(Guid? organizationId)
    {
        var httpContext = new DefaultHttpContext();

        var routeData = new RouteData();

        if (organizationId.HasValue)
        {
            routeData.Values["organizationId"] = organizationId.Value.ToString();
        }

        var actionContext = new ActionContext(
            httpContext,
            routeData,
            new ActionDescriptor());

        return new ActionExecutingContext(
            actionContext,
            new List<IFilterMetadata>(),
            new Dictionary<string, object?>(),
            controller: new object());
    }

    private static ActionExecutionDelegate CreateNextDelegate(
        ActionExecutingContext executingContext,
        Action onNext)
    {
        return () =>
        {
            onNext();

            var executedContext = new ActionExecutedContext(
                executingContext,
                executingContext.Filters,
                executingContext.Controller);

            return Task.FromResult(executedContext);
        };
    }

    private sealed class FakeOrganizationAccessAuthorizer : IOrganizationAccessAuthorizer
    {
        private readonly bool _canAccess;

        public FakeOrganizationAccessAuthorizer(bool canAccess)
        {
            _canAccess = canAccess;
        }

        public Guid? LastOrganizationId { get; private set; }

        public bool CanAccessOrganization(Guid organizationId)
        {
            LastOrganizationId = organizationId;
            return _canAccess;
        }

        public void EnsureCanAccessOrganization(Guid organizationId)
        {
            if (!CanAccessOrganization(organizationId))
            {
                throw new UnauthorizedAccessException();
            }
        }
    }
}
