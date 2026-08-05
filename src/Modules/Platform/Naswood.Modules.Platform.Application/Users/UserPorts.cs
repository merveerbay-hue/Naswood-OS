using Naswood.BuildingBlocks.Domain;
using Naswood.Modules.Platform.Domain.Authentication;
using Naswood.Modules.Platform.Domain.Organization;
using Naswood.Modules.Platform.Domain.Users;

namespace Naswood.Modules.Platform.Application.Users;

public sealed record UserSearchCriteria(
    string? EmployeeNumber,
    string? Username,
    string? Name,
    string? Email,
    string? DepartmentCode,
    string? CompanyId,
    string? PlantId,
    UserAccountStatus? Status,
    int Page,
    int PageSize);

public interface IUserManagementRepository
{
    Task<AuthUser?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<AuthUser?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);

    Task<bool> UsernameExistsAsync(string username, Guid? excludingUserId, CancellationToken cancellationToken = default);

    Task<bool> EmailExistsAsync(string email, Guid? excludingUserId, CancellationToken cancellationToken = default);

    Task<bool> EmployeeNumberExistsAsync(
        string employeeNumber,
        Guid? excludingUserId,
        CancellationToken cancellationToken = default);

    Task AddAsync(AuthUser user, CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<AuthUser> Items, int TotalCount)> SearchAsync(
        UserSearchCriteria criteria,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<AuthUser>> ListActiveForExportAsync(CancellationToken cancellationToken = default);
}

public interface IOrganizationReferenceRepository
{
    Task<CompanyReference?> GetCompanyByCodeAsync(string code, CancellationToken cancellationToken = default);

    Task<PlantReference?> GetPlantByCodeAsync(string code, CancellationToken cancellationToken = default);

    Task<DepartmentReference?> GetDepartmentByCodeAsync(string code, CancellationToken cancellationToken = default);

    Task<PositionReference?> GetPositionByCodeAsync(string code, CancellationToken cancellationToken = default);

    Task<bool> AnyAsync(CancellationToken cancellationToken = default);

    Task SeedAsync(
        IEnumerable<CompanyReference> companies,
        IEnumerable<PlantReference> plants,
        IEnumerable<DepartmentReference> departments,
        IEnumerable<PositionReference> positions,
        CancellationToken cancellationToken = default);
}

public interface IUserHistoryRepository
{
    Task AddAsync(UserHistoryEntry entry, CancellationToken cancellationToken = default);
}

public sealed class UserHistoryEntry
{
    public Guid Id { get; init; } = UuidV7.NewGuid();

    public required Guid UserId { get; init; }

    public Guid? ActorUserId { get; init; }

    public required string Action { get; init; }

    public string? Details { get; init; }

    public required string CorrelationId { get; init; }

    public required DateTimeOffset OccurredAt { get; init; }
}
