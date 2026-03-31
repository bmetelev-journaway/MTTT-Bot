using System.Text;
using System.Text.Json;

namespace MTTT;

public class makeMove
{
    private const string postUrl = "http://localhost:5088/api/Game/";
    
    public static string playerMove(string gameId, string playerId, int x, int y)
    {
        string url = postUrl + gameId + "/moves";
        MoveDto moveDto = new MoveDto
        {
            GameId = gameId,
            PlayerId = playerId,
            X = x,
            Y = y
        };
        
        string moveResult = Task.Run(() => sendMove(url, moveDto)).Result;
        return moveResult;
    }
    
    private static async Task<string> sendMove(string url, MoveDto moveDto)
    {
        HttpClient client = new HttpClient();
        String payload = JsonSerializer.Serialize(moveDto);
        HttpContent content = new StringContent(payload, Encoding.UTF8, "application/json");
    
        HttpResponseMessage response = await client.PostAsync(url, content);
        
        if (!response.IsSuccessStatusCode) {
            string error = await response.Content.ReadAsStringAsync();
            return string.IsNullOrEmpty(error) ? "Error" : error;
        }

        return await response.Content.ReadAsStringAsync();
    }
}