using Microsoft.VisualBasic.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess_App
{
    struct Square
    {
        public bool danger_white;
        public bool danger_black;
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
        }
        private void Update_Danger()
        {
            possible_moves_pins = Moves.Get_All_Possible_Moves(this);
            for (int row = 0; row < Variables.Board_Size; row++)
            {
                for (int col = 0; col < Variables.Board_Size; col++)
                {
                    pieces[row, col].pinned = false;
                    squares[row, col].danger_white = false;
                    squares[row, col].danger_black = false;
                }
            }
            foreach (Position pin in possible_moves_pins.pins)
            {
                pieces[pin.row, pin.column].pinned = true;
            }
            foreach (Move move in possible_moves_pins.moves)
            {
                int row = move.end_pos.row;
                int col = move.end_pos.column;
                int danger_color = (move.piece & 24);
                if (move.possible_capture)
                {
                    if (danger_color == Pieces.White)
                        squares[row, col].danger_white = true;
                    if (danger_color == Pieces.Black)
                        squares[row, col].danger_black = true;
                }
                // Different handling for pawns
                if ((move.piece & 7) == Pieces.Pawn)
                {
                    int row_diff = (move.piece & 24) == Pieces.White ? -1 : 1;
                    int[] vars = [-1, 1];
                    foreach (int var in vars)
                    {
                        Position considered_position = new Position { row = move.start_pos.row + row_diff, column = move.start_pos.column + var };
                        if (Pieces.Check_Move(pieces, move.start_pos, considered_position))
                        {
                            if (danger_color == Pieces.White)
                                squares[considered_position.row, considered_position.column].danger_white = true;
                            if (danger_color == Pieces.Black)
                                squares[considered_position.row, considered_position.column].danger_black = true;
                            
                        }
                    }
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
        public string Make_Move(Move move, bool check_valid = true)
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

            Update_Danger();
            possible_moves_pins = Moves.Get_All_Possible_Moves(this);

            is_white_turn = !is_white_turn;

            return NotationPanelManager.Get_Algebraic_Notation(move);
        }
    }
}
