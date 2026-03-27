// See https://aka.ms/new-console-template for more information

using MTTT;

class Program
{
    static void Main()
    {
        Guid playerOneId = Guid.NewGuid();
        Guid playerTwoId = Guid.NewGuid();
        const string expectedResponse = "MoveMade";
        Dictionary<int, string> moveMap = new Dictionary<int, string>();
        
        string gameId = PrepareGame.startGame(playerOneId, playerTwoId);
        gameId = gameId.Remove(0, 1);
        gameId = gameId.Remove(gameId.Length - 1, 1);
        
        
        Console.Clear();
        Console.WriteLine("Game Id\n" + gameId + "\n\nPlayer One Id:\n" +  playerOneId + "\n\nPlayer Two Id:\n"+ playerTwoId);
        
        bool isGameOver = false;
        
        for (int i = 0; i < 20; i++)
        {
            if (isGameOver) break;
        
            string move = "";
            string response = "";
        
            //Player 1
            Console.Write($"Round: {i}\nPlayer One");
            while (response != expectedResponse)
            {
                GameDto dto;
                try {
                    dto = Task.Run(() => PrepareGame._GetGameDTO_Actual(gameId)).Result;
                } catch {
                    isGameOver = true; break; 
                }
        
                var moveCoords = BotBrain.ChooseMove(dto);
                response = makeMove.playerMove(gameId, playerOneId.ToString(), moveCoords.x, moveCoords.y);
        
                if (response == "GameOver") { isGameOver = true; break; }
                
                if (response == expectedResponse) {
                    move += $"Player One: X: {moveCoords.x}, Y: {moveCoords.y} | ";
                    Console.Write(" " + response + ": " + moveCoords.x + " " + moveCoords.y + " | ");
                }
            }
            
            if (!isGameOver)
            {
                Thread.Sleep(500);
                response = "";
                Console.Write("Player Two");
                while (response != expectedResponse)
                {
                    int x = makeMove.choseField();
                    int y = makeMove.choseField();
                    response = makeMove.playerMove(gameId, playerTwoId.ToString(), x, y);
        
                    if (response == "GameOver") { isGameOver = true; break; }
                    
                    if (response == expectedResponse) {
                        move += $"Player Two: X: {x}, Y: {y}";
                        Console.WriteLine(" " + response + ": " + x + " " + y);
                    }
                }
            }
            
            moveMap.Add(i, move);
            
            if (isGameOver) break;
            Thread.Sleep(500);
        }
        
        Console.WriteLine("\n SUMMARY");
        foreach (KeyValuePair<int, string> entry in moveMap)
        {
            Console.WriteLine("Round " + entry.Key + ": " + entry.Value);
        }
                
    }    
}



