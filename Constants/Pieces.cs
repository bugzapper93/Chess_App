using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess_App
{
    struct Piece
    {
        public int value;
        public Position position;
        public bool has_moved;
        public bool pinned;
    }
    class Pieces
    {
        public static Dictionary<char, int> Pieces_Notation = new Dictionary<char, int>
        {
            { 'k', King | Black },
            { 'p', Pawn | Black },
            { 'n', Knight | Black },
            { 'b', Bishop | Black },
            { 'r', Rook | Black },
            { 'q', Queen | Black },
            { 'K', King | White },
            { 'P', Pawn | White },
            { 'N', Knight | White },
            { 'B', Bishop | White },
            { 'R', Rook | White },
            { 'Q', Queen | White },
            { '1', 1 },
            { '2', 2 },
            { '3', 3 },
            { '4', 4 },
            { '5', 5 },
            { '6', 6 },
            { '7', 7 },
            { '8', 8 },
            { '/', 0 }
        };

        public const int King = 1;
        public const int Pawn = 2;
        public const int Knight = 3;
        public const int Bishop = 4;
        public const int Rook = 5;
        public const int Queen = 6;

        public const int White = 8;
        public const int Black = 16;

        public const string Default_Starting_Position = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR";

        public static int Get_Value(Piece[,] pieces, Position pos)
        {
            if (pos.row < 0 || pos.row > 7 || pos.column < 0 || pos.column > 7)
                return -1;
            return pieces[pos.row, pos.column].value;
        }
        public static bool Check_Move(Piece[,] pieces, Position start, Position end)
        {
            if (end.row >= 8 || end.row < 0 || end.column >= 8 || end.column < 0)
                return false;
            if ((pieces[start.row, start.column].value & 24) == (pieces[end.row, end.column].value & 24))
                return false;
            return true;
        }
        public static Piece[,] Parse_FEN(string FEN_string)
        {
            Piece[,] pieces = new Piece[Variables.Board_Size, Variables.Board_Size];
            bool has_moved = false;

            if (FEN_string != Default_Starting_Position)
                has_moved = true;

            int row = 0;
            int col = 0;

            foreach (char piece in FEN_string)
            {
                if (!Pieces_Notation.ContainsKey(piece))
                    continue;

                int current_piece = Pieces_Notation[piece];

                if (current_piece == 0)
                {
                    row++;
                    col = 0;
                    continue;
                }

                if (current_piece >= 1 && current_piece <= 8)
                {
                    col += current_piece;
                }

                if (col < Variables.Board_Size && row < Variables.Board_Size)
                {
                    pieces[row, col] = new Piece
                    {
                        value = current_piece,
                        position = new Position
                        {
                            row = row,
                            column = col
                        },
                        has_moved = has_moved
                    };
                    col++;
                }
            }
            return pieces;
        }
        public static string Piece_Value_To_String(int value)
        {
            foreach (var entry in Pieces_Notation)
            {
                if (entry.Value == value)
                {
                    return entry.Key.ToString();  
                }
            }

            return string.Empty;
        }
        public static int Get_Piece_Value(int piece)
        {
            switch (piece & ~24)
            {
                case King: return 1000;
                case Queen: return 9;
                case Rook: return 5;
                case Bishop: return 3;
                case Knight: return 3;
                case Pawn: return 1;
                default: return 0;
            }
        }
    }
}
