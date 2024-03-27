using Messages;

namespace PixelService.Services;

public interface IPublisher
{
    void Send(VisitInfo visitInfo);
}