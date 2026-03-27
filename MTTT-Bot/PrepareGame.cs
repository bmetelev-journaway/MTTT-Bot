using System.Net.Http.Json;
namespace MTTT;

public class PrepareGame
{
    private const string urlJoinGame = "http://localhost:5088/api/Game/lobby/";
    private const string urlLobbyState = "http://localhost:5088/api/Game/";
    
    public static string startGame(Guid playerOneId, Guid playerTwoId)
    {
        string gameId;
        
        Console.WriteLine("Waiting for Players...");
        gameId = Task.Run(() => JoinGame(playerOneId)).Result;
        Console.WriteLine("Checking Lobby State...");
        Task.Run(() => GetGameState(gameId)).Wait();
        
        Console.WriteLine("Waiting for Players...");
        gameId = Task.Run(() => JoinGame(playerTwoId)).Result;
        Console.WriteLine("Checking Lobby State...");
        Task.Run(() => GetGameState(gameId)).Wait();
        
        //Thread.Sleep(500);
        Console.WriteLine("Joined Game");
        Console.WriteLine(gameId);
        Task.Run(() => _GetGameDTO(gameId)).Wait();
        
        return gameId;
    }


    
    private static async Task<string> JoinGame(Guid playerId)
    {
        HttpClient client = new HttpClient();
        HttpResponseMessage response = await client.GetAsync(urlJoinGame + playerId);
        
        string gameId = response.Content.ReadAsStringAsync().Result;
        return gameId;
    }

    private static async Task GetGameState(string gameId)
    {
        HttpClient client = new HttpClient();
        gameId = gameId.Remove(0, 1);
        gameId = gameId.Remove(gameId.Length - 1, 1);
        HttpResponseMessage response = await client.GetAsync(urlLobbyState + gameId + "/lobby/");
    }

    private static async Task _MakeMove(string playerId)
    {
        throw new NotImplementedException();
    }

    private static async Task _GetGameDTO(string gameId)
    {
        HttpClient client = new HttpClient();
        gameId = gameId.Remove(0, 1);
        gameId = gameId.Remove(gameId.Length - 1, 1);
        HttpResponseMessage response = await client.GetAsync(urlLobbyState + gameId);
        Console.WriteLine(response.Content.ReadAsStringAsync().Result);
    }
    
    public static async Task<GameDto> _GetGameDTO_Actual(string gameId)
    {
        HttpClient client = new HttpClient();
        
        var cleanId = gameId.Replace("\"", "");
        return await client.GetFromJsonAsync<GameDto>($"http://localhost:5088/api/Game/{cleanId}");
    }
}