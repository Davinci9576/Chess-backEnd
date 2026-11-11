namespace ChessEngine.Models;

public static class PieceHelper
{
    public static bool IsWhitePiece(Piece piece)
    {
        return piece == Piece.WhitePawn ||
               piece == Piece.WhiteKnight ||
               piece == Piece.WhiteBishop ||
               piece == Piece.WhiteRook ||
               piece == Piece.WhiteQueen ||
               piece == Piece.WhiteKing;
    }
    public static bool IsBlackPiece(Piece piece)
    {
        return piece == Piece.BlackPawn ||
               piece == Piece.BlackKnight ||
               piece == Piece.BlackBishop ||
               piece == Piece.BlackRook ||
               piece == Piece.BlackQueen ||
               piece == Piece.BlackKing;
    }
}