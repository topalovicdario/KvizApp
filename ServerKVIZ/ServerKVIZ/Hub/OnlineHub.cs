using MySqlX.XDevAPI;
using Microsoft.AspNetCore.SignalR;
using MySqlX.XDevAPI;
using ServerKVIZ.Models;
using ServerKVIZ.Repositoryes;

//jos uvijek u postupku implementacije, zanemarite ovaj kod
public class OnlineHub : Hub
{
    private readonly IHubContext<GameHub> _gameHubContext;
    private readonly PlayerRepository _onlineDataBase;

    public OnlineHub(IHubContext<GameHub> gameHubContext, PlayerRepository onlineDataBase)
    {
        _gameHubContext = gameHubContext;
        _onlineDataBase = onlineDataBase;
    }

    public override async Task OnConnectedAsync()
    {
        var nickname = Context.User.Identity.Name;

        _onlineDataBase.AddConnection(_onlineDataBase.GetOnlineUsers().FirstOrDefault(y => y.NickName == nickname), Context.ConnectionId);
        await UpdateOnlineUsers();
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        var nickname = Context.User.Identity.Name;

        _onlineDataBase.RemoveConnection(_onlineDataBase.GetOnlineUsers().FirstOrDefault(y => y.NickName == nickname), Context.ConnectionId);
        await UpdateOnlineUsers();
        await base.OnDisconnectedAsync(exception);
    }

    private async Task UpdateOnlineUsers()
    {
        var onlineUsers = _onlineDataBase.GetOnlineUsers();
        var names = onlineUsers.Select(x => x.NickName).ToList();
        await Clients.All.SendAsync("OnlineUsersUpdated", names);
    }
    public async Task SendInvite(string targetNickname)
    {
        var targetPlayer = _onlineDataBase.GetPlayerConnectionByNickname(targetNickname);

        if (targetPlayer != null && !string.IsNullOrEmpty(targetPlayer))
        {
            await Clients.Client(targetPlayer).SendAsync("ReceiveInvite", Context.User.Identity.Name);
        }
        else
        {
            await Clients.Caller.SendAsync("UserNotOnline", targetNickname);
        }
    }
    public async Task RespondToInvite(string fromNickname, bool accepted)
    {
        var fromPlayerConnectionId = _onlineDataBase.GetPlayerConnectionByNickname(fromNickname);
        var currentPlayerConnectionId = Context.ConnectionId;

        if (fromPlayerConnectionId != null && !string.IsNullOrEmpty(fromPlayerConnectionId))
        {
            if (accepted)
            {
                string gameSessionId = Guid.NewGuid().ToString();

                // Dodajte oba korisnika u grupu igre
                await _gameHubContext.Groups.AddToGroupAsync(fromPlayerConnectionId, gameSessionId);
                await _gameHubContext.Groups.AddToGroupAsync(currentPlayerConnectionId, gameSessionId);

                // Obavestite oba korisnika da je igra spremna
                await Clients.Client(fromPlayerConnectionId).SendAsync("GameReady", gameSessionId);
                await Clients.Client(currentPlayerConnectionId).SendAsync("GameReady", gameSessionId);
            }
            else
            {
                await Clients.Client(fromPlayerConnectionId).SendAsync("InviteDeclined", Context.User.Identity.Name);
            }
        }
    }
}
