using ChatSupportSystem.Models;
using MediatR;

namespace ChatSupportSystem.CQRS.Commands;

public record EnqueueChatSessionCommand(Guid SessionId) : IRequest<(bool IsAccepted, string Message, ChatSession? Session)>;