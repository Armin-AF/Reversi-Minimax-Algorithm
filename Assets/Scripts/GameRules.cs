using System;
using System.Collections.Generic;

public static class GameRules
{
    public static bool IsBoardPlayable(TileState[,] board, TileState color)
    {
        for (var i = 0; i < board.GetLength(1); i++)
        {
            for (var j = board.GetLength(0) - 1; j >= 0; j--)
            {
                if (IsPlayable(board, new Index(j, i), color)) return true;
            }
        }
        return false;
    }
    public static List<Index> GetPlayableTiles(TileState[,] board, TileState color)
    {
        var playablePieces = new List<Index>();
        for (var i = 0; i < board.GetLength(1); i++)
        {
            for (var j = board.GetLength(0) - 1; j >= 0; j--)
            {
                if (IsPlayable(board, new Index(j, i), color)) playablePieces.Add(new Index(j, i));
            }
        }
        return playablePieces;
    }
    
    public static bool IsPlayable(TileState[,] board, Index index, TileState turn)
    {
        if (!CheckBorders(board, index)) return false;

        if (IsPlayable(board, index, turn, 0, 1, 1)) return true;
        if (IsPlayable(board, index, turn, 1, 1, 1)) return true;
        if (IsPlayable(board, index, turn, 1, 0, 1)) return true;
        if (IsPlayable(board, index, turn, 1, -1, 1)) return true;
        if (IsPlayable(board, index, turn, 0, -1, 1)) return true;
        if (IsPlayable(board, index, turn, -1, -1, 1)) return true;
        return IsPlayable(board, index, turn, -1, 0, 1) || IsPlayable(board, index, turn, -1, 1, 1);
    }

    private static bool CheckBorders(TileState[,] board, Index index)
    {
        if (index.Z >= board.GetLength(0)) return false;
        if (index.X >= board.GetLength(1)) return false;
        if (board[index.Z, index.X] != TileState.Empty) return false;
        return true;
    }

    private static bool IsPlayable(TileState[,] board, Index index, TileState turn, int directionZ, int directionX, int depth) //Overload
    {
        if (index.Z + (directionZ * depth) >= board.GetLength(0)) return false;
        if (index.X + (directionX * depth) >= board.GetLength(1)) return false;
        if (index.Z + (directionZ * depth) < 0) return false;
        if (index.X + (directionX * depth) < 0) return false;
        return CheckTurn(board, index, turn, directionZ, directionX, depth);
    }

    private static bool CheckTurn(TileState[,] board, Index index, TileState turn, int directionZ, int directionX,
        int depth)
    {
        switch (turn)
        {
            case TileState.Black:
                return board[index.Z + (directionZ * depth), index.X + (directionX * depth)] switch
                {
                    TileState.Black when depth > 1 => true,
                    TileState.White => IsPlayable(board, index, turn, directionZ, directionX, ++depth),
                    _ => false
                };
            case TileState.White:
                return board[index.Z + (directionZ * depth), index.X + (directionX * depth)] switch
                {
                    TileState.White when depth > 1 => true,
                    TileState.Black => IsPlayable(board, index, turn, directionZ, directionX, ++depth),
                    _ => false
                };
            case TileState.Empty:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(turn), turn, null);
        }

        return false;
    }

    public static TileState[,] SimulateTurn(TileState[,] board, Index index, TileState turnTo)
    {
        TileState[,] result = CopyBoard(board);
        SimulateLane(result, index, turnTo, 0, 1, 1);
        SimulateLane(result, index, turnTo, 1, 1, 1);
        SimulateLane(result, index, turnTo, 1, 0, 1);
        SimulateLane(result, index, turnTo, 1, -1, 1);
        SimulateLane(result, index, turnTo, 0, -1, 1);
        SimulateLane(result, index, turnTo, -1, -1, 1);
        SimulateLane(result, index, turnTo, -1, 0, 1);
        SimulateLane(result, index, turnTo, -1, 1, 1);
        return result;
    }

    private static TileState[,] CopyBoard(TileState[,] board)
    {
        TileState[,] result = new TileState[board.GetLength(0), board.GetLength(1)];
        for (int i = 0; i < board.GetLength(1); i++)
        {
            for (int j = 0; j < board.GetLength(0); j++)
            {
                result[j, i] = board[j, i];
            }
        }
        return result;
    }


    private static bool SimulateLane(TileState[,] board, Index index, TileState turnTo, int directionZ, int directionX, int depth)
    {

        var z = index.Z + (directionZ * depth);
        var x = index.X + (directionX * depth);
        
        if (z >= board.GetLength(0)) return false;
        if (x >= board.GetLength(1)) return false;
        
        if (z < 0) return false;
        if (x < 0) return false;
        
        switch (turnTo)
        {
            case TileState.Black:
                switch (board[z, x])
                {
                    case TileState.Empty:
                        return false;
                    case TileState.Black when depth > 1:
                        return true;
                }

                if (board[z, x] == TileState.White)
                {
                    if (!SimulateLane(board, index, turnTo, directionZ, directionX, ++depth)) return false;
                    board[z, x] = TileState.Black;
                    return true;

                }
                break;
            
            case TileState.White:
                switch (board[z, x])
                {
                    case TileState.Empty:
                        return false;
                    case TileState.White when depth > 1:
                        return true;
                }
                if (board[z, x] == TileState.Black)
                {
                    if (!SimulateLane(board, index, turnTo, directionZ, directionX, ++depth)) return false;
                    board[z, x] = TileState.White;
                    return true;

                }
                break;
            case TileState.Empty:
                break;
        }
        return false;
    }
}