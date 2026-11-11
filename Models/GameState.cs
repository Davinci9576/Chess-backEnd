namespace ChessEngine;

public enum GameState
{
    InProgress,
    WhiteInCheck,
    BlackInCheck,

    WhiteWins,
    BlackWins,
    StaleMate,
    Draw
}