namespace ChatSupportSystem.Contracts;

public interface ChatSessionRequested
{
    Guid SessionId { get; }
    DateTime RequestedAt { get; }
}
