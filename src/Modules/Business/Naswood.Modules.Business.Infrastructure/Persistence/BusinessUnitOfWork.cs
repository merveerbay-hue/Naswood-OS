using Naswood.Modules.Business.Application.Common;

namespace Naswood.Modules.Business.Infrastructure.Persistence;

public sealed class BusinessUnitOfWork : IBusinessUnitOfWork
{
    private readonly BusinessDbContext _db;
    public BusinessUnitOfWork(BusinessDbContext db) => _db = db;
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _db.SaveChangesAsync(cancellationToken);
}
