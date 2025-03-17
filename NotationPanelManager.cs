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
        public void AddRowToTable(string moveNotation, bool isWhiteTurn)
        {
            if (!isWhiteTurn)
            {
                Label indexLabel = new Label
                {
                    Text = (currentRow + 1).ToString(),
                    TextAlign = ContentAlignment.MiddleCenter,
                    Dock = DockStyle.Fill
                };
                tableLayoutPanel.Controls.Add(indexLabel, 0, currentRow);

                Label label1 = new Label
                {
                    Text = moveNotation,
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
                    TextAlign = ContentAlignment.MiddleCenter,
                    Dock = DockStyle.Fill
                };
                tableLayoutPanel.Controls.Add(label2, 2, currentRow);
                currentRow++;
            }
        }
        public string GetAlgebraicNotation(Position startPos, Position endPos, bool isWhiteTurn, bool isCapture, bool isPawnMove, bool isEnPassant, int pieceType)
        {
            string pieceNotation = Pieces.PieceValueToString(pieceType);
            char columnStart = (char)('a' + startPos.column);
            char rowStart = (char)('8' - startPos.row);

            char columnEnd = (char)('a' + endPos.column);
            char rowEnd = (char)('8' - endPos.row);

            string notation = "";
            if (isPawnMove)
            {
                if (isCapture)
                {
                    notation = $"{columnStart} x {columnEnd}{rowEnd}";
                }
                else
                {
                    notation = $"{columnEnd}{rowEnd}";
                }

                if (isEnPassant)
                {
                    notation += "(e.p.)";
                }
            }
            else
            {
                notation = $"{pieceNotation.ToString()}{columnEnd}{rowEnd}";

                if (isCapture)
                {
                    notation = $"{pieceNotation.ToString()} x {columnEnd}{rowEnd}";
                }
            }

            return notation;
        }
    }
}
