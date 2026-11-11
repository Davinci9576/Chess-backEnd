using ChessEngine;
using ChessEngine.Models;

GameController game = new GameController();

Move move = new Move(6, 4, 5, 4);

bool success = game.PlayMove(move);
if (success)
{
    Console.WriteLine("Move successful!");
}
else
{
    Console.WriteLine("Invalid move!");
}
Console.WriteLine();



