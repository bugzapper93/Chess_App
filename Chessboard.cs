using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess_App
{
    struct Square
    {
        public int danger_type;
    }
    class Chessboard
    {
        public Piece[,] pieces;
        public Square[,] squares;
        public Position? en_passant_target = null;
        string current_moves = "";
        public Chessboard(string FEN_string = Pieces.Default_Starting_Position)
        {
            Initialize_Pieces(FEN_string);
            Initialize_Board();
        }
        public void Initialize_Pieces(string FEN_string)
        {
            pieces = Pieces.Parse_FEN(FEN_string);
        }
        public void Initialize_Board()
        {
            squares = new Square[Variables.Board_Size, Variables.Board_Size];
            Moveset moveset = Moves.Get_All_Possible_Moves(this);
            foreach (Position pin in moveset.pins)
                pieces[pin.row, pin.column].pinned = true;
            foreach (Move move in moveset.moves)
            {
                Position target = move.end_pos;
                if (move.possible_capture)
                    squares[target.row, target.column] = new Square { danger_type = move.piece & 24 };
                    
            }
        }
    }
}
