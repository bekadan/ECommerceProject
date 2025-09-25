using Azure;
using Azure.Messaging.EventGrid;
using Core.Events;
using Core.Events.Abstractions;
using Core.Events.Dispatching;
using Moq;
using System.Text.Json;
using Xunit;

namespace Core.Events.Tests.Dispatching;

public class AzureEventGridDispatcherTests
{
    [Fact]
    public async Task DispatchAsync_ShouldCallSendEventAsync()
    {
        // Arrange
        var mockClient = new Mock<IEventGridClient>();
        EventGridEvent? sentEvent = null;

        mockClient
            .Setup(c => c.SendEventAsync(It.IsAny<EventGridEvent>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask)
            .Callback<EventGridEvent, CancellationToken>((evt, token) => sentEvent = evt);

        var dispatcher = new AzureEventGridDispatcher(mockClient.Object);

        var integrationEvent = new TestIntegrationEvent();

        // Act
        await dispatcher.DispatchAsync(integrationEvent);

        // Assert
        Assert.NotNull(sentEvent);
        Assert.Equal(integrationEvent.GetType().Name, sentEvent.EventType);

        var deserialized = JsonSerializer.Deserialize<TestIntegrationEvent>(sentEvent.Data.ToString()!);
        Assert.Equal(integrationEvent.Id, deserialized?.Id);
    }

    public class TestIntegrationEvent : IIntegrationEvent
    {
        public Guid Id { get; } = Guid.NewGuid();
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }
}