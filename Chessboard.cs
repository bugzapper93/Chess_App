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
        }
        public void Initialize_Pieces(string FEN_string)
        {
            pieces = Pieces.Parse_FEN(FEN_string);
        }
        public void Initialize_Board()
        {

        }
    }
}
