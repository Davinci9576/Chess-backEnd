namespace ChessEngine;

using ChessEngine.Models;
public class CheckDetector
{

    public bool IsKingInCheck(Board board, bool whiteKing)
    {
        if (!TryFindKing(
            board,
            whiteKing,
            out int kingRow,
            out int kingCol
        ))
        {
            throw new InvalidOperationException("King not found on the board.");
        }

        return IsSquareAttacked(board, kingRow, kingCol, !whiteKing);
    }

    public bool TryFindKing(
       Board board,
       bool whiteKing,
       out int kingRow,
       out int kingCol)
    {
        Piece king = whiteKing
        ? Piece.WhiteKing
        : Piece.BlackKing;

        for (int row = 0; row < 8; row++)
        {
            for (int col = 0; col < 8; col++)
            {
                if (board.Squares[row, col] == king)
                {
                    kingRow = row;
                    kingCol = col;
                    return true;
                }
            }
        }
        kingRow = -1;
        kingCol = -1;
        return false;
    }
    private readonly MoveValidator moveValidator = new MoveValidator();
    public bool IsSquareAttacked(
        Board board,
        int targetRow,
        int targetCol,
        bool byWhite
    )
    {
        for (int row = 0; row < 8; row++)
        {
            for (int col = 0; col < 8; col++)
            {
                Piece attacker = board.Squares[row, col];

                if (attacker == Piece.Empty)
                {
                    continue;
                }
                if (PieceHelper.IsWhitePiece(attacker) != byWhite)
                {
                    continue;
                }
                Move hypotheticalMove = new Move(row, col, targetRow, targetCol);

                if (moveValidator.CanAttackSquare(hypotheticalMove, attacker, board))
                {
                    return true;
                }

            }
        }

        return false;
    }

}