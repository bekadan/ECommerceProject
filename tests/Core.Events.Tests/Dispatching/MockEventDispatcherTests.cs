using Core.Events.Testing;
using Core.Events.Tests.Events;

namespace Core.Events.Tests.Dispatching;

public class MockEventDispatcherTests
{
    [Fact]
    public async Task DispatchAsync_ShouldStoreEventInMemory()
    {
        var dispatcher = new MockEventDispatcher();
        var @event = new ProductCreatedEvent(Guid.NewGuid());

        await dispatcher.DispatchAsync(@event);

        Assert.True(dispatcher.WasDispatched<ProductCreatedEvent>());
    }
}