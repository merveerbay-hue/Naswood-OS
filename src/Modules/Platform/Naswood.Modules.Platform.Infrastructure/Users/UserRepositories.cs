using Microsoft.EntityFrameworkCore;
using Naswood.Modules.Platform.Application.Users;
using Naswood.Modules.Platform.Domain.Authentication;
using Naswood.Modules.Platform.Domain.Organization;
using Naswood.Modules.Platform.Domain.Users;
using Naswood.Modules.Platform.Infrastructure.Persistence;

namespace Naswood.Modules.Platform.Infrastructure.Users;

public sealed class UserManagementRepository : IUserManagementRepository
{
    private readonly PlatformDbContext _db;

    public UserManagementRepository(PlatformDbContext db) => _db = db;

    public Task<AuthUser?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        _db.AuthUsers.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task<AuthUser?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default) =>
        _db.AuthUsers.FirstOrDefaultAsync(x => x.Username == username, cancellationToken);

    public Task<bool> UsernameExistsAsync(
        string username,
        Guid? excludingUserId,
        CancellationToken cancellationToken = default)
    {
        var normalized = username.Trim();
        return _db.AuthUsers.AnyAsync(
            x => !x.IsDeleted &&
                 x.Username == normalized &&
                 (!excludingUserId.HasValue || x.Id != excludingUserId.Value),
            cancellationToken);
    }

    public Task<bool> EmailExistsAsync(
        string email,
        Guid? excludingUserId,
        CancellationToken cancellationToken = default)
    {
        var normalized = email.Trim();
        return _db.AuthUsers.AnyAsync(
            x => !x.IsDeleted &&
                 x.Email != null &&
                 x.Email.ToLower() == normalized.ToLower() &&
                 (!excludingUserId.HasValue || x.Id != excludingUserId.Value),
            cancellationToken);
    }

    public Task<bool> EmployeeNumberExistsAsync(
        string employeeNumber,
        Guid? excludingUserId,
        CancellationToken cancellationToken = default)
    {
        var normalized = employeeNumber.Trim();
        return _db.AuthUsers.AnyAsync(
            x => !x.IsDeleted &&
                 x.EmployeeNumber != null &&
                 x.EmployeeNumber == normalized &&
                 (!excludingUserId.HasValue || x.Id != excludingUserId.Value),
            cancellationToken);
    }

    public async Task AddAsync(AuthUser user, CancellationToken cancellationToken = default) =>
        await _db.AuthUsers.AddAsync(user, cancellationToken).ConfigureAwait(false);

    public async Task<(IReadOnlyList<AuthUser> Items, int TotalCount)> SearchAsync(
        UserSearchCriteria criteria,
        CancellationToken cancellationToken = default)
    {
        var query = _db.AuthUsers.AsNoTracking().Where(x => !x.IsDeleted);

        if (!string.IsNullOrWhiteSpace(criteria.EmployeeNumber))
        {
            var value = criteria.EmployeeNumber.Trim();
            query = query.Where(x => x.EmployeeNumber != null && EF.Functions.ILike(x.EmployeeNumber, $"%{value}%"));
        }

        if (!string.IsNullOrWhiteSpace(criteria.Username))
        {
            var value = criteria.Username.Trim();
            query = query.Where(x => EF.Functions.ILike(x.Username, $"%{value}%"));
        }

        if (!string.IsNullOrWhiteSpace(criteria.Name))
        {
            var value = criteria.Name.Trim();
            query = query.Where(x =>
                EF.Functions.ILike(x.DisplayName, $"%{value}%") ||
                (x.FirstName != null && EF.Functions.ILike(x.FirstName, $"%{value}%")) ||
                (x.LastName != null && EF.Functions.ILike(x.LastName, $"%{value}%")));
        }

        if (!string.IsNullOrWhiteSpace(criteria.Email))
        {
            var value = criteria.Email.Trim();
            query = query.Where(x => x.Email != null && EF.Functions.ILike(x.Email, $"%{value}%"));
        }

        if (!string.IsNullOrWhiteSpace(criteria.DepartmentCode))
        {
            var value = criteria.DepartmentCode.Trim().ToUpper();
            query = query.Where(x => x.DepartmentCode == value);
        }

        if (criteria.Status is not null)
        {
            query = query.Where(x => x.Status == criteria.Status);
        }

        // Company/plant filters applied in memory because assignments are stored as text[].
        var matched = await query.OrderBy(x => x.Username).ToListAsync(cancellationToken).ConfigureAwait(false);

        if (!string.IsNullOrWhiteSpace(criteria.CompanyId))
        {
            var company = criteria.CompanyId.Trim();
            matched = matched
                .Where(x => x.CompanyIds.Contains(company, StringComparer.OrdinalIgnoreCase))
                .ToList();
        }

        if (!string.IsNullOrWhiteSpace(criteria.PlantId))
        {
            var plant = criteria.PlantId.Trim();
            matched = matched
                .Where(x => x.PlantIds.Contains(plant, StringComparer.OrdinalIgnoreCase))
                .ToList();
        }

        var total = matched.Count;
        var items = matched
            .Skip((criteria.Page - 1) * criteria.PageSize)
            .Take(criteria.PageSize)
            .ToList();

        return (items, total);
    }

    public async Task<IReadOnlyList<AuthUser>> ListActiveForExportAsync(
        CancellationToken cancellationToken = default) =>
        await _db.AuthUsers.AsNoTracking()
            .Where(x => !x.IsDeleted)
            .OrderBy(x => x.Username)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
}

public sealed class OrganizationReferenceRepository : IOrganizationReferenceRepository
{
    private readonly PlatformDbContext _db;

    public OrganizationReferenceRepository(PlatformDbContext db) => _db = db;

    public Task<CompanyReference?> GetCompanyByCodeAsync(
        string code,
        CancellationToken cancellationToken = default)
    {
        var normalized = code.Trim().ToUpperInvariant();
        return _db.Companies.FirstOrDefaultAsync(x => x.Code == normalized, cancellationToken);
    }

    public Task<PlantReference?> GetPlantByCodeAsync(
        string code,
        CancellationToken cancellationToken = default)
    {
        var normalized = code.Trim().ToUpperInvariant();
        return _db.Plants.FirstOrDefaultAsync(x => x.Code == normalized, cancellationToken);
    }

    public Task<DepartmentReference?> GetDepartmentByCodeAsync(
        string code,
        CancellationToken cancellationToken = default)
    {
        var normalized = code.Trim().ToUpperInvariant();
        return _db.Departments.FirstOrDefaultAsync(x => x.Code == normalized, cancellationToken);
    }

    public Task<PositionReference?> GetPositionByCodeAsync(
        string code,
        CancellationToken cancellationToken = default)
    {
        var normalized = code.Trim().ToUpperInvariant();
        return _db.Positions.FirstOrDefaultAsync(x => x.Code == normalized, cancellationToken);
    }

    public Task<bool> AnyAsync(CancellationToken cancellationToken = default) =>
        _db.Companies.AnyAsync(cancellationToken);

    public async Task SeedAsync(
        IEnumerable<CompanyReference> companies,
        IEnumerable<PlantReference> plants,
        IEnumerable<DepartmentReference> departments,
        IEnumerable<PositionReference> positions,
        CancellationToken cancellationToken = default)
    {
        await _db.Companies.AddRangeAsync(companies, cancellationToken).ConfigureAwait(false);
        await _db.Plants.AddRangeAsync(plants, cancellationToken).ConfigureAwait(false);
        await _db.Departments.AddRangeAsync(departments, cancellationToken).ConfigureAwait(false);
        await _db.Positions.AddRangeAsync(positions, cancellationToken).ConfigureAwait(false);
    }
}

public sealed class UserHistoryRepository : IUserHistoryRepository
{
    private readonly PlatformDbContext _db;

    public UserHistoryRepository(PlatformDbContext db) => _db = db;

    public async Task AddAsync(UserHistoryEntry entry, CancellationToken cancellationToken = default) =>
        await _db.UserHistory.AddAsync(entry, cancellationToken).ConfigureAwait(false);
}
