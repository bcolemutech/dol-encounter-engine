namespace dol_hub;

using dol_sdk.POCOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Services;

[Authorize]
public class GameHub : Hub<IGameClient>
{
    private readonly ISessionService _sessionService;
    private readonly IPlayerService _playerService;

    public GameHub(ISessionService sessionService, IPlayerService playerService)
    {
        _sessionService = sessionService;
        _playerService = playerService;
    }

    public override async Task OnConnectedAsync()
    {
        var user = this.Context.User ?? throw new ArgumentNullException(nameof(this.Context.User));
        //Find a session or create a new one
        var userId = user.Claims.First(c => c.Type == "user_id").Value;
        var player = await _playerService.GetPlayer(userId);

        ISession session;

        if (string.IsNullOrWhiteSpace(player.SessionId))
        {
            session = NewSession(player);
        }
        else
        {
            session = await _sessionService.GetSession(player.SessionId) ?? NewSession(player);
        }

        player.ConnectionId = Context.ConnectionId;
        await _playerService.UpdatePlayer(userId, player);
        session.Players.Add(player);
        await _sessionService.Upsert(session);
        
        await Groups.AddToGroupAsync(Context.ConnectionId, session.ID);
        await base.OnConnectedAsync();

        await Clients.Group(session.ID).GameUpdate(session);
    }

    private static ISession NewSession(IUser player)
    {
        ISession session = new Session();
        session.ID = Guid.NewGuid().ToString();
        session.Players = new List<User>();
        player.SessionId = session.ID;
        return session;
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var user = this.Context.User ?? throw new ArgumentNullException(nameof(this.Context.User));
        var player = await _playerService.GetPlayer(user.Identity?.Name);
        var session = await _sessionService.GetSession(player.SessionId);
        //If game is complete delete it
        if (session is not null)
        {
            session.Players.Remove(player);
            if (session.Players.Count == 0)
            {
                await _sessionService.Terminate(session.ID);
            }
            else
            {
                await _sessionService.Upsert(session);
            }

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, session.ID);
            await Clients.Group(session.ID).PlayerDropOut(player.Email);
        }

        await base.OnDisconnectedAsync(exception);
    }
}

public interface IGameClient
{
    Task GameUpdate(ISession? session);
    Task PlayerDropOut(string playerEmail);
}
