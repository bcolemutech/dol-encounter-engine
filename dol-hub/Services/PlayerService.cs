namespace dol_hub.Services;

using dol_sdk.POCOs;

public interface IPlayerService
{
    Task<IUser> GetPlayer(string? email);
    Task<IUser> UpdatePlayer(IUser player);
}

public class PlayerService : IPlayerService
{
    public Task<IUser> GetPlayer(string? email)
    {
        throw new NotImplementedException();
    }

    public Task<IUser> UpdatePlayer(IUser player)
    {
        throw new NotImplementedException();
    }
}
