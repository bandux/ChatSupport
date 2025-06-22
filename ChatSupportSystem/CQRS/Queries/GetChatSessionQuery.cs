using ChatSupportSystem.Models;
using MediatR;

namespace ChatSupportSystem.CQRS.Queries;

public record GetChatSessionQuery(Guid SessionId) : IRequest<ChatSession?>;
