using ChatSupportSystem.Contracts;
using ChatSupportSystem.CQRS.Commands;
using MassTransit;
using MediatR;

namespace ChatSupportSystem.Consumers;

public class ChatSessionConsumer : IConsumer<ChatSessionRequested>
{
    private readonly IMediator _mediator;

    public ChatSessionConsumer(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Consume(ConsumeContext<ChatSessionRequested> context)
    {
        var command = new EnqueueChatSessionCommand(context.Message.SessionId);
        var (accepted, msg, session) = await _mediator.Send(command);

        Console.WriteLine($"[ChatRequest] Session {context.Message.SessionId} - {msg}");
    }
}
