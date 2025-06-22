using ChatSupportSystem.Models;

namespace ChatSupportSystem.CQRS;

public interface ICommandHandler<TCommand>
{
    (bool IsAccepted, string Message, ChatSession? Session) Handle(TCommand command);
}
