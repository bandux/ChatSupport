using ChatSupportSystem.Services;
using ChatSupportSystem.Models;
using MediatR;

namespace ChatSupportSystem.CQRS.Queries;

public class GetChatSessionHandler : IRequestHandler<GetChatSessionQuery, ChatSession?>
{
    private readonly ChatQueueService _chatQueueService;

    public GetChatSessionHandler(ChatQueueService chatQueueService)
    {
        _chatQueueService = chatQueueService;
    }

    public Task<ChatSession?> Handle(GetChatSessionQuery request, CancellationToken cancellationToken)
    {
        var session = _chatQueueService.GetSessionById(request.SessionId);
        return Task.FromResult(session);
    }
}
