using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Chess_App
{
    class NotationPanelManager
    {
        private int currentRow = 0;
        private TableLayoutPanel tableLayoutPanel;
        public NotationPanelManager(TableLayoutPanel tableLayoutPanel)
        {
            this.tableLayoutPanel = tableLayoutPanel;
        }
        public void Add_Row_To_Table(string moveNotation, bool is_white_wurn)
        {
            //Color current_color_primary = currentRow % 2 == 0 ? Variables.Primary : Variables.Secondary;
            //Color current_color_secondary = currentRow % 2 == 0 ? Variables.Secondary : Variables.Primary;

            if (is_white_wurn)
            {
                Label index_Label = new Label
                {
                    Text = (currentRow + 1).ToString(),
                    Margin = Padding.Empty,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Dock = DockStyle.Fill
                };
                tableLayoutPanel.Controls.Add(index_Label, 0, currentRow);

                Label label1 = new Label
                {
                    Text = moveNotation,
                    Margin = Padding.Empty,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Dock = DockStyle.Fill
                };
                tableLayoutPanel.Controls.Add(label1, 1, currentRow);
            }
            else
            {
                Label label2 = new Label
                {
                    Text = moveNotation,
                    Margin = Padding.Empty,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Dock = DockStyle.Fill
                };
                tableLayoutPanel.Controls.Add(label2, 2, currentRow);
                currentRow++;
            }
        }
        public static string Get_Algebraic_Notation(Move move)
        {
            Position start_pos = move.start_pos;
            Position end_pos = move.end_pos;
            bool is_pawn_move = (move.piece & 7) == Pieces.Pawn ? true : false;
            bool is_capture = move.capture;
            bool is_en_passant = move.is_en_passant;

            string pieceNotation = Pieces.Piece_Value_To_String(move.piece);
            char columnStart = (char)('a' + start_pos.column);
            char rowStart = (char)('8' - start_pos.row);

            char columnEnd = (char)('a' + end_pos.column);
            char rowEnd = (char)('8' - end_pos.row);

            string notation = "";
            if (is_pawn_move)
            {
                if (is_capture)
                {
                    notation = $"{columnStart} x {columnEnd}{rowEnd}";
                }
                else
                {
                    notation = $"{columnEnd}{rowEnd}";
                }

                if (is_en_passant)
                {
                    notation += "(e.p.)";
                }
            }
            else
            {
                notation = $"{pieceNotation.ToString()}{columnEnd}{rowEnd}";

                if (is_capture)
                {
                    notation = $"{pieceNotation.ToString()} x {columnEnd}{rowEnd}";
                }
            }

            return notation;
        }
    }
}
