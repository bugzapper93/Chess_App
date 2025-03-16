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
        public Moveset possible_moves_pins;
        public int? en_passant_target_color = null;
        public bool is_white_turn = true;
        string current_moves = "";
        public Chessboard(string FEN_string = Pieces.Default_Starting_Position)
        {
            pieces = new Piece[Variables.Board_Size, Variables.Board_Size];
            squares = new Square[Variables.Board_Size, Variables.Board_Size];

            Initialize_Pieces(FEN_string);
            Initialize_Board();

            possible_moves_pins = Moves.Get_All_Possible_Moves(this);

            Update_Danger();
        }
        private void Initialize_Pieces(string FEN_string)
        {
            pieces = Pieces.Parse_FEN(FEN_string);
        }
        private void Initialize_Board()
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
        private void Update_Danger()
        {
            for (int row = 0; row < Variables.Board_Size; row++)
            {
                for (int col = 0; col < Variables.Board_Size; col++)
                {
                    squares[row, col].danger_type = 0;
                }
            }
            foreach (Move move in possible_moves_pins.moves)
            {
                int row = move.end_pos.row;
                int col = move.end_pos.column;
                if (move.possible_capture)
                {
                    int danger_color = (move.piece & 24);
                    squares[row, col].danger_type = danger_color;
                }
            }
        }
        public bool Check_If_Valid_Move(Position start_pos, Position end_pos, out Move out_move)
        {
            foreach (Move move in possible_moves_pins.moves)
            {
                if (Moves.Compare_Positions(start_pos, move.start_pos) && Moves.Compare_Positions(end_pos, move.end_pos))
                {
                    out_move = move;
                    return true;
                }
                    
            }
            out_move = new Move();
            return false;
        }
        public void Make_Move(Move move, bool check_valid = true)
        {
            Position start_pos = move.start_pos;
            Position end_pos = move.end_pos;
            Position capture_pos = move.capture_pos;
            int piece_value = move.piece;

            if (move.capture)
                pieces[capture_pos.row, capture_pos.column] = new Piece { value = 0 };

            pieces[end_pos.row, end_pos.column] = pieces[start_pos.row, start_pos.column];
            pieces[start_pos.row, start_pos.column] = new Piece { value = 0 };

            if ((piece_value & 7) == Pieces.Pawn && Math.Abs(start_pos.row - end_pos.row) == 2)
            {
                en_passant_target = new Position { row = start_pos.row + (((piece_value & 24) == Pieces.White) ? -1 : 1), column = start_pos.column };
                en_passant_target_color = piece_value & 24;
            } 
            else
                en_passant_target = null;

            possible_moves_pins = Moves.Get_All_Possible_Moves(this);
            Update_Danger();
            is_white_turn = !is_white_turn;
        }
    }
}
