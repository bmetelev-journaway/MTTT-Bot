using MTTT;

class Program
{
    static void Main()
    {
        List<string> failedMoves = new List<string>();
        // 1. Identität festlegen
        Guid myPlayerId = Guid.NewGuid(); 
        const string expectedResponse = "MoveMade";
        bool isGameOver = false;
        GameDto lastValidDto = null;

        // 2. Spiel beitreten
        string gameIdRaw = PrepareGame.startGame(myPlayerId);
        string gameId = gameIdRaw.Replace("\"", "");
        
        Console.WriteLine($"BOT-ALEX GESTARTET\nGame ID: {gameId}\n Alex Player ID: {myPlayerId}\n---");

        // 3. Wait-Schleife
        while (!isGameOver)
        {
            GameDto dto = null;
            try 
            {
                dto = Task.Run(() => PrepareGame._GetGameDTO_Actual(gameId)).Result;
            }
            catch (Exception ex)
            {
                if (isGameOver) {
                    Console.WriteLine("Spiel beendet.");
                } else {
                    Console.WriteLine("Warte darauf, dass der Gegner beitritt oder der Server bereit ist...");
                    Thread.Sleep(2000);
                }
                continue;
            }

            // Mein Zug?
            if (dto.CurrentPlayerId == myPlayerId)
            {
                Console.WriteLine("\n[Alex Zug] Berechne Zug...");
                
                var moveCoords = BotBrain.ChooseMove(dto);
                string moveKey = $"{moveCoords.x}-{moveCoords.y}";
                if (failedMoves.Contains(moveKey))
                {
                    moveCoords = BotBrain.GetAlternativeMove(dto, moveCoords.x, failedMoves);
                }
                string response = makeMove.playerMove(gameId, myPlayerId.ToString(), moveCoords.x, moveCoords.y);

                if (response == "GameOver")
                {
                    isGameOver = true;
                    Console.WriteLine("Letzter Zug gemacht. Spiel vorbei!");
                }
                else if (response == expectedResponse)
                {
                    Console.WriteLine($"Zug ausgeführt: X {moveCoords.x}, Y {moveCoords.y}");
                    failedMoves.Clear();
                }
                else
                {
                    Console.WriteLine($"Server sagt Nein: {response} für {moveCoords.x}/{moveCoords.y}");
                    failedMoves.Add($"{moveCoords.x}-{moveCoords.y}");
                    Thread.Sleep(500);
                }
            }
            else
            {
                Console.Write(".");
                Thread.Sleep(1000); 
            }
        }

        Console.WriteLine("Programm beendet. Ergebnis in Server-Konsole");
    }
}