using ChessEngine.Models;
using ChessEngine.Logic;

namespace ChessEngine;

public class GameController
{
    private readonly Board board;
    private readonly GameStatus gameStatus;

    private bool whiteToMove = true;

    public GameState CurrentGameState { get; private set; }

    public GameController()
    {
        board = new Board();
        board.Initialize();
        gameStatus = new GameStatus(board);
        CurrentGameState = GameState.InProgress;
    }
    public bool PlayMove(Move move)
    {
        if (CurrentGameState == GameState.WhiteWins ||
           CurrentGameState == GameState.BlackWins ||
           CurrentGameState == GameState.StaleMate)
        {
            return false;
        }

        if (!board.InsideBoard(move.FromRow, move.FromCol) ||
        !board.InsideBoard(move.ToRow, move.ToCol))
            return false;

        Piece piece = board.Squares[move.FromRow, move.FromCol];

        if (piece != Piece.Empty)
        {
            return false;
        }
        if (PieceHelper.IsWhitePiece(piece) != whiteToMove)
        {
            return false;
        }
        if (!board.MakeMove(move))
        {
            return false;
        }
        whiteToMove = !whiteToMove;

        CurrentGameState = gameStatus.GetGameState(board, whiteToMove);
        return true;
    }
}