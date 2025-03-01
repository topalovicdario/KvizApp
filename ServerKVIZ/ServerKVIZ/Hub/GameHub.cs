using MySqlX.XDevAPI;
using ServerKVIZ.Models;

using ServerKVIZ.Services;
using Microsoft.AspNetCore.SignalR;
using MySqlX.XDevAPI;
using ServerKVIZ.Models;
using ServerKVIZ.Services;
using System.Collections.Concurrent;
using ServerKVIZ.Controllers;
//jos uvijek u postupku implementacije, zanemarite ovaj kod
public class GameHub : Hub
{
    private readonly ConcurrentDictionary<string, HashSet<string>> _groupConnections = new();

    private readonly ConcurrentDictionary<string, Dictionary<string, int>> _playerAnswers = new();

    private readonly GameSession _gameSession;
    private static readonly ConcurrentDictionary<string, string> ActiveUsers = new();

    public override async Task OnConnectedAsync()
    {
        // Dohvaćanje korisničkog imena iz tokena
        var userName = Context.User?.Identity?.Name;
        if (!string.IsNullOrEmpty(userName))
        {
            // Spremanje aktivnog korisnika. Ključom je ConnectionId, a vrijednost je korisničko ime.
            ActiveUsers[Context.ConnectionId] = userName;
        }

       
        await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userName}");

        Console.WriteLine($"Korisnik {userName} spojen s Connection ID: {Context.ConnectionId}");
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        // Uklanjanje korisnika iz liste aktivnih pri prekidu konekcije
        ActiveUsers.TryRemove(Context.ConnectionId, out var userName);
        Console.WriteLine($"Korisnik {userName} prekinuo konekciju: {Context.ConnectionId}");
        await base.OnDisconnectedAsync(exception);
        

      
        foreach (var group in _groupConnections.Keys)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, group);
        }

        Console.WriteLine($"Korisnik {userName} prekinuo konekciju i uklonjen iz grupa.");
        await base.OnDisconnectedAsync(exception);
    }

 
    public Task<List<string>> GetActiveUsers()
    {
        // Vraća listu jedinstvenih imena iz ActiveUsers kolekcije
        var users = ActiveUsers.Values.Distinct().ToList();
        return Task.FromResult(users);
    }
    public GameHub(GameSession gameSession)
    {
        _gameSession = gameSession;
    }

    public async Task StartGame(string gameSessionId, string category, string difficulty)
    {
        _gameSession.Init(category, difficulty);
        await _gameSession.StoreQuestions();

        
        await Groups.AddToGroupAsync(Context.ConnectionId, gameSessionId);

        Console.WriteLine($"Kreator igre dodan u grupu: {gameSessionId}");
    }
    public async Task JoinGameSession(string gameSessionId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, gameSessionId);
        Console.WriteLine($"Korisnik {Context.ConnectionId} pridružen grupi: {gameSessionId}");
    }
    public async Task SendNextQuestion(string gameSessionId)
    {
        var question = await _gameSession.GetQuestion();
        if (question != null)
        {
            await Clients.Group(gameSessionId).SendAsync("ReceiveQuestion", question);
        }
    }
    private void AddToGroup(string groupName, string connectionId)
    {
        if (!_groupConnections.ContainsKey(groupName))
        {
            _groupConnections[groupName] = new HashSet<string>();
        }

        lock (_groupConnections[groupName])
        {
            _groupConnections[groupName].Add(connectionId);
        }
    }

    private void RemoveFromGroup(string groupName, string connectionId)
    {
        if (_groupConnections.ContainsKey(groupName))
        {
            lock (_groupConnections[groupName])
            {
                _groupConnections[groupName].Remove(connectionId);

                if (_groupConnections[groupName].Count == 0)
                {
                    _groupConnections.TryRemove(groupName, out _);
                }
            }
        }
    }

    private List<string> GetGroupConnections(string groupName)
    {
        if (_groupConnections.TryGetValue(groupName, out var connections))
        {
            return connections.ToList();
        }

        return new List<string>();
    }

    public async Task SubmitAnswer(string gameSessionId, int questionId, int answerIndex)
    {
     
        if (!_playerAnswers.ContainsKey(gameSessionId))
        {
            _playerAnswers[gameSessionId] = new Dictionary<string, int>();
        }

        var playerAnswers = _playerAnswers[gameSessionId];

        lock (playerAnswers)
        {
            playerAnswers[Context.ConnectionId] = answerIndex;
        }

        Console.WriteLine($"Player {Context.ConnectionId} submitted answer: {answerIndex}");

       
        var groupConnections = GetGroupConnections(gameSessionId);

        if (playerAnswers.Count == groupConnections.Count)
        {
            Console.WriteLine("All players have answered. Processing results...");

           
            var results = ProcessAnswers(gameSessionId, questionId);

           
            await Clients.Group(gameSessionId).SendAsync("QuestionResults", results);

            
            if (IsGameOver())
            {
                Console.WriteLine("Game over!");

                
                await Clients.Group(gameSessionId).SendAsync("GameOver");

              
                CleanupGameSession(gameSessionId);

                return;
            }

          
            lock (playerAnswers)
            {
                playerAnswers.Clear();
            }

            Console.WriteLine("Results sent to all players.");
        }
    }
    private void CleanupGameSession(string gameSessionId)
    {
        _playerAnswers.TryRemove(gameSessionId, out _);
        _groupConnections.TryRemove(gameSessionId, out _);
    }


    private List<SendAnswerCheckQuestion> ProcessAnswers(string gameSessionId, int questionId)
    {
        var results = new List<SendAnswerCheckQuestion>();

        foreach (var playerAnswer in _playerAnswers[gameSessionId])
        {
            var connectionId = playerAnswer.Key;
            var answerIndex = playerAnswer.Value;

           
            var question = _gameSession.GetById(questionId);
            if (question == null)
            {
                throw new Exception("Question not found.");
            }

            
            var result = new SendAnswerCheckQuestion
            {
                corect_ans = question.CorrectAnswer,
                is_correct = answerIndex > 0 && question.CorrectAnswer == question.AllAnswers[answerIndex - 1],
                score = _gameSession.player1.Score,
                enemy_score = _gameSession.player2.Score
            };

            if (result.is_correct)
            {
                result.score += 1; 
            }

            results.Add(result);
        }

        return results;
    }

    private bool IsGameOver()
    {
        
        return false;
    }



}
