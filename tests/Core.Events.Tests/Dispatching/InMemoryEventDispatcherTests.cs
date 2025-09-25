using Core.Events.Dispatching;
using Core.Events.Handlers;
using Core.Events.Tests.Events;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Events.Tests.Dispatching;

public class InMemoryEventDispatcherTests
{
    [Fact]
    public async Task DispatchAsync_ShouldInvokeRegisteredHandler()
    {
        var services = new ServiceCollection();
        var handler = new ProductCreatedHandler();
        services.AddSingleton<IEventHandler<ProductCreatedEvent>>(handler);
        services.AddSingleton<IEventDispatcher, InMemoryEventDispatcher>();
        var provider = services.BuildServiceProvider();

        var dispatcher = provider.GetRequiredService<IEventDispatcher>();
        var @event = new ProductCreatedEvent(Guid.NewGuid());

        await dispatcher.DispatchAsync(@event);

        Assert.Equal(1, handler.CallCount);
    }
}
