using Caritas.Brigadas.Domain.Entities;

namespace Caritas.Brigadas.Domain.Tests.Entities;

public sealed class MediaReleaseTests
{
    [Fact]
    public void Constructor_WithNoMediaAllowed_ShouldCreateWithoutBlocking()
    {
        var release = new MediaRelease(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            allowPhoto: false,
            allowVideo: false);

        Assert.False(release.AllowPhoto);
        Assert.False(release.AllowVideo);
        Assert.False(release.AllowsAnyMedia);
        Assert.Equal(MediaReleaseStatus.Active, release.Status);
    }

    [Fact]
    public void UpdatePermissions_ShouldUpdateMediaFlags()
    {
        var release = new MediaRelease(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid());

        release.UpdatePermissions(true, true);

        Assert.True(release.AllowPhoto);
        Assert.True(release.AllowVideo);
        Assert.True(release.AllowsAnyMedia);
    }

    [Fact]
    public void Revoke_ShouldDisableMedia()
    {
        var release = new MediaRelease(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            allowPhoto: true,
            allowVideo: true);

        release.Revoke();

        Assert.False(release.AllowPhoto);
        Assert.False(release.AllowVideo);
        Assert.False(release.AllowsAnyMedia);
        Assert.Equal(MediaReleaseStatus.Revoked, release.Status);
    }
}
