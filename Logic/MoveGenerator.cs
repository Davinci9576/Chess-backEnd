using ChessEngine.Models;

namespace ChessEngine;

public class MoveGenerator
{
    private readonly Board board;

    public MoveGenerator(Board board)
    {
        this.board = board;
    }

    public bool HasAnyLegalMove(bool white)
    {
        for (int fromRow = 0; fromRow < 8; fromRow++)
        {
            for (int fromCol = 0; fromCol < 8; fromCol++)
            {
                Piece piece = board.Squares[fromRow, fromCol];

                if (piece == Piece.Empty)
                    continue;

                if (PieceHelper.IsWhitePiece(piece) != white)
                    continue;

                for (int toRow = 0; toRow < 8; toRow++)
                {
                    for (int toCol = 0; toCol < 8; toCol++)
                    {
                        bool isPromotion =
                            (piece == Piece.WhitePawn && toRow == 0) ||
                            (piece == Piece.BlackPawn && toRow == 7);

                        if (isPromotion)
                        {
                            if (HasLegalPromotion(
                                fromRow, fromCol, toRow, toCol, white))
                                return true;
                        }
                        else
                        {
                            Move move = new Move(
                                fromRow, fromCol, toRow, toCol);

                            if (board.TryMoveForSimulation(move))
                                return true;
                        }
                    }
                }
            }
        }

        return false;
    }

    private bool HasLegalPromotion(
        int fromRow,
        int fromCol,
        int toRow,
        int toCol,
        bool white)
    {
        Piece[] promotionPieces = white
            ? new Piece[]
            {
                Piece.WhiteQueen,
                Piece.WhiteBishop,
                Piece.WhiteKnight,
                Piece.WhiteRook
            }
            : new Piece[]
            {
                Piece.BlackQueen,
                Piece.BlackBishop,
                Piece.BlackKnight,
                Piece.BlackRook
            };

        foreach (Piece promotionPiece in promotionPieces)
        {
            Move move = new Move(fromRow, fromCol, toRow, toCol);
            move.PromotionPiece = promotionPiece;

            if (board.TryMoveForSimulation(move))
                return true;
        }

        return false;
    }
}