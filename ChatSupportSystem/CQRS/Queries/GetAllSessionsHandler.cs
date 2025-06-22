using ChatSupportSystem.Models;
using ChatSupportSystem.Services;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ChatSupportSystem.CQRS.Queries;

public class GetAllSessionsHandler : IRequestHandler<GetAllSessionsQuery, List<ChatSession>>
{
    private readonly ChatQueueService _service;

    public GetAllSessionsHandler(ChatQueueService service)
    {
        _service = service;
    }

    public Task<List<ChatSession>> Handle(GetAllSessionsQuery request, CancellationToken cancellationToken)
    {
        var sessions = _service.GetAllSessions();
        return Task.FromResult(sessions);
    }
}
