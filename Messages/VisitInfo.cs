namespace Messages;

public record VisitInfo(string IpAddress, string? UserAgent, string? Referer, DateTime DateOfVisit);