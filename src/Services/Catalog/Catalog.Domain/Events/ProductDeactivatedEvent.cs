namespace Catalog.Domain.Events;

public record ProductDeactivatedEvent(
    Guid ProductId,
    Guid VendorId) : DomainEvent;
