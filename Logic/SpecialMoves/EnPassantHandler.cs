using System.Security.Cryptography.X509Certificates;
using ChessEngine;
using ChessEngine.Models;

namespace ChessEngine.Logic.SpecialMoves;

public class EnPassantHandler
{
    public bool IsEnPassantMove(Move move, Piece piece, Board board)
    {
        return (piece == Piece.WhitePawn || piece == Piece.BlackPawn)
        && board.EnPassantTarget == (move.ToRow, move.ToCol)
        && board.Squares[move.ToRow, move.ToCol] == Piece.Empty
        && move.FromCol != move.ToCol;
    }

    public void ExecutionEnPassant(Move move, Board board)
    {
        board.Squares[move.FromRow, move.ToCol] = Piece.Empty;
    }
    public void RestoreCapturedPawn(Move move, Piece piece, Board board)
    {
        board.Squares[move.FromRow, move.ToCol] =
        piece == Piece.WhitePawn ? Piece.BlackPawn : Piece.WhitePawn;
    }
    public void UpdateEnPassantTarget(Move move, Piece piece, Board board)
    {
        board.EnPassantTarget = null;
        if (piece == Piece.WhitePawn && move.FromRow == 6 && move.ToRow == 4)
        {
            board.EnPassantTarget = (5, move.FromCol);
        }
        if (piece == Piece.BlackPawn && move.FromRow == 1 && move.ToRow == 3)
        {
            board.EnPassantTarget = (2, move.FromCol);
        }
    }
}