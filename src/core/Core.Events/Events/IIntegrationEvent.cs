namespace Core.Events.Abstractions;

public interface IIntegrationEvent
{
    DateTime OccurredOn { get; }
    Guid Id { get; }
}
