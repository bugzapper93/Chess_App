using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess_App
{
    struct Position
    {
        public int row;
        public int column;
    }
    struct Move
    {
        public int piece;
        public Position start_pos;
        public Position end_pos;
        public bool capture;
    }
    class Moves
    {
        public static void Check_Validity(Move move, Chessboard board)
        {
            
        }
        //public static bool Check_Move
        public static void Get_All_Possible_Moves(Chessboard board)
        {
            List<Move> moves = new List<Move>();

            for (int i = 0; i < Variables.Board_Size; i++)
                for (int j = 0; j < Variables.Board_Size; j++)
                {
                    Position start = new Position { row = i, column = j };

                    int temp_piece = Pieces.Get_Value(board.pieces, start);

                    if (temp_piece == 0)
                        continue;

                    int base_piece = temp_piece & 7;
                    int color = temp_piece & 24;

                    if ((temp_piece & 7) == Pieces.Pawn)
                    {
                        if (color == Pieces.White)
                        {
                            int[] vars = [-1, 1];
                            if (Pieces.Get_Value(board.pieces, new Position { row = i - 1, column = j}) == 0 && i - 1 >= 0)
                                moves.Add(new Move { start_pos = start, end_pos = new Position { row = i - 1, column = j }, piece = temp_piece, capture = false });

                            foreach (int var in vars)
                            {
                                if (Pieces.Get_Value(board.pieces, new Position { row = i - 1, column = j + var }) != 0 &&
                                    (Pieces.Get_Value(board.pieces, new Position { row = i - 1, column = j + var }) & 7) != Pieces.King)
                                    moves.Add(new Move { start_pos = start, end_pos = new Position { row = i - 1, column = j + var }, piece = temp_piece, capture = true });
                                if (board.en_passant_target != null)
                                    if (board.en_passant_target)
                            }
                        }
                    }

                }
        
        }
    }
}
