namespace Catalog.Domain.Events;

public record ProductUpdatedEvent(
    Guid ProductId,
    string Name,
    Guid VendorId) : DomainEvent;
