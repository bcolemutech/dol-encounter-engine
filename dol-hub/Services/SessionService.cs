namespace dol_hub.Services;

using dol_sdk.POCOs;
using Google.Cloud.Firestore;

public interface ISessionService
{
    Task<ISession?> GetSession(string sessionId);
    Task Upsert(ISession session);
    Task Terminate(string sessionId);
}

public class SessionService : ISessionService
{
    private const string Sessions = "sessions";
    private readonly FirestoreDb _db;
    
    public SessionService(IConfiguration configuration)
    {
        _db = FirestoreDb.Create(configuration["ProjectId"]);
    }
    
    public async Task<ISession?> GetSession(string sessionId)
    {
        var docRef = _db.Collection(Sessions).Document(sessionId);

        var snapshot = await docRef.GetSnapshotAsync();

        return snapshot.Exists
            ? snapshot.ConvertTo<Session>()
            : throw new KeyNotFoundException($"No session found with ID: {sessionId}");
    }

    public async Task Upsert(ISession session)
    {
        var docRef = _db.Collection(Sessions).Document(session.ID);
        await docRef.SetAsync(session, SetOptions.MergeAll);
    }

    public async Task Terminate(string sessionId)
    {
        var docRef = _db.Collection(Sessions).Document(sessionId);
        await docRef.DeleteAsync();
    }
}
