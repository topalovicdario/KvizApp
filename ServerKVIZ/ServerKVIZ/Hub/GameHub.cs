using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using ServerKVIZ.Models;
using ServerKVIZ.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
//jos uvijek u postupku implementacije, zanemarite ovaj kod

[Authorize]
public class GameHub : Hub
{
    

    public readonly IGameSessionService gameSessionService;

  
    private static  ConcurrentDictionary<string, string> activePlayers ;
    public GameHub(IGameSessionService gameSessionService)
    {
        this.gameSessionService = gameSessionService;
        activePlayers = new ConcurrentDictionary<string, string>();
    }
    public override async Task OnConnectedAsync()
    {
        // Dohvaćanje korisničkog imena iz tokena
        var userName = Context.User?.FindFirst(ClaimTypes.Name)?.Value;
        if (!string.IsNullOrEmpty(userName))
        {
            // Spremanje aktivnog korisnika. Ključom je ConnectionId, a vrijednost je korisničko ime.
            activePlayers[Context.ConnectionId] = userName;
        }
        var userId = Context.User?.FindFirst(ClaimTypes.Name)?.Value;

        // Kreiranje osobnog lobija za korisnika
        var gameSession = await gameSessionService.CreateGameSession(userId);

        // Dodavanje korisnika u grupu za njegov lobby
        await Groups.AddToGroupAsync(Context.ConnectionId, gameSession.Id.ToString());

        // Obavijest korisniku da je povezan i da ima svoj lobby
        Console.WriteLine($"Korisnik {userName} spojen s Connection ID: {Context.ConnectionId}");
        await Clients.Caller.SendAsync("LobbyCreated", gameSession.Id);


        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        // Uklanjanje korisnika iz liste aktivnih pri prekidu konekcije
       activePlayers.TryRemove(Context.ConnectionId, out var userName);
        Console.WriteLine($"Korisnik {userName} prekinuo konekciju: {Context.ConnectionId}");
        await base.OnDisconnectedAsync(exception);



        var userId = Context.User?.FindFirst(ClaimTypes.Name)?.Value;

        // Dobivanje trenutne sesije korisnika
        int  currentSession = gameSessionService.GetSessionIdForUser(userId);

        if (currentSession != 0 )//ovo nitko nece znat zasto je 0 -> ali 0 je jer ukoliko se nenadje id sesije vrijednost ce biti 0
        {
            // Uklanjanje korisnika iz grupe
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, currentSession.ToString());

            // Obavijest drugim korisnicima u sesiji da je korisnik napustio
            await Clients.Group(currentSession.ToString())
                .SendAsync("PlayerLeft", userId);

            // Ažuriranje sesije
            gameSessionService.RemovePlayerFromSession(userId);
        }

        await base.OnDisconnectedAsync(exception);

        Console.WriteLine($"Korisnik {userName} prekinuo konekciju i uklonjen iz grupa.");
        await base.OnDisconnectedAsync(exception);
    }

 
    public Task<List<string>> GetActiveUsers()
    {
        // Vraća listu jedinstvenih imena iz ActiveUsers kolekcije
        var users = activePlayers.Values.Distinct().ToList();
        return Task.FromResult(users);
    }
   

    public async Task StartGame(int category, int difficulty)
    { var adminId = Context.User?.FindFirst(ClaimTypes.Name)?.Value;
        var gameSession= gameSessionService.GetSessionForUser(adminId);
        gameSessionService.SetParametersOfGameSession(adminId, category, difficulty);
        

        
        await Groups.AddToGroupAsync(Context.ConnectionId, gameSession.Id.ToString());

        Console.WriteLine($"Kreator igre dodan u grupu: {gameSession.Id.ToString()}");
    }
    public async Task InvitePlayer(string invitedPlayerId)
    {
        var userId = Context.User?.FindFirst(ClaimTypes.Name)?.Value;


        var hostGameSession = gameSessionService.GetSessionForUser(userId);

       
        await Clients.User(invitedPlayerId).SendAsync("GameInvitation",
            new { HostId = userId, SessionId = hostGameSession.Id });
    }
    public async Task AcceptInvitation(string hostPlayerId)
    {
        var userId = Context.User?.FindFirst(ClaimTypes.Name)?.Value;

        // Napuštanje trenutne sesije
        var currentSession = gameSessionService.GetSessionForUser(userId);
        if (currentSession != null)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, currentSession.Id.ToString());
            gameSessionService.RemovePlayerFromSession(userId);
        }
        var   sessionId = gameSessionService.GetSessionIdForUser(userId);
        // Pridruživanje novoj sesiji
         gameSessionService.JoinGameSession(hostPlayerId,userId);
        await Groups.AddToGroupAsync(Context.ConnectionId, sessionId.ToString());

        // Obavijest svima u grupi da se pridružio novi igrač
        await Clients.Group(sessionId.ToString()).SendAsync("PlayerJoined", userId);
    }
    //
    public async Task SetGameParameters(int difficulty, int category)
    {
        var userId = Context.User?.FindFirst(ClaimTypes.Name)?.Value;

        // Dobivanje sesije
        var session = gameSessionService.GetSessionForUser(userId);

        // Provjera je li korisnik admin sesije
        if (session.player1.NickName== userId)
        {
            // Postavlja parametre igre
            gameSessionService.SetParametersOfGameSession(userId, category, difficulty);

            // Obavještava sve igrače o promjeni parametara
            await Clients.Group(session.Id.ToString())
                .SendAsync("GameParametersSet", new { Difficulty = difficulty, Category = category });
        }
        else
        {
            // Korisnik nije admin, ne može mijenjati parametre
            await Clients.Caller.SendAsync("Error", "Samo admin može postaviti parametre igre.");
        }
    }
    public async Task SendNextQuestion()
    {
        var userId = Context.User?.FindFirst(ClaimTypes.Name)?.Value;
        var session = gameSessionService.GetSessionForUser(userId);
        var question = await gameSessionService.GetNextQuestion(Context.User.Identity.Name);
        if (question != null)
        {
            await Clients.Group(session.Id.ToString()).SendAsync("ReceiveQuestion", question);
        }
    }
    public async Task SendResultAfterRound( int questionId, int answerIndex)
    {
        var userId = Context.User?.FindFirst(ClaimTypes.Name)?.Value;
        var session = gameSessionService.GetSessionForUser(userId);
        while (!gameSessionService.ArePlayersDone(userId))
        {
          
        }
        await Clients.User(userId).SendAsync("ReceiveAnswer", gameSessionService.GetResultsAfterQuestion(userId));

    }
    public async Task SubmitAnswer( int questionId, int answerIndex)
    {
        var userId = Context.User?.FindFirst(ClaimTypes.Name)?.Value;
        var session = gameSessionService.GetSessionForUser(userId);
        gameSessionService.SubmitAnswerToQuestion(userId, questionId, answerIndex);
        await Clients.Caller.SendAsync("Error", "odgovor primljen i obradjen, cekamo drugog igraca");
    }
 
    public  bool IsGameOver()
    {
        
        return false;
    }



}
