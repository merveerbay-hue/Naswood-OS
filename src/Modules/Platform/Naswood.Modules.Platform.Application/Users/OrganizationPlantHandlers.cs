using Naswood.BuildingBlocks.Application.Abstractions;
using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Platform.Application.Authentication;
using Naswood.Modules.Platform.Domain.Organization;
using Naswood.Modules.Platform.Domain.Users;

namespace Naswood.Modules.Platform.Application.Users;

public sealed record ListOrganizationPlantsQuery : IQuery<Result<IReadOnlyList<OrganizationPlantDto>>>;

public sealed record CreateOrganizationPlantCommand(
    string Code,
    string Name,
    string CompanyCode) : ICommand<Result<OrganizationPlantDto>>;

public sealed class OrganizationPlantDto
{
    public required string Code { get; init; }
    public required string Name { get; init; }
    public required string CompanyCode { get; init; }
    public required bool IsActive { get; init; }
}

public sealed class ListOrganizationPlantsQueryHandler
    : IQueryHandler<ListOrganizationPlantsQuery, Result<IReadOnlyList<OrganizationPlantDto>>>
{
    private readonly IOrganizationReferenceRepository _organization;

    public ListOrganizationPlantsQueryHandler(IOrganizationReferenceRepository organization) =>
        _organization = organization;

    public async Task<Result<IReadOnlyList<OrganizationPlantDto>>> HandleAsync(
        ListOrganizationPlantsQuery query,
        CancellationToken cancellationToken = default)
    {
        var plants = await _organization.ListActivePlantsAsync(cancellationToken).ConfigureAwait(false);
        return Result.Success<IReadOnlyList<OrganizationPlantDto>>(
            plants.Select(p => new OrganizationPlantDto
            {
                Code = p.Code,
                Name = p.Name,
                CompanyCode = p.CompanyCode,
                IsActive = p.IsActive
            }).ToArray());
    }
}

public sealed class CreateOrganizationPlantCommandHandler
    : ICommandHandler<CreateOrganizationPlantCommand, Result<OrganizationPlantDto>>
{
    private readonly IOrganizationReferenceRepository _organization;
    private readonly IPlatformUnitOfWork _unitOfWork;

    public CreateOrganizationPlantCommandHandler(
        IOrganizationReferenceRepository organization,
        IPlatformUnitOfWork unitOfWork)
    {
        _organization = organization;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<OrganizationPlantDto>> HandleAsync(
        CreateOrganizationPlantCommand command,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(command.Code) || string.IsNullOrWhiteSpace(command.Name))
            return Result.Failure<OrganizationPlantDto>(UserErrors.Validation("Plant code and name are required."));

        var company = await _organization.GetCompanyByCodeAsync(command.CompanyCode, cancellationToken)
            .ConfigureAwait(false);
        if (company is null)
            return Result.Failure<OrganizationPlantDto>(UserErrors.CompanyNotFound(command.CompanyCode));

        var existing = await _organization.GetPlantByCodeAsync(command.Code, cancellationToken).ConfigureAwait(false);
        if (existing is not null)
            return Result.Failure<OrganizationPlantDto>(UserErrors.Validation($"Plant '{command.Code}' already exists."));

        var plant = PlantReference.Create(command.Code, command.Name, company.Code);
        await _organization.AddPlantAsync(plant, cancellationToken).ConfigureAwait(false);
        await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return Result.Success(new OrganizationPlantDto
        {
            Code = plant.Code,
            Name = plant.Name,
            CompanyCode = plant.CompanyCode,
            IsActive = plant.IsActive
        });
    }
}
