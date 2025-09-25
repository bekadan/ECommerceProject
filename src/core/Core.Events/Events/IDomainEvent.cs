namespace Core.Events.Abstractions;

public interface IDomainEvent
{
    DateTime OccurredOn { get; }
}
