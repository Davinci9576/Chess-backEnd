
namespace ChessEngine.Logic;

public class GameStatus
{
    private readonly CheckDetector checkDetector = new CheckDetector();
    private readonly MoveGenerator moveGenerator;
    private readonly Board board;


    public GameStatus(Board board)
    {
        this.board = board;
        moveGenerator = new MoveGenerator(board);
    }
    public GameState GetGameState(Board board, bool whiteToMove)
    {
        bool inCheck = checkDetector.IsKingInCheck(board, whiteToMove);
        bool hasLegalMove = moveGenerator.HasAnyLegalMove(whiteToMove);

        if (!hasLegalMove)
        {
            if (inCheck)
            {
                return whiteToMove ? GameState.BlackWins : GameState.WhiteWins;
            }
            return GameState.StaleMate;
        }
        if (inCheck)
        {
            return whiteToMove ? GameState.WhiteInCheck : GameState.BlackInCheck;

        }
        return GameState.InProgress;
    }
}