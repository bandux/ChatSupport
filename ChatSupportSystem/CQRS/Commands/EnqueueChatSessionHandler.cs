using ChatSupportSystem.Models;
using ChatSupportSystem.Services;
using MediatR;

namespace ChatSupportSystem.CQRS.Commands;

public class EnqueueChatSessionHandler : IRequestHandler<EnqueueChatSessionCommand, (bool, string, ChatSession?)>
{
    private readonly ChatQueueService _chatQueueService;

    public EnqueueChatSessionHandler(ChatQueueService chatQueueService)
    {
        _chatQueueService = chatQueueService;
    }

    public Task<(bool, string, ChatSession?)> Handle(EnqueueChatSessionCommand request, CancellationToken cancellationToken)
    {
        var result = _chatQueueService.EnqueueChatSession(request.SessionId);
        return Task.FromResult(result);
    }
}
