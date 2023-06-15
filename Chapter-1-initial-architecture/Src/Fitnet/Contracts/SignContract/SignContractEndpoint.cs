namespace EvolutionaryArchitecture.Fitnet.Contracts.SignContract;

using Data.Database;
using Events;
using Shared.Events.EventBus;
using Shared.Events.EventBus.InMemory;

internal static class SignContractEndpoint
{
    internal static void MapSignContract(this IEndpointRouteBuilder app)
    {
        app.MapPatch(ContractsApiPaths.Sign, async (Guid id, SignContractRequest request,
            ContractsPersistence persistence, IEventBus bus, CancellationToken cancellationToken) =>
        {
            var contract =
                await persistence.Contracts.FindAsync(new object?[] { id }, cancellationToken);
            if (contract is null)
                return Results.NotFound();

            contract.Sign(request.SignedAt);
            await persistence.SaveChangesAsync(cancellationToken);
            var @event = ContractSignedEvent.Create(contract.Id, contract.CustomerId);
            await bus.PublishAsync(@event, cancellationToken);
            
            return Results.NoContent();
        });
    }
}