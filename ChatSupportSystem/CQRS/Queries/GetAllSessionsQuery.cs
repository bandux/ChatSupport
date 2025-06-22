using ChatSupportSystem.Models;
using MediatR;
using System.Collections.Generic;

namespace ChatSupportSystem.CQRS.Queries;

public record GetAllSessionsQuery : IRequest<List<ChatSession>>;
