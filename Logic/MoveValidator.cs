using ChessEngine.Models;

namespace ChessEngine;

public class MoveValidator
{

    public bool IsValidMove(
        Move move,
        Piece piece,
        Board board,
        Piece target
    )
    {
        if (!IsInsideBoard(move.FromRow, move.FromCol) || !IsInsideBoard(move.ToRow, move.ToCol))
        {
            return false;
        }
        if (piece == Piece.Empty || board.Squares[move.FromRow, move.FromCol] != piece)
        {

            return false;
        }
        if (target != Piece.Empty && PieceHelper.IsWhitePiece(piece) == PieceHelper.IsWhitePiece(target))
        {
            return false;
        }
        int rowDifference = Math.Abs(move.ToRow - move.FromRow);
        int colDifference = Math.Abs(move.ToCol - move.FromCol);

        if (IsPawn(piece))
        {
            int direction = piece == Piece.WhitePawn ? -1 : 1;
            int startRow = piece == Piece.WhitePawn ? 6 : 1;
            bool white = piece == Piece.WhitePawn;

            int rowMove = move.ToRow - move.FromRow;

            if (move.FromCol == move.ToCol && rowMove == direction && target == Piece.Empty)
            {
                return true;
            }
            int intermediateRow =
                move.FromRow + direction;

            if (move.FromRow == startRow &&
                move.FromCol == move.ToCol &&
                rowMove == 2 * direction &&
                target == Piece.Empty &&
                board.Squares[intermediateRow, move.FromCol]
                    == Piece.Empty)
            {
                return true;
            }
            if (Math.Abs(move.ToCol - move.FromCol) == 1 && move.ToRow - move.FromRow == direction)
            {
                if (target != Piece.Empty)
                {
                    if (target == Piece.WhiteKing || target == Piece.BlackKing)
                    {
                        return false;
                    }
                    return white != PieceHelper.IsWhitePiece(target);
                }
                if (board.EnPassantTarget == (move.ToRow, move.ToCol))
                {
                    Piece capturedPawn = board.Squares[move.FromRow, move.ToCol];

                    return capturedPawn == (white ? Piece.BlackPawn : Piece.WhitePawn);
                }
            }

            return false;
        }
        if (!IsValidMovementPattern(move, piece))
        {
            return false;
        }
        if (IsSlidingPiece(piece) && !IsPathClear(move, board))
        {
            return false;
        }
        return true;
    }

    public bool IsValidMovementPattern(
        Move move,
        Piece piece
    )
    {
        if (!IsInsideBoard(move.FromRow, move.FromCol) || !IsInsideBoard(move.ToRow, move.ToCol))
        {
            return false;
        }
        int colDifference = Math.Abs(move.ToCol - move.FromCol);
        int rowDifference = Math.Abs(move.ToRow - move.FromRow);

        bool diagnole = colDifference == rowDifference && rowDifference != 0;
        bool straight = (colDifference == 0 || rowDifference == 0) && (rowDifference != 0 || colDifference != 0);

        if (IsKnight(piece))
        {
            if ((rowDifference == 2 && colDifference == 1) || (rowDifference == 1 && colDifference == 2))
            {
                return true;
            }
        }
        if (IsQueen(piece))
        {
            return diagnole || straight;
        }
        if (IsBishop(piece))
        {
            return diagnole;
        }
        if (IsRook(piece))
        {
            return straight;
        }
        if (IsKing(piece))
        {
            return rowDifference <= 1 && colDifference <= 1 && (rowDifference != 0 || colDifference != 0);
        }
        return false;
    }
    private static bool IsInsideBoard(int row, int col)
    {
        return row < 8 && row >= 0 && col < 8 && col >= 0;
    }
    private static bool IsPawn(Piece piece)
    {
        return piece == Piece.WhitePawn || piece == Piece.BlackPawn;
    }
    private static bool IsKnight(Piece piece)
    {
        return piece == Piece.WhiteKnight || piece == Piece.BlackKnight;
    }
    private static bool IsBishop(Piece piece) =>
        piece == Piece.WhiteBishop ||
        piece == Piece.BlackBishop;

    private static bool IsRook(Piece piece) =>
        piece == Piece.WhiteRook ||
        piece == Piece.BlackRook;

    private static bool IsQueen(Piece piece) =>
        piece == Piece.WhiteQueen ||
        piece == Piece.BlackQueen;

    private static bool IsKing(Piece piece) =>
        piece == Piece.WhiteKing ||
        piece == Piece.BlackKing;

    public static bool IsSlidingPiece(Piece piece)
    {
        return
        IsBishop(piece) ||
        IsRook(piece) ||
        IsQueen(piece);
    }
    public static bool IsPathClear(Move move, Board board)
    {
        int rowDirection = Math.Sign(move.ToRow - move.FromRow);
        int colDirection = Math.Sign(move.ToCol - move.FromCol);

        int currentRow = move.FromRow + rowDirection;
        int currentCol = move.FromCol + colDirection;

        while (currentRow != move.ToRow || currentCol != move.ToCol)
        {
            if (board.Squares[currentRow, currentCol] != Piece.Empty)
            {
                return false;
            }
            currentRow += rowDirection;
            currentCol += colDirection;
        }
        return true;
    }
    public bool CanAttackSquare(
        Move move,
        Piece piece,
        Board board
    )
    {
        if (!IsInsideBoard(move.FromRow, move.FromCol) || !IsInsideBoard(move.ToRow, move.ToCol))
        {
            return false;
        }
        if (piece == Piece.Empty)
        {
            return false;
        }
        int colDifference = Math.Abs(move.FromCol - move.ToCol);

        if (IsPawn(piece))
        {
            int direction = piece == Piece.WhitePawn ? -1 : 1;

            return move.ToRow - move.FromRow == direction && (colDifference == 1);
        }
        if (!IsValidMovementPattern(move, piece))
        {
            return false;
        }
        if (IsSlidingPiece(piece) && !IsPathClear(move, board))
        {
            return false;
        }
        return true;
    }

}