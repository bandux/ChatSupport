using ChatSupportSystem.Models;
using ChatSupportSystem.Consumers;
using ChatSupportSystem.Contracts;
using ChatSupportSystem.CQRS.Commands;
using MassTransit;
using MassTransit.Testing;
using MediatR;
using Moq;
using Xunit;

public class ChatSessionConsumerTests
{
    [Fact]
    public async Task ChatSessionConsumer_Should_Call_Mediator_With_Command()
    {
        // Arrange
        var mediatorMock = new Mock<IMediator>();

        var fakeSession = new ChatSession { Id = Guid.NewGuid() };

        mediatorMock
            .Setup(m => m.Send(It.IsAny<EnqueueChatSessionCommand>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<(bool, string, ChatSession?)>((true, "Accepted", fakeSession)));

        using var harness = new InMemoryTestHarness();
        var consumerHarness = harness.Consumer(() => new ChatSessionConsumer(mediatorMock.Object));

        await harness.Start();
        try
        {
            var sessionId = Guid.NewGuid();
            var requestedAt = DateTime.UtcNow;

            // Act
            await harness.InputQueueSendEndpoint.Send<ChatSessionRequested>(new
            {
                SessionId = sessionId,
                RequestedAt = requestedAt
            });

            // Assert
            Assert.True(await harness.Consumed.Any<ChatSessionRequested>(), "Message not consumed");
            Assert.True(await consumerHarness.Consumed.Any<ChatSessionRequested>(), "Consumer did not receive the message");

            mediatorMock.Verify(m => m.Send(It.Is<EnqueueChatSessionCommand>(cmd => cmd.SessionId == sessionId), It.IsAny<CancellationToken>()), Times.Once);
        }
        finally
        {
            await harness.Stop();
        }
    }
}
