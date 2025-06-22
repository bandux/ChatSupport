using ChatSupportSystem.CQRS.Commands;
using ChatSupportSystem.CQRS.Queries;
using ChatSupportSystem.Models;
using ChatSupportSystem.Services;
using MediatR;
using Xunit;

namespace ChatSupportSystem.Tests;

public class ChatQueueServiceCQRS_Tests
{
    private readonly ChatQueueService _service;
    private readonly IRequestHandler<EnqueueChatSessionCommand, (bool IsAccepted, string Message, ChatSession? Session)> _enqueueHandler;
    private readonly IRequestHandler<GetChatSessionQuery, ChatSession?> _getSessionHandler;
    private readonly IRequestHandler<GetAllSessionsQuery, List<ChatSession>> _getAllSessionsHandler;

    public ChatQueueServiceCQRS_Tests()
    {
        _service = new ChatQueueService();
        _enqueueHandler = new EnqueueChatSessionHandler(_service);
        _getSessionHandler = new GetChatSessionHandler(_service);
        _getAllSessionsHandler = new GetAllSessionsHandler(_service);
    }

    [Fact]
    public async Task EnqueueChatSession_ShouldAcceptSession_WhenQueueHasCapacity()
    {
        var sessionId = Guid.NewGuid();

        var result = await _enqueueHandler.Handle(new EnqueueChatSessionCommand(sessionId), default);

        Assert.True(result.IsAccepted);
        Assert.Equal("Chat accepted", result.Message);
        Assert.NotNull(result.Session);
        Assert.Equal(sessionId, result.Session!.Id);
    }

    [Fact]
    public async Task GetSessionById_ShouldReturnSession_WhenSessionExists()
    {
        var sessionId = Guid.NewGuid();
        await _enqueueHandler.Handle(new EnqueueChatSessionCommand(sessionId), default);

        var session = await _getSessionHandler.Handle(new GetChatSessionQuery(sessionId), default);

        Assert.NotNull(session);
        Assert.Equal(sessionId, session!.Id);
    }

    [Fact]
    public async Task CheckPollTimeouts_ShouldMarkSessionInactive_WhenMissedPollsReachThree()
    {
        var sessionId = Guid.NewGuid();
        var result = await _enqueueHandler.Handle(new EnqueueChatSessionCommand(sessionId), default);
        var session = result.Session!;
        session.AssignedAgentId = Guid.NewGuid();

        session.LastPolledAt = null;
        _service.CheckPollTimeouts();
        _service.CheckPollTimeouts();
        _service.CheckPollTimeouts();

        var updatedSession = await _getSessionHandler.Handle(new GetChatSessionQuery(sessionId), default);

        Assert.NotNull(updatedSession);
        Assert.False(updatedSession!.IsActive);
        Assert.Equal(3, updatedSession.MissedPolls);
    }

    [Fact]
    public async Task EnqueueChatSession_ShouldUseOverflowAgents_WhenMainQueueIsFull_AndOfficeHours()
    {
        for (int i = 0; i < 10; i++)
        {
            await _enqueueHandler.Handle(new EnqueueChatSessionCommand(Guid.NewGuid()), default);
        }

        var overflowSessionId = Guid.NewGuid();
        var overflowResult = await _enqueueHandler.Handle(new EnqueueChatSessionCommand(overflowSessionId), default);

        Assert.True(overflowResult.IsAccepted);
        Assert.Equal("Chat accepted", overflowResult.Message);
    }

    [Fact]
    public async Task GetAllSessions_ShouldReturnAllEnqueuedSessions()
    {
        await _enqueueHandler.Handle(new EnqueueChatSessionCommand(Guid.NewGuid()), default);
        await _enqueueHandler.Handle(new EnqueueChatSessionCommand(Guid.NewGuid()), default);

        var allSessions = await _getAllSessionsHandler.Handle(new GetAllSessionsQuery(), default);

        Assert.Equal(2, allSessions.Count);
    }
}
