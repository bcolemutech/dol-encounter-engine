namespace dol_hub.Services;

using dol_sdk.POCOs;
using Google.Cloud.Firestore;

public interface ISessionService
{
    Task<ISession?> GetSession(string sessionId);
    Task Upsert(ISession session);
    Task Terminate(string sessionId);
    Task<ISession> NewSession(User player);
}

public class SessionService : ISessionService
{
    private const string Sessions = "sessions";
    private readonly FirestoreDb _db;
    
    private readonly ICharacterService _characterService;
    
    public SessionService(IConfiguration configuration, ICharacterService characterService)
    {
        _characterService = characterService;
        _db = FirestoreDb.Create(configuration["ProjectId"]);
    }
    
    public async Task<ISession?> GetSession(string sessionId)
    {
        var docRef = _db.Collection(Sessions).Document(sessionId);

        var snapshot = await docRef.GetSnapshotAsync();

        return snapshot.Exists
            ? snapshot.ConvertTo<Session>()
            : null;
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

    public async Task<ISession> NewSession(User player)
    {
        var character = await _characterService.GetCharacter(player.UserId, player.CurrentCharacter);

        if (character is null)
        {
            throw new NullReferenceException("Current character not set");
        }
        
        ISession session = new Session();
        session.ID = Guid.NewGuid().ToString();
        session.Players = new List<User>();
        session.Position = character.Position;
        player.SessionId = session.ID;
        return session;
    }
}
