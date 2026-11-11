namespace ChessEngine.Models;

public class Move
{
    public int FromRow { get; }
    public int FromCol { get; }

    public int ToRow { get; }
    public int ToCol { get; }

    public Piece? PromotionPiece { get; set; }

    public Move(int fromRow, int fromCol, int toRow, int toColumn)
    {
        FromRow = fromRow;
        FromCol = fromCol;
        ToCol = toColumn;
        ToRow = toRow;
    }
}