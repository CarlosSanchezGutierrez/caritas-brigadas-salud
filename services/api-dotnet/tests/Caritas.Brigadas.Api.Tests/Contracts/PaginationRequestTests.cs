using Caritas.Brigadas.Contracts.Api;
using Xunit;

namespace Caritas.Brigadas.Api.Tests.Contracts;

public sealed class PaginationRequestTests
{
    [Fact]
    public void NormalizedPageNumber_ClampsVeryLargeValues()
    {
        var request = new PaginationRequest
        {
            PageNumber = int.MaxValue,
            PageSize = PaginationRequest.MaxPageSize
        };

        Assert.Equal(PaginationRequest.MaxPageNumber, request.NormalizedPageNumber);
    }

    [Fact]
    public void Skip_DoesNotOverflow_ForVeryLargePageNumber()
    {
        var request = new PaginationRequest
        {
            PageNumber = int.MaxValue,
            PageSize = PaginationRequest.MaxPageSize
        };

        var expectedSkip = (int)(((long)PaginationRequest.MaxPageNumber - 1L) * PaginationRequest.MaxPageSize);

        Assert.Equal(expectedSkip, request.Skip);
        Assert.InRange(request.Skip, 0, int.MaxValue);
    }

    [Fact]
    public void NormalizedPageSize_ClampsValuesAboveMaximum()
    {
        var request = new PaginationRequest
        {
            PageNumber = 1,
            PageSize = int.MaxValue
        };

        Assert.Equal(PaginationRequest.MaxPageSize, request.NormalizedPageSize);
    }

    [Fact]
    public void NormalizedValues_UseDefaults_ForInvalidLowValues()
    {
        var request = new PaginationRequest
        {
            PageNumber = -100,
            PageSize = -100
        };

        Assert.Equal(PaginationRequest.DefaultPageNumber, request.NormalizedPageNumber);
        Assert.Equal(PaginationRequest.DefaultPageSize, request.NormalizedPageSize);
        Assert.Equal(0, request.Skip);
    }
}