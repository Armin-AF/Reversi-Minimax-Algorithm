using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class AI
{
    public static Index CalculateNextMove(TileState[,] board, TileState color, int depth)
    {
        var alpha = int.MinValue;
        var beta = int.MaxValue;
        List<Index> availableMoves = GameRules.GetPlayableTiles(board, color);
        if (availableMoves.Count == 0)
        {
            return new Index(board.GetLength(0), board.GetLength(1));
        }

        Index toReturn = availableMoves[0];
       
        for (int i = 1; i < availableMoves.Count; i++)
        {
            switch (color)
            {
                case TileState.Black:
                    int maxValue = MiniMaxAlgorithm(GameRules.SimulateTurn(board, availableMoves[0], color), TileState.White, depth - 1, alpha, beta);
                    if (MiniMaxAlgorithm(GameRules.SimulateTurn(board, availableMoves[i], color), TileState.White, depth - 1, alpha, beta) > maxValue)
                    {
                        toReturn = availableMoves[i];
                    }
                    break;
                case TileState.White:
                    int minValue = MiniMaxAlgorithm(GameRules.SimulateTurn(board, availableMoves[0], color), TileState.Black, depth - 1, alpha, beta);
                    if (MiniMaxAlgorithm(GameRules.SimulateTurn(board, availableMoves[i], color), TileState.Black, depth - 1, alpha, beta) < minValue)
                    {
                        toReturn = availableMoves[i];
                    }
                    break;
                case TileState.Empty:
                    break;
            }
        }

        return toReturn;
    }

    private static int MiniMaxAlgorithm(TileState[,] board, TileState color, int depth, int alpha, int beta)
    {
        List<Index> playableMoves = GameRules.GetPlayableTiles(board, color);
        if (depth <= 0 || playableMoves.Count <= 0) return AnalyzeGame(board);
        switch (color)
        {
            case TileState.Black:
                var maxValue = int.MinValue;
                foreach (var eval in playableMoves.Select(move => MiniMaxAlgorithm(GameRules.SimulateTurn(board, move, color), TileState.White, depth - 1, alpha, beta)))
                {
                    maxValue = Mathf.Max(maxValue, eval);
                    alpha = Mathf.Max(alpha, eval);
                    if (beta <= alpha)
                    {
                        break;
                    }
                }
                return maxValue;

            case TileState.White:
                var minValue = int.MaxValue;
                foreach (var eval in playableMoves.Select(move => MiniMaxAlgorithm(GameRules.SimulateTurn(board, move, color), TileState.Black, depth - 1, alpha, beta)))
                {
                    maxValue = Mathf.Min(minValue, eval);
                    beta = Mathf.Min(beta, eval);
                    if (beta <= alpha)
                    {
                        break;
                    }
                }
                return minValue;
            case TileState.Empty:
                break;
        }
        return 1;
    }

    private static int AnalyzeGame(TileState[,] board)
    {
        var numberOfWhitePieces = 0; 
        var numberOfBlackPieces = 0;

        for (int i = 0; i < board.GetLength(1); i++)
        {
            for (int j = 0; j < board.GetLength(0); j++)
            {
                if (board[j, i] != TileState.Black)
                {
                    if (board[j, i] == TileState.White)
                    {
                        numberOfWhitePieces++;
                    }
                }
                else
                {
                    numberOfBlackPieces++;
                }
            }
        }

        var evalValue = numberOfBlackPieces - numberOfWhitePieces;

        return evalValue;
    }
}