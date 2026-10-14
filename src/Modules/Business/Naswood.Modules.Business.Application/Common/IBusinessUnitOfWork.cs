namespace Naswood.Modules.Business.Application.Common;

public interface IBusinessUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
