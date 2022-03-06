namespace dol_hub.Services;

using dol_sdk.POCOs;

public interface ISessionService
{
    Task<ISession?> GetSession(string sessionId);
    Task AddSession(ISession session);
    Task EndSession(string sessionId);
    Task UpdateSession(ISession session);
}

public class SessionService : ISessionService
{
    public Task<ISession?> GetSession(string sessionId)
    {
        throw new NotImplementedException();
    }

    public Task AddSession(ISession session)
    {
        throw new NotImplementedException();
    }

    public Task EndSession(string sessionId)
    {
        throw new NotImplementedException();
    }

    public Task UpdateSession(ISession session)
    {
        throw new NotImplementedException();
    }
}
