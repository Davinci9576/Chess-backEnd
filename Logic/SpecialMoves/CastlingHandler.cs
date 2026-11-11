using ChessEngine;
using ChessEngine.Models;

namespace ChessEngine.Logic.SpecialMoves;

public class CastlingHandler
{
    private readonly CheckDetector checkDetector = new CheckDetector();

    public bool IsCastlingMove(Move move, Piece piece)
    {
        return (piece == Piece.WhiteKing || piece == Piece.BlackKing)
            && move.FromRow == move.ToRow
            && Math.Abs(move.ToCol - move.FromCol) == 2;
    }

    public bool TryCastle(Move move, Piece piece, Board board)
    {
        bool white = piece == Piece.WhiteKing;

        int homeRow = white ? 7 : 0;
        bool kingSide = move.ToCol == 6;
        bool queenSide = move.ToCol == 2;

        if (move.FromCol != 4 || move.FromRow != homeRow || move.ToRow != homeRow)
            return false;

        if (!kingSide && !queenSide)
            return false;

        if (white)
        {
            if (kingSide && !board.WhiteKingSideCastle) return false;
            if (queenSide && !board.WhiteQueenSideCastle) return false;
        }
        else
        {
            if (kingSide && !board.BlackKingSideCastle) return false;
            if (queenSide && !board.BlackQueenSideCastle) return false;
        }

        int rookCol = kingSide ? 7 : 0;
        Piece expectedRook = white ? Piece.WhiteRook : Piece.BlackRook;

        if (board.Squares[homeRow, rookCol] != expectedRook)
            return false;

        int[] columnsToCheck = kingSide ?
        new int[] { 2, 3 } :
        new int[] { 5, 6, 7 };


        foreach (int columns in columnsToCheck)
        {
            if (board.Squares[homeRow, columns] != Piece.Empty)
            {
                return false;
            }
        }

        bool enemyIsWhite = !white;

        if (checkDetector.IsSquareAttacked(board, homeRow, 4, enemyIsWhite))
            return false;

        int transitCol = kingSide ? 5 : 3;

        if (checkDetector.IsSquareAttacked(board, homeRow, transitCol, enemyIsWhite))
            return false;

        if (checkDetector.IsSquareAttacked(board, homeRow, move.ToCol, enemyIsWhite))
            return false;

        board.Squares[homeRow, move.FromCol] = Piece.Empty;
        board.Squares[homeRow, move.ToCol] = piece;

        int rookDestination = kingSide ? 5 : 3;

        board.Squares[homeRow, rookDestination] = expectedRook;
        board.Squares[homeRow, rookCol] = Piece.Empty;

        board.UpdateCastlingRights(move, piece, Piece.Empty);
        board.EnPassantTarget = null;

        return true;
    }
}