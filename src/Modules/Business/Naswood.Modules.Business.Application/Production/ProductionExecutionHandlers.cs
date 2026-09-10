using Naswood.BuildingBlocks.Application.Abstractions;
using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Business.Application.Common;
using Naswood.Modules.Business.Application.Inventory;
using Naswood.Modules.Business.Contracts.Production;
using Naswood.Modules.Business.Domain.Inventory;
using Naswood.Modules.Business.Domain.Production;

namespace Naswood.Modules.Business.Application.Production;

public sealed record ListShopFloorWorkCentersQuery(IReadOnlyList<string>? AllowedPlantIds)
    : IQuery<Result<IReadOnlyList<ShopFloorWorkCenterCardDto>>>;

public sealed record GetShopFloorQueueQuery(Guid WorkCenterId, IReadOnlyList<string>? AllowedPlantIds)
    : IQuery<Result<ShopFloorQueueDto>>;

public sealed record GetProductionOrderProgressQuery(Guid OrderId, IReadOnlyList<string>? AllowedPlantIds)
    : IQuery<Result<ProductionOrderProgressDto>>;

public sealed record UpsertProductionOperationsCommand(
    Guid OrderId,
    IReadOnlyList<UpsertProductionOperationRequestDto> Steps,
    IReadOnlyList<string>? AllowedPlantIds) : ICommand<Result<ProductionOrderProgressDto>>;

public sealed record StartProductionExecutionCommand(
    Guid OperationId,
    Guid? WorkCenterId,
    IReadOnlyList<string>? AllowedPlantIds,
    string Actor) : ICommand<Result<ProductionExecutionPassportDto>>;

public sealed record GetProductionExecutionQuery(Guid ExecutionId, IReadOnlyList<string>? AllowedPlantIds)
    : IQuery<Result<ProductionExecutionPassportDto>>;

public sealed record PauseProductionExecutionCommand(Guid ExecutionId, IReadOnlyList<string>? AllowedPlantIds, string Actor)
    : ICommand<Result<ProductionExecutionPassportDto>>;

public sealed record ResumeProductionExecutionCommand(Guid ExecutionId, IReadOnlyList<string>? AllowedPlantIds, string Actor)
    : ICommand<Result<ProductionExecutionPassportDto>>;

public sealed record ScanProductionExecutionQuery(Guid ExecutionId, string Barcode, IReadOnlyList<string>? AllowedPlantIds)
    : IQuery<Result<ProductionExecutionScanDto>>;

public sealed record ConsumeProductionExecutionCommand(
    Guid ExecutionId,
    ConsumeProductionExecutionRequestDto Body,
    IReadOnlyList<string>? AllowedPlantIds,
    string Actor) : ICommand<Result<ProductionExecutionPassportDto>>;

public sealed record StartDowntimeCommand(Guid ExecutionId, DowntimeRequestDto Body, IReadOnlyList<string>? AllowedPlantIds, string Actor)
    : ICommand<Result<ProductionExecutionPassportDto>>;

public sealed record EndDowntimeCommand(Guid ExecutionId, DowntimeRequestDto Body, IReadOnlyList<string>? AllowedPlantIds, string Actor)
    : ICommand<Result<ProductionExecutionPassportDto>>;

public sealed record AddExecutionScrapCommand(Guid ExecutionId, ScrapRequestDto Body, IReadOnlyList<string>? AllowedPlantIds, string Actor)
    : ICommand<Result<ProductionExecutionPassportDto>>;

public sealed record CompleteProductionExecutionCommand(
    Guid ExecutionId,
    CompleteProductionExecutionRequestDto Body,
    IReadOnlyList<string>? AllowedPlantIds,
    string Actor) : ICommand<Result<ProductionExecutionPassportDto>>;

public sealed record CancelProductionExecutionCommand(
    Guid ExecutionId,
    CancelProductionExecutionRequestDto Body,
    IReadOnlyList<string>? AllowedPlantIds,
    string Actor) : ICommand<Result<ProductionExecutionPassportDto>>;

public sealed class ListShopFloorWorkCentersQueryHandler : IQueryHandler<ListShopFloorWorkCentersQuery, Result<IReadOnlyList<ShopFloorWorkCenterCardDto>>>
{
    private readonly ProductionExecutionGateway _gate;
    public ListShopFloorWorkCentersQueryHandler(ProductionExecutionGateway gate) => _gate = gate;
    public Task<Result<IReadOnlyList<ShopFloorWorkCenterCardDto>>> HandleAsync(ListShopFloorWorkCentersQuery query, CancellationToken cancellationToken = default)
        => _gate.ListWorkCentersAsync(query.AllowedPlantIds, cancellationToken);
}

public sealed class GetShopFloorQueueQueryHandler : IQueryHandler<GetShopFloorQueueQuery, Result<ShopFloorQueueDto>>
{
    private readonly ProductionExecutionGateway _gate;
    public GetShopFloorQueueQueryHandler(ProductionExecutionGateway gate) => _gate = gate;
    public Task<Result<ShopFloorQueueDto>> HandleAsync(GetShopFloorQueueQuery query, CancellationToken cancellationToken = default)
        => _gate.QueueAsync(query.WorkCenterId, query.AllowedPlantIds, cancellationToken);
}

public sealed class GetProductionOrderProgressQueryHandler : IQueryHandler<GetProductionOrderProgressQuery, Result<ProductionOrderProgressDto>>
{
    private readonly ProductionExecutionGateway _gate;
    public GetProductionOrderProgressQueryHandler(ProductionExecutionGateway gate) => _gate = gate;
    public Task<Result<ProductionOrderProgressDto>> HandleAsync(GetProductionOrderProgressQuery query, CancellationToken cancellationToken = default)
        => _gate.OrderProgressAsync(query.OrderId, query.AllowedPlantIds, cancellationToken);
}

public sealed class UpsertProductionOperationsCommandHandler : ICommandHandler<UpsertProductionOperationsCommand, Result<ProductionOrderProgressDto>>
{
    private readonly ProductionExecutionGateway _gate;
    private readonly IBusinessUnitOfWork _uow;
    public UpsertProductionOperationsCommandHandler(ProductionExecutionGateway gate, IBusinessUnitOfWork uow)
    { _gate = gate; _uow = uow; }

    public async Task<Result<ProductionOrderProgressDto>> HandleAsync(UpsertProductionOperationsCommand command, CancellationToken cancellationToken = default)
    {
        var r = await _gate.UpsertOperationsAsync(command.OrderId, command.Steps, command.AllowedPlantIds, cancellationToken).ConfigureAwait(false);
        if (r.IsFailure) return r;
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return r;
    }
}

public sealed class StartProductionExecutionCommandHandler : ICommandHandler<StartProductionExecutionCommand, Result<ProductionExecutionPassportDto>>
{
    private readonly ProductionExecutionGateway _gate;
    private readonly IBusinessUnitOfWork _uow;
    public StartProductionExecutionCommandHandler(ProductionExecutionGateway gate, IBusinessUnitOfWork uow)
    { _gate = gate; _uow = uow; }

    public async Task<Result<ProductionExecutionPassportDto>> HandleAsync(StartProductionExecutionCommand command, CancellationToken cancellationToken = default)
    {
        var r = await _gate.StartAsync(command.OperationId, command.WorkCenterId, command.AllowedPlantIds, command.Actor, cancellationToken).ConfigureAwait(false);
        if (r.IsFailure || r.Value.IdempotentReplay) return r;
        try
        {
            await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return r;
        }
        catch (Exception ex)
        {
            var recovered = await _gate.TryRecoverStartAsync(command.OperationId, command.AllowedPlantIds, ex, cancellationToken).ConfigureAwait(false);
            if (recovered is not null) return recovered;
            throw;
        }
    }
}

public sealed class GetProductionExecutionQueryHandler : IQueryHandler<GetProductionExecutionQuery, Result<ProductionExecutionPassportDto>>
{
    private readonly ProductionExecutionGateway _gate;
    public GetProductionExecutionQueryHandler(ProductionExecutionGateway gate) => _gate = gate;
    public Task<Result<ProductionExecutionPassportDto>> HandleAsync(GetProductionExecutionQuery query, CancellationToken cancellationToken = default)
        => _gate.LoadAsync(query.ExecutionId, query.AllowedPlantIds, cancellationToken);
}

public sealed class PauseProductionExecutionCommandHandler : ICommandHandler<PauseProductionExecutionCommand, Result<ProductionExecutionPassportDto>>
{
    private readonly ProductionExecutionGateway _gate;
    private readonly IBusinessUnitOfWork _uow;
    public PauseProductionExecutionCommandHandler(ProductionExecutionGateway gate, IBusinessUnitOfWork uow)
    { _gate = gate; _uow = uow; }

    public async Task<Result<ProductionExecutionPassportDto>> HandleAsync(PauseProductionExecutionCommand command, CancellationToken cancellationToken = default)
    {
        var r = await _gate.PauseAsync(command.ExecutionId, command.AllowedPlantIds, command.Actor, cancellationToken).ConfigureAwait(false);
        if (r.IsFailure) return r;
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return r;
    }
}

public sealed class ResumeProductionExecutionCommandHandler : ICommandHandler<ResumeProductionExecutionCommand, Result<ProductionExecutionPassportDto>>
{
    private readonly ProductionExecutionGateway _gate;
    private readonly IBusinessUnitOfWork _uow;
    public ResumeProductionExecutionCommandHandler(ProductionExecutionGateway gate, IBusinessUnitOfWork uow)
    { _gate = gate; _uow = uow; }

    public async Task<Result<ProductionExecutionPassportDto>> HandleAsync(ResumeProductionExecutionCommand command, CancellationToken cancellationToken = default)
    {
        var r = await _gate.ResumeAsync(command.ExecutionId, command.AllowedPlantIds, command.Actor, cancellationToken).ConfigureAwait(false);
        if (r.IsFailure) return r;
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return r;
    }
}

public sealed class ScanProductionExecutionQueryHandler : IQueryHandler<ScanProductionExecutionQuery, Result<ProductionExecutionScanDto>>
{
    private readonly ProductionExecutionGateway _gate;
    public ScanProductionExecutionQueryHandler(ProductionExecutionGateway gate) => _gate = gate;
    public Task<Result<ProductionExecutionScanDto>> HandleAsync(ScanProductionExecutionQuery query, CancellationToken cancellationToken = default)
        => _gate.ScanAsync(query.ExecutionId, query.Barcode, query.AllowedPlantIds, cancellationToken);
}

public sealed class ConsumeProductionExecutionCommandHandler : ICommandHandler<ConsumeProductionExecutionCommand, Result<ProductionExecutionPassportDto>>
{
    private readonly ProductionExecutionGateway _gate;
    private readonly IBusinessUnitOfWork _uow;
    public ConsumeProductionExecutionCommandHandler(ProductionExecutionGateway gate, IBusinessUnitOfWork uow)
    { _gate = gate; _uow = uow; }

    public async Task<Result<ProductionExecutionPassportDto>> HandleAsync(ConsumeProductionExecutionCommand command, CancellationToken cancellationToken = default)
    {
        var r = await _gate.ConsumeAsync(command.ExecutionId, command.Body, command.AllowedPlantIds, command.Actor, cancellationToken).ConfigureAwait(false);
        if (r.IsFailure || r.Value.IdempotentReplay) return r;
        try
        {
            await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return r;
        }
        catch (Exception ex)
        {
            var recovered = await _gate.TryRecoverConsumeAsync(command.ExecutionId, command.Body, command.AllowedPlantIds, ex, cancellationToken).ConfigureAwait(false);
            if (recovered is not null) return recovered;
            throw;
        }
    }
}

public sealed class StartDowntimeCommandHandler : ICommandHandler<StartDowntimeCommand, Result<ProductionExecutionPassportDto>>
{
    private readonly ProductionExecutionGateway _gate;
    private readonly IBusinessUnitOfWork _uow;
    public StartDowntimeCommandHandler(ProductionExecutionGateway gate, IBusinessUnitOfWork uow)
    { _gate = gate; _uow = uow; }

    public async Task<Result<ProductionExecutionPassportDto>> HandleAsync(StartDowntimeCommand command, CancellationToken cancellationToken = default)
    {
        var r = await _gate.StartDowntimeAsync(command.ExecutionId, command.Body, command.AllowedPlantIds, command.Actor, cancellationToken).ConfigureAwait(false);
        if (r.IsFailure) return r;
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return r;
    }
}

public sealed class EndDowntimeCommandHandler : ICommandHandler<EndDowntimeCommand, Result<ProductionExecutionPassportDto>>
{
    private readonly ProductionExecutionGateway _gate;
    private readonly IBusinessUnitOfWork _uow;
    public EndDowntimeCommandHandler(ProductionExecutionGateway gate, IBusinessUnitOfWork uow)
    { _gate = gate; _uow = uow; }

    public async Task<Result<ProductionExecutionPassportDto>> HandleAsync(EndDowntimeCommand command, CancellationToken cancellationToken = default)
    {
        var r = await _gate.EndDowntimeAsync(command.ExecutionId, command.Body, command.AllowedPlantIds, command.Actor, cancellationToken).ConfigureAwait(false);
        if (r.IsFailure) return r;
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return r;
    }
}

public sealed class AddExecutionScrapCommandHandler : ICommandHandler<AddExecutionScrapCommand, Result<ProductionExecutionPassportDto>>
{
    private readonly ProductionExecutionGateway _gate;
    private readonly IBusinessUnitOfWork _uow;
    public AddExecutionScrapCommandHandler(ProductionExecutionGateway gate, IBusinessUnitOfWork uow)
    { _gate = gate; _uow = uow; }

    public async Task<Result<ProductionExecutionPassportDto>> HandleAsync(AddExecutionScrapCommand command, CancellationToken cancellationToken = default)
    {
        var r = await _gate.AddScrapAsync(command.ExecutionId, command.Body, command.AllowedPlantIds, command.Actor, cancellationToken).ConfigureAwait(false);
        if (r.IsFailure) return r;
        await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return r;
    }
}

public sealed class CompleteProductionExecutionCommandHandler : ICommandHandler<CompleteProductionExecutionCommand, Result<ProductionExecutionPassportDto>>
{
    private readonly ProductionExecutionGateway _gate;
    private readonly IBusinessUnitOfWork _uow;
    public CompleteProductionExecutionCommandHandler(ProductionExecutionGateway gate, IBusinessUnitOfWork uow)
    { _gate = gate; _uow = uow; }

    public async Task<Result<ProductionExecutionPassportDto>> HandleAsync(CompleteProductionExecutionCommand command, CancellationToken cancellationToken = default)
    {
        var r = await _gate.CompleteAsync(command.ExecutionId, command.Body, command.AllowedPlantIds, command.Actor, cancellationToken).ConfigureAwait(false);
        if (r.IsFailure || r.Value.IdempotentReplay) return r;
        try
        {
            await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return r;
        }
        catch (Exception ex)
        {
            var recovered = await _gate.TryRecoverCompleteAsync(command.ExecutionId, command.AllowedPlantIds, ex, cancellationToken).ConfigureAwait(false);
            if (recovered is not null) return recovered;
            throw;
        }
    }
}

public sealed class CancelProductionExecutionCommandHandler : ICommandHandler<CancelProductionExecutionCommand, Result<ProductionExecutionPassportDto>>
{
    private readonly ProductionExecutionGateway _gate;
    private readonly IBusinessUnitOfWork _uow;
    public CancelProductionExecutionCommandHandler(ProductionExecutionGateway gate, IBusinessUnitOfWork uow)
    { _gate = gate; _uow = uow; }

    public async Task<Result<ProductionExecutionPassportDto>> HandleAsync(CancelProductionExecutionCommand command, CancellationToken cancellationToken = default)
    {
        var r = await _gate.CancelAsync(command.ExecutionId, command.Body, command.AllowedPlantIds, command.Actor, cancellationToken).ConfigureAwait(false);
        if (r.IsFailure || r.Value.IdempotentReplay) return r;
        try
        {
            await _uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return r;
        }
        catch (Exception ex)
        {
            var recovered = await _gate.TryRecoverCancelAsync(command.ExecutionId, command.AllowedPlantIds, ex, cancellationToken).ConfigureAwait(false);
            if (recovered is not null) return recovered;
            throw;
        }
    }
}
