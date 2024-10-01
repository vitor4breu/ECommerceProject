namespace Application.UseCases.Order;

public record ItemOutput(Guid Id, string Name, decimal Price, uint Quantity);
