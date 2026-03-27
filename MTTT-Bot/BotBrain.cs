namespace MTTT;

public static class BotBrain
{
    private static readonly int[][] WinningPatterns = new[]
    {
        new[] {0, 1, 2}, new[] {3, 4, 5}, new[] {6, 7, 8},
        new[] {0, 3, 6}, new[] {1, 4, 7}, new[] {2, 5, 8},
        new[] {0, 4, 8}, new[] {2, 4, 6}
    };

    public static (int x, int y) ChooseMove(GameDto dto)
    {
        char[,] board = ParseField(dto.PlayField);
        char mySymbol = (dto.PlayerIds[0] == dto.CurrentPlayerId) ? 'X' : 'O';
        char oppSymbol = (mySymbol == 'X') ? 'O' : 'X';
        
        int targetBigField = dto.NextFieldIndicator;
        
        if (targetBigField == -1 || dto.WinningField[targetBigField] != ' ')
        {
            for (int i = 0; i < 9; i++)
            {
                if (dto.WinningField[i] == ' ')
                {
                    targetBigField = i;
                    break;
                }
            }
        }
        

        //Regel 1: Prüfe ob ich selbst 3 in eine Reihe kriege
        int? winningCell = FindCompletingMove(board, targetBigField, mySymbol);
        if (winningCell.HasValue)
        {
            return (targetBigField, winningCell.Value);
        }

        //Regel 2: Prüfe ob der Gegner im nächsten Zug 3 in eine Reihe kriegen würde
        int? blockingCell = FindCompletingMove(board, targetBigField, oppSymbol);
        if (blockingCell.HasValue)
        {
            return (targetBigField, blockingCell.Value);
        }

        //fallback
        for (int cellIndex = 0; cellIndex < 9; cellIndex++)
        {
            if (board[targetBigField, cellIndex] == ' ') return (targetBigField, cellIndex);
        }

        return (0, 0);
    }

    private static int? FindCompletingMove(char[,] board, int boardIdx, char symbol)
    {
        foreach (var pattern in WinningPatterns)
        {
            int countSymbol = 0;
            int emptyIndex = -1;

            foreach (int pos in pattern)
            {
                if (board[boardIdx, pos] == symbol) countSymbol++;
                else if (board[boardIdx, pos] == ' ') emptyIndex = pos;
                else if (board[boardIdx, pos] != symbol) { 
                    countSymbol = -10; 
                }
            }

            if (countSymbol == 2 && emptyIndex != -1)
            {
                return emptyIndex;
            }
        }
        return null;
    }
    
    public static char[,] ParseField(List<string> lines)
    {
        char[,] grid = new char[9, 9];
        for (int i = 0; i < 9; i++)
        for (int j = 0; j < 9; j++)
            grid[i, j] = ' ';

        int logicalRow = 0;
        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line) || line.Contains("-") || line.Contains("_")) 
                continue;
            
            if (line.Length < 23) continue; 

            int k = logicalRow / 3;
            int i = logicalRow % 3;

            try {
                // Block 1 (Links)
                grid[3 * k, 3 * i + 0] = line[0]; 
                grid[3 * k, 3 * i + 1] = line[2]; 
                grid[3 * k, 3 * i + 2] = line[4];

                // Block 2 (Mitte) Sicherstellen dass Index 9 existiert
                grid[3 * k + 1, 3 * i + 0] = line[9]; 
                grid[3 * k + 1, 3 * i + 1] = line[11]; 
                grid[3 * k + 1, 3 * i + 2] = line[13];

                // Block 3 (Rechts) Sicherstellen dass Index 18 existiert
                grid[3 * k + 2, 3 * i + 0] = line[18]; 
                grid[3 * k + 2, 3 * i + 1] = line[20]; 
                grid[3 * k + 2, 3 * i + 2] = line[22];

                logicalRow++;
            } catch (IndexOutOfRangeException) {
                continue;
            }

            if (logicalRow >= 9) break;
        }
        return grid;
    }
    
    public static (int x, int y) GetAlternativeMove(GameDto dto, int bigField, List<string> blacklist)
    {
        char[,] board = ParseField(dto.PlayField);
        for (int y = 0; y < 9; y++)
        {
            if (board[bigField, y] == ' ' && !blacklist.Contains($"{bigField}-{y}"))
                return (bigField, y);
        }
        return (bigField, 0);
    }
}