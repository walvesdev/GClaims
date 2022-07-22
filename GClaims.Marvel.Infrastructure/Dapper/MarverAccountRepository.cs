using Dapper;
using Dapper.Contrib.Extensions;
using GClaims.Core.Extensions;
using GClaims.Marvel.Core.Models;

namespace GClaims.Marvel.Infrastructure.Dapper;

public class MarverAccountRepository
{
    private readonly DbSession _session;

    public MarverAccountRepository(DbSession session)
    {
        _session = session;
    }

    public async Task<IEnumerable<MarvelAccount>> GetAll()
    {
        return await _session.Connection
            .GetAllAsync<MarvelAccount>(_session.Transaction);
    }
    
    public async Task<MarvelAccount> Find(int id)
    {
        return await _session.Connection
            .GetAsync<MarvelAccount>(id, _session.Transaction);
    }

    public async Task<long> Save(MarvelAccount model)
    {
        return await _session.Connection
            .InsertAsync(model, _session.Transaction);
    }
    
    public async Task<bool> Update(MarvelAccount model)
    {
        return await _session.Connection
            .UpdateAsync(model, _session.Transaction);
    }
    
    public async Task<bool> Delete(MarvelAccount model)
    {
        return await _session.Connection
            .DeleteAsync(model, _session.Transaction);
    }
}