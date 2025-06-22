using System.Reflection;
using ChatSupportSystem.Consumers;
using ChatSupportSystem.Contracts;
using ChatSupportSystem.Models;
using ChatSupportSystem.Services;
using MassTransit;
using MediatR;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly())
);

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<ChatSessionConsumer>();

    x.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host("rabbitmq://localhost", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        cfg.ReceiveEndpoint("chat-session-queue", e =>
        {
            e.ConfigureConsumer<ChatSessionConsumer>(ctx);
        });
    });
});

builder.Services.AddSingleton<ChatQueueService>();

var app = builder.Build();

app.MapGet("/sessions", async (IMediator mediator) =>
{
    var result = await mediator.Send(new ChatSupportSystem.CQRS.Queries.GetAllSessionsQuery());
    return Results.Ok(result);
});

app.MapGet("/poll/{sessionId}", (Guid sessionId, ChatQueueService service) =>
{
    var session = service.GetSessionById(sessionId);
    if (session == null || !session.IsActive)
        return Results.NotFound("Session not found or inactive.");

    session.LastPolledAt = DateTime.UtcNow;
    session.MissedPolls = 0;

    return Results.Ok("Poll received.");
});

app.MapPost("/chats", async (IPublishEndpoint publishEndpoint) =>
{
    var sessionId = Guid.NewGuid();

    await publishEndpoint.Publish<ChatSessionRequested>(new
    {
        SessionId = sessionId,
        RequestedAt = DateTime.UtcNow
    });

    return Results.Ok(new { sessionId, status = "Queued" });
});

app.Run();

var queueService = app.Services.GetRequiredService<ChatQueueService>();
var pollTimer = new PeriodicTimer(TimeSpan.FromSeconds(5));

_ = Task.Run(async () =>
{
    while (await pollTimer.WaitForNextTickAsync())
    {
        queueService.CheckPollTimeouts();
    }
});
