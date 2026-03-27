using MTTT;

class Program
{
    static void Main()
    {
        // 1. Identität festlegen
        Guid myPlayerId = Guid.NewGuid(); 
        const string expectedResponse = "MoveMade";
        bool isGameOver = false;
        GameDto lastValidDto = null;

        // 2. Spiel beitreten
        string gameIdRaw = PrepareGame.startGame(myPlayerId);
        string gameId = gameIdRaw.Replace("\"", "");

        Console.Clear();
        Console.WriteLine($"BOT-ALEX GESTARTET\nGame ID: {gameId}\n Alex Player ID: {myPlayerId}\n---");

        // 3. Wait-Schleife
        while (!isGameOver)
        {
            GameDto dto;
            try 
            {
                dto = Task.Run(() => PrepareGame._GetGameDTO_Actual(gameId)).Result;
                lastValidDto = dto;
            }
            catch (Exception) 
            {
                isGameOver = true;
                break;
            }

            // Mein Zug?
            if (dto.CurrentPlayerId == myPlayerId)
            {
                Console.WriteLine("\n[Alex Zug] Berechne Zug...");
                
                var moveCoords = BotBrain.ChooseMove(dto);
                string response = makeMove.playerMove(gameId, myPlayerId.ToString(), moveCoords.x, moveCoords.y);

                if (response == "GameOver")
                {
                    isGameOver = true;
                    Console.WriteLine("Letzter Zug gemacht. Spiel vorbei!");
                }
                else if (response == expectedResponse)
                {
                    Console.WriteLine($"Zug ausgeführt: X {moveCoords.x}, Y {moveCoords.y}");
                }
                else
                {
                    Console.WriteLine($"Server-Antwort: {response}");
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