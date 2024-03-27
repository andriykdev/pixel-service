using Messages;
using Microsoft.AspNetCore.Diagnostics;
using PixelService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IPublisher, RabbitMqPublisher>();
builder.Logging.AddConsole();

var app = builder.Build();

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
        logger.LogError(context.Features.Get<IExceptionHandlerFeature>().Error, "An unhandled exception occurred.");

        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await context.Response.WriteAsync("An internal server error occurred.");
    });
});


// Create a 1x1 pixel transparent GIF image
byte[] gifBytes = {
    0x47, 0x49, 0x46, 0x38, 0x39, 0x61, 0x01, 0x00, 0x01, 0x00, 0x80, 0xff, 0x00, 0xff, 0xff, 0xff,
    0x00, 0x00, 0x00, 0x21, 0xf9, 0x04, 0x01, 0x00, 0x00, 0x00, 0x00, 0x2c, 0x00, 0x00, 0x00, 0x00,
    0x01, 0x00, 0x01, 0x00, 0x00, 0x02, 0x01, 0x44, 0x00, 0x3b
};

app.MapGet("/track", async (HttpContext context, IPublisher publisher) =>
{
    // we assuming its not null because no proxy and its a tcp connection
    string ipAddress = context.Connection.RemoteIpAddress?.ToString()!;
    string? userAgent = context.Request.Headers["User-Agent"];
    string? referer = context.Request.Headers["Referer"];

    var visitInfo = new VisitInfo(ipAddress, userAgent, referer, DateTime.UtcNow);

    publisher.Send(visitInfo);

    context.Response.ContentType = "image/gif";
    await context.Response.Body.WriteAsync(gifBytes);
});

app.Run();

// Make the implicit Program class public so test projects can access it
public partial class Program { }