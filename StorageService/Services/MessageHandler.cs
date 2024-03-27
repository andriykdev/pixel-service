using System.Text.Json;
using Messages;

namespace StorageService.Services;

public class MessageHandler
{
    private readonly ILogWriter _writer;
    private readonly ILogger<MessageHandler> _logger;

    public MessageHandler(ILogWriter writer, ILogger<MessageHandler> logger)
    {
        _writer = writer;
        _logger = logger;
    }

    public bool Handle(string message)
    {
        try
        {
            var visitInfo = JsonSerializer.Deserialize<VisitInfo>(message);
            var log = CreateLogEntry(visitInfo);
            _writer.Write(log);
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"Failed to store message: {message}");
            return false;
        }
    }

    private string CreateLogEntry(VisitInfo vi) =>
        $"{vi.DateOfVisit:o}|{vi.Referer ?? "null"}|{vi.UserAgent ?? "null"}|{vi.IpAddress}";
}