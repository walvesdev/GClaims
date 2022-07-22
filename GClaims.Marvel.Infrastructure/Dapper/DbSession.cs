using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;

namespace GClaims.Marvel.Infrastructure.Dapper;

public sealed class DbSession : IDisposable
{
    private Guid _id;
    public IDbConnection Connection { get; }
    public IDbTransaction Transaction { get; set; }

    public DbSession()
    {
        _id = Guid.NewGuid();
        Connection = new SqlConnection("Data Source=tcp:localhost,1433;Initial Catalog=Marvel;User Id=sa;Password=1q2w3E*;MultipleActiveResultSets=True;Connect Timeout=300;");
        Connection.Open();
    }

    public void Dispose() => Connection?.Dispose();
}