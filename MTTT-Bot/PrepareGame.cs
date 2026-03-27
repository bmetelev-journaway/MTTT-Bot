using System.Net.Http.Json;
namespace MTTT;

public class PrepareGame
{
    private const string urlJoinGame = "http://localhost:5088/api/Game/lobby/";
    private const string urlLobbyState = "http://localhost:5088/api/Game/";
    
    public static string startGame(Guid myPlayerId)
    {
        string gameId;
    
        Console.WriteLine("Suche nach Mitspieler (Lobby)...");
        
        gameId = Task.Run(() => JoinGame(myPlayerId)).Result;
        gameId = gameId.Trim('"');

        Console.WriteLine("Lobby beigetreten. Warte auf Spielstart...");
        
        Task.Run(() => GetGameState(gameId)).Wait();
    
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