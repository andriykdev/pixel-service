using System.Text.Json;
using FluentAssertions;
using Messages;
using Microsoft.Extensions.Logging;
using NSubstitute;
using StorageService.Services;

namespace StorageService.UnitTests;

public class MessageHandlerTests
{
    private readonly MessageHandler _sut;

    private readonly ILogWriter _writer = Substitute.For<ILogWriter>();
    private readonly ILogger<MessageHandler> _logger = Substitute.For<ILogger<MessageHandler>>();

    public MessageHandlerTests()
    {
        _sut = new MessageHandler(_writer, _logger);
    }

    [Fact]
    public void MessageHandler_ProcessesMessageSuccessfully()
    {
        var date = DateTime.UtcNow;
        var ip = "127.0.0.1";
        var referer = "ref";
        var ua = "ua";

        var vi = new VisitInfo(ip, ua, referer, date);
        var expectedLog = $"{date:o}|{referer}|{ua}|{ip}";

        var message = JsonSerializer.Serialize(vi);

        var result = _sut.Handle(message);

        result.Should().BeTrue();
        _writer.Received(1).Write(expectedLog);
    }


    [Fact]
    public void MessageHandler_DoesNotProcessesMessage()
    {
        var message = JsonSerializer.Serialize("{wrong:json:format}");

        var act = () => _sut.Handle(message);

        var result = act();

        result.Should().BeFalse();
        act.Should().NotThrow();
        _writer.Received(0).Write(Arg.Any<string>());
    }
}