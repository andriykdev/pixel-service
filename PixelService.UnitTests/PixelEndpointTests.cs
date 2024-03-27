using System.Net;
using FluentAssertions;
using Messages;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using PixelService.Services;

namespace PixelService.UnitTests;

public class PixelEndpointTests
{
    private readonly IPublisher _publisher = Substitute.For<IPublisher>();

    [Fact]
    public async Task TrackEndpoint_ReturnsGifImage()
    {
        var application = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.AddScoped(_ => _publisher);
                });
            });

        var client = application.CreateClient();

        var response = await client.GetAsync("/track");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType.ToString().Should().Be("image/gif");
        response.Content.ReadAsByteArrayAsync().Result.Length.Should().Be(42);
    }


    [Fact]
    public async Task TrackEndpoint_CallsPublisherWithVisitInfo()
    {
        var application = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.AddScoped(_ => _publisher);
                });
            });

        var client = application.CreateClient();

      
        var response = await client.GetAsync("/track");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        _publisher.Received(1).Send(Arg.Any<VisitInfo>());
    }
}