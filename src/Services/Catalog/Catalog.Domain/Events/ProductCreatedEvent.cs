namespace Catalog.Domain.Events;

public record ProductCreatedEvent(
    Guid ProductId,
    string Name,
    Guid VendorId,
    decimal Price,
    Guid CategoryId) : DomainEvent;
