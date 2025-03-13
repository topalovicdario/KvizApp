using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using ServerKVIZ.Models;
using ServerKVIZ.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Threading.Tasks;
//potrebno testirati ovo

[Authorize]
public class GameHub : Hub
{
   

    public readonly IGameSessionService gameSessionService;

    private static Timer timer;
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
        
        await Clients.Caller.SendAsync("LobbyCreated", gameSession.Id);
        await GetActiveUsers();
        Console.WriteLine("Kreirao si sesiju: "+gameSession.Id+" ,host name : "+gameSession.player1.NickName);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        // Uklanjanje korisnika iz liste aktivnih pri prekidu konekcije
       activePlayers.TryRemove(Context.ConnectionId, out var userName);
       
       



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

        

        Console.WriteLine($"Korisnik {userName} prekinuo konekciju i uklonjen iz grupa.");
       
        await base.OnDisconnectedAsync(exception);
    }


    public async Task GetActiveUsers()
    {
        // Pravimo listu jedinstvenih imena igrača
        var users = activePlayers.Values.Distinct().ToList();

        // Ispis u konzolu za proveru
        Console.WriteLine("Aktivni igrači: " +string.Join(" ",users));

        // Slanje liste svih aktivnih igrača svim klijentima
        await Clients.All.SendAsync("ActivePlayers", users);

        
    }
    public async Task GetCategories()
    {
        var categories = await gameSessionService.GetCategories();
        await Clients.All.SendAsync("GetCategories", categories);
    }



   
    public async Task InvitePlayer(string invitedPlayerId)
    {
       
        var userId = Context.User?.FindFirst(ClaimTypes.Name)?.Value;
        Console.WriteLine($"Invite player {invitedPlayerId}");

        var hostGameSession = gameSessionService.GetSessionForUser(userId);
        Console.WriteLine("Gamesesija " + hostGameSession.Id+ ", ciji je host "+hostGameSession.player1.NickName);
       
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
        var   session = gameSessionService.GetSessionForUser(hostPlayerId);
        Console.WriteLine("Prihvacen invite od "+userId+" za game invite od "+hostPlayerId +", stoga je dodan u lobby "+
            session.Id.ToString());
        // Pridruživanje novoj sesiji
         gameSessionService.JoinGameSession(hostPlayerId,userId);
        await Groups.AddToGroupAsync(Context.ConnectionId, session.Id.ToString());

        // Obavijest svima u grupi da se pridružio novi igrač
        await Clients.Group(session.Id.ToString()).SendAsync("PlayerJoined", userId);
    }
    //
    public async Task SetGameParameters( int category,int difficulty)
    {
        Console.WriteLine($"{category} {difficulty}");
       
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
                .SendAsync("GameParametersSet", "Dodani parametri");
            
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
        
        if (!await GameOver(userId))
        {
var session = gameSessionService.GetSessionForUser(userId);
        var question = await gameSessionService.GetNextQuestion(userId);
       
            if (question != null)
        {
            Console.WriteLine(question.Text);
            await Clients.Group(session.Id.ToString()).SendAsync("ReceiveQuestion", question);
        }
        }
        
    }
    public async Task SendResultAfterRound( )
    {
        var userId = Context.User?.FindFirst(ClaimTypes.Name)?.Value;
        var session = gameSessionService.GetSessionForUser(userId);
        while (!gameSessionService.ArePlayersDone(userId))
        {
            Console.WriteLine("nisu svi jos odgovorili");
        }
        
        Console.WriteLine("ovo saljem kljijentu: "+gameSessionService.GetResultsAfterQuestion(userId).PlayerScore +":"+gameSessionService.GetResultsAfterQuestion(userId).EnemyScore);
        
        await Clients.Caller.SendAsync("ReceiveAnswer", gameSessionService.GetResultsAfterQuestion(userId));

    }
    public async Task SubmitAnswer( int questionId, int answerIndex)
    {
        var userId = Context.User?.FindFirst(ClaimTypes.Name)?.Value;
        var session = gameSessionService.GetSessionForUser(userId);
        gameSessionService.SubmitAnswerToQuestion(userId, questionId, answerIndex);
        await Clients.Caller.SendAsync("Error", "odgovor primljen i obradjen, cekamo drugog igraca");
    }
 
    public  async Task<bool>  GameOver(string userId)
    {
      
        var session = gameSessionService.GetSessionForUser(userId);

        if (gameSessionService.IsGameOver(userId))
        {
            
await Clients.Group(session.Id.ToString()).SendAsync("GameOver", "Kraj igre");
            return true;
        }
        else
        {
            return false;
        }
        
        

    }



}
