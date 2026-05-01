namespace Catalog.Domain.Events;

public record ProductPriceChangedEvent(
    Guid ProductId,
    decimal OldPrice,
    decimal NewPrice) : DomainEvent;
