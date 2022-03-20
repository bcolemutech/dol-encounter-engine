namespace dol_hub.Services;

using dol_sdk.POCOs;
using Google.Cloud.Firestore;

public interface ICharacterService
{
    Task<Character?> GetCharacter(string userId, string name);
}

public class CharacterService : ICharacterService
{
    private const string Players = "players";
    private const string Characters = "characters";
    private readonly FirestoreDb _db;

    public CharacterService(IConfiguration configuration)
    {
        _db = FirestoreDb.Create(configuration["ProjectId"]);
    }

    public async Task<Character?> GetCharacter(string userId, string name)
    {
        var docRef = _db.Collection(Players).Document(userId).Collection(Characters).Document($"{name.ToLower()}:");

        var snapshot = await docRef.GetSnapshotAsync();

        return snapshot.Exists ? snapshot.ConvertTo<Character>() : null;
    }
}
