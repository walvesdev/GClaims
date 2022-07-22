namespace GClaims.Marvel.Infrastructure.Dapper;

public interface IUnitOfWork : IDisposable
{
    void BeginTransaction();
    void Commit();
    void Rollback();
}