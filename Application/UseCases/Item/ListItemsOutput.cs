namespace Application.UseCases.Item;

public record ListItemsOutput(Guid Id, string Name, decimal Price, uint QuantityInStock);
