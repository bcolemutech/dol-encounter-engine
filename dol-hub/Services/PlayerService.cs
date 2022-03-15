namespace dol_hub.Services;

using Google.Cloud.Firestore;
using Microsoft.Extensions.Configuration;
using dol_sdk.POCOs;

public interface IPlayerService
{
    Task<User> GetPlayer(string? userId);
    Task UpdatePlayer(string userId, User? player);
}

public class PlayerService : IPlayerService
{
    private const string Players = "players";
    private readonly FirestoreDb _db;

    public PlayerService(IConfiguration configuration)
    {
        _db = FirestoreDb.Create(configuration["ProjectId"]);
    }

    public async Task<User> GetPlayer(string? userId)
    {
        var docRef = _db.Collection(Players).Document(userId);

        var snapshot = await docRef.GetSnapshotAsync();

        return snapshot.Exists
            ? snapshot.ConvertTo<User>()
            : throw new KeyNotFoundException($"No player found with ID: {userId}");
    }

    public async Task UpdatePlayer(string userId, User? player)
    {
        var docRef = _db.Collection(Players).Document(userId);
        await docRef.SetAsync(player, SetOptions.MergeAll);
    }
}
