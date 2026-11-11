
using ChessEngine.Models;
using ChessEngine.Logic.SpecialMoves;


namespace ChessEngine;

public class Board
{
    public Piece[,] Squares { get; } = new Piece[8, 8];
    private readonly EnPassantHandler enPassantHandler = new EnPassantHandler();
    private readonly CastlingHandler castlingHandler = new CastlingHandler();
    public (int Row, int Col)? EnPassantTarget { get; internal set; }
    //tuple is used to store multiple values in a variable 

    public Board()
    {

    }

    public void Initialize()
    {
        Array.Clear(Squares);
        EnPassantTarget = null;

        for (int col = 0; col < 8; col++)
        {
            Squares[1, col] = Piece.BlackPawn;
            Squares[6, col] = Piece.WhitePawn;
        }

        Piece[] backRow =
        {
            Piece.BlackRook,
            Piece.BlackKnight,
            Piece.BlackBishop,
            Piece.BlackQueen,
            Piece.BlackKing,
            Piece.BlackBishop,
            Piece.BlackKnight,
            Piece.BlackRook
        };
        for (int col = 0; col < 8; col++)
        {
            Squares[0, col] = backRow[col];
            Squares[7, col] = backRow[col] switch
            {
                Piece.BlackRook => Piece.WhiteRook,
                Piece.BlackKnight => Piece.WhiteKnight,
                Piece.BlackBishop => Piece.WhiteBishop,
                Piece.BlackQueen => Piece.WhiteQueen,
                Piece.BlackKing => Piece.WhiteKing,
                _ => Piece.Empty
            };
        }
        WhiteKingSideCastle = true;
        WhiteQueenSideCastle = true;

        BlackKingSideCastle = true;
        BlackQueenSideCastle = true;
    }

    private MoveValidator validator = new MoveValidator();
    private readonly CheckDetector checkDetector = new CheckDetector();
    public bool MakeMove(Move move)
    {
        if (!InsideBoard(move.FromRow, move.FromCol) || !InsideBoard(move.ToRow, move.ToCol))
        {
            return false;
        }

        if (!HasPiece(move.FromRow, move.FromCol))
        {
            return false;
        }

        Piece piece = Squares[move.FromRow, move.FromCol];
        Piece target = Squares[move.ToRow, move.ToCol];

        bool isEnPassant = enPassantHandler.IsEnPassantMove(move, piece, this);

        if (target != Piece.Empty)
        {
            if (PieceHelper.IsWhitePiece(piece) == PieceHelper.IsWhitePiece(target))
            {
                return false;
            }
        }
        if (target == Piece.WhiteKing || target == Piece.BlackKing)
        {
            return false;
        }
        if (castlingHandler.IsCastlingMove(move, piece))
        {
            return castlingHandler.TryCastle(move, piece, this);
        }
        if (!validator.IsValidMove(move, piece, this, target))
        {
            return false;
        }

        Piece placedPiece = piece;

        if (piece == Piece.WhitePawn && move.ToRow == 0)
        {
            if (move.PromotionPiece == null || !IsValidPromotionPiece(move.PromotionPiece.Value, true))
            {
                return false;
            }
            placedPiece = move.PromotionPiece.Value;
        }
        else if (piece == Piece.BlackPawn && move.ToRow == 7)
        {
            if (move.PromotionPiece == null || !IsValidPromotionPiece(move.PromotionPiece.Value, false))
            {
                return false;
            }
            placedPiece = move.PromotionPiece.Value;
        }
        else if (move.PromotionPiece != null)
        {
            return false;
        }
        if (isEnPassant)
        {
            enPassantHandler.ExecutionEnPassant(move, this);
        }

        Squares[move.FromRow, move.FromCol] = Piece.Empty;
        Squares[move.ToRow, move.ToCol] = placedPiece;

        bool whiteKing = PieceHelper.IsWhitePiece(piece);

        if (checkDetector.IsKingInCheck(this, whiteKing))
        {
            Squares[move.FromRow, move.FromCol] = piece;
            Squares[move.ToRow, move.ToCol] = target;
            if (isEnPassant)
            {
                enPassantHandler.RestoreCapturedPawn(move, piece, this);
            }
            return false;
        }
        enPassantHandler.UpdateEnPassantTarget(move, piece, this);

        UpdateCastlingRights(move, piece, target);
        return true;
    }



    internal bool TryMoveForSimulation(Move move)
    {
        Piece[,] originalSquares = (Piece[,])Squares.Clone();

        bool whiteKingSide = WhiteKingSideCastle;
        bool whiteQueenSide = WhiteQueenSideCastle;
        bool blackKingSide = BlackKingSideCastle;
        bool blackQueenSide = BlackQueenSideCastle;
        var originalEnPassantTarget = EnPassantTarget;

        bool isLegal = MakeMove(move);

        Array.Copy(originalSquares, Squares, Squares.Length);

        WhiteKingSideCastle = whiteKingSide;
        WhiteQueenSideCastle = whiteQueenSide;
        BlackKingSideCastle = blackKingSide;
        BlackQueenSideCastle = blackQueenSide;
        EnPassantTarget = originalEnPassantTarget;

        return isLegal;
    }
    public bool HasPiece(int row, int col)
    {
        return Squares[row, col] != Piece.Empty;
    }
    public bool InsideBoard(int row, int col)
    {
        return row >= 0 && row < 8 && col >= 0 && col < 8;
    }
    public bool WhiteKingSideCastle { get; internal set; } = true;
    public bool WhiteQueenSideCastle { get; internal set; } = true;

    public bool BlackKingSideCastle { get; internal set; } = true;

    public bool BlackQueenSideCastle { get; internal set; } = true;

    internal void UpdateCastlingRights(Move move, Piece piece, Piece target)
    {
        if (piece == Piece.WhiteKing)
        {
            WhiteKingSideCastle = false;
            WhiteQueenSideCastle = false;
        }
        if (piece == Piece.BlackKing)
        {
            BlackKingSideCastle = false;
            BlackQueenSideCastle = false;
        }

        if (piece == Piece.WhiteRook)
        {
            if (move.FromRow == 7 && move.FromCol == 7)
            {
                WhiteKingSideCastle = false;
            }
            if (move.FromRow == 7 && move.FromCol == 0)
            {
                WhiteQueenSideCastle = false;
            }
        }
        if (piece == Piece.BlackRook)
        {
            if (move.FromRow == 0 && move.FromCol == 7)
            {
                BlackKingSideCastle = false;
            }
            if (move.FromRow == 0 && move.FromCol == 0)
            {
                BlackQueenSideCastle = false;
            }
        }
        if (target == Piece.WhiteRook)
        {
            if (move.ToRow == 7 && move.ToCol == 7)
            {
                WhiteKingSideCastle = false;
            }
            if (move.ToRow == 7 && move.ToCol == 0)
            {
                WhiteQueenSideCastle = false;
            }
        }
        if (target == Piece.BlackRook)
        {
            if (move.ToRow == 0 && move.ToCol == 7)
            {
                BlackKingSideCastle = false;
            }
            if (move.ToRow == 0 && move.ToCol == 0)
            {
                BlackQueenSideCastle = false;
            }
        }
    }
    private bool IsValidPromotionPiece(Piece piece, bool white)
    {
        if (white)
        {
            return piece == Piece.WhiteQueen ||
            piece == Piece.WhiteBishop ||
            piece == Piece.WhiteKnight ||
            piece == Piece.WhiteRook;
        }
        return piece == Piece.BlackQueen ||
        piece == Piece.BlackBishop ||
        piece == Piece.BlackKnight ||
        piece == Piece.BlackRook;
    }
}