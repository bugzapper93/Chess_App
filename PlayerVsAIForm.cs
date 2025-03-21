using Chess;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;

namespace Chess_App
{
    public partial class PlayerVsAIForm : Form
    {
        Chessboard board;

        bool is_dragging = false;
        Position selected_position;
        Point drag_offset = Point.Empty;
        Point start_pos;
        Point original_location;
        Panel[,] background;
        Panel[,] piecesDisplay;
        Color boardFirstColor = Color.FromArgb(208, 206, 241);
        Color boardSecondColor = Color.FromArgb(35, 41, 76);
        private NotationPanelManager notationPanelManager;
        private AIPlayer aiPlayer;
        public PlayerVsAIForm()
        {
            InitializeComponent();
            board = new Chessboard();
            start_pos = groupBox1.Location;

            piecesDisplay = new Panel[Variables.Board_Size, Variables.Board_Size];
            background = new Panel[Variables.Board_Size, Variables.Board_Size];

            notationPanelManager = new NotationPanelManager(tableLayoutPanel1);
            Initialize_Board();
            Initialize_Pieces();
            aiPlayer = new AIPlayer(2, Pieces.Black, 100);
        }
        private void Initialize_Pieces()
        {
            Point start = groupBox1.Location;
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                {
                    if (board.pieces[i, j].value != 0)
                    {
                        Panel piece = Create_Piece(board.pieces[i, j].value, i, j);
                        piece.Location = new Point(Variables.Margin / 2 + start_pos.X + j * Variables.Square_Size, Variables.Margin / 2 + start_pos.Y + i * Variables.Square_Size);
                        piece.MouseDown += Piece_Mouse_Down;
                        piece.MouseMove += Piece_Mouse_Move;
                        piece.MouseUp += Piece_Mouse_Up;

                        Controls.Add(piece);
                        piece.BringToFront();

                        piecesDisplay[i, j] = piece;
                    }
                }
        }
        private void Initialize_Board()
        {
            Point start = groupBox1.Location;
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                {
                    Panel panel = new Panel();
                    panel.Size = new Size(Variables.Square_Size, Variables.Square_Size);
                    panel.BackColor = (i + j) % 2 == 0 ? boardFirstColor : boardSecondColor;
                    panel.Location = new Point(start.X + j * Variables.Square_Size, start.Y + i * Variables.Square_Size);

                    Controls.Add(panel);
                    panel.BringToFront();

                    background[i, j] = panel;
                }
        }
        private Panel Create_Piece(int value, int row, int col)
        {
            Panel piece = new Panel();
            piece.Size = new Size(Variables.Square_Size - Variables.Margin, Variables.Square_Size - Variables.Margin);
            int pieceType = value & 7;
            int pieceColor = value & 24;
            string imageName = GetPieceImageName(pieceType, pieceColor);
            string imagePath = $"Resources/{imageName}.png";
            if (System.IO.File.Exists(imagePath))
            {
                piece.BackgroundImage = Image.FromFile(imagePath);
                piece.BackgroundImageLayout = ImageLayout.Stretch;
                piece.BackColor = GetColorUnderControl(piece, row, col);
            }
            else
            {
                piece.BackColor = pieceColor == Pieces.White ? Color.White : Color.Black;
            }
            return piece;
        }
        private Color GetColorUnderControl(Panel control, int row, int col)
        {
            return background[row, col].BackColor;
        }
        private static string GetPieceImageName(int pieceType, int pieceColor)
        {
            string color = pieceColor == Pieces.White ? "white" : "black";
            string type = "";

            switch (pieceType)
            {
                case Pieces.Pawn:
                    type = "pawn";
                    break;
                case Pieces.Rook:
                    type = "rook";
                    break;
                case Pieces.Knight:
                    type = "knight";
                    break;
                case Pieces.Bishop:
                    type = "bishop";
                    break;
                case Pieces.Queen:
                    type = "queen";
                    break;
                case Pieces.King:
                    type = "king";
                    break;
                default:
                    type = "empty"; 
                    break;
            }

            return $"{type}_{color}";
        }
        private void Highlight_Board(Position piece_pos)
        {
            Reset_Board();
            foreach (Move move in board.possible_moves_pins.moves)
            {
                if (Moves.Compare_Positions(move.start_pos, piece_pos))
                {
                    background[move.end_pos.row, move.end_pos.column].BackColor = Color.Red;
                }
            }
        }
        private void Reset_Board()
        {
            for (int i = 0; i < Variables.Board_Size; i++)
                for (int j = 0; j < Variables.Board_Size; j++)
                {
                    background[i, j].BackColor = (i + j) % 2 == 0 ? boardFirstColor : boardSecondColor;
                }
            ShowDanger();
        }
        private void ShowDanger()
        {
            for (int row = 0; row < Variables.Board_Size; row++)
            {
                for (int col = 0; col < Variables.Board_Size; col++)
                {
                    if (board.squares[row, col].danger_white)
                        background[row, col].BackColor = Color.Yellow;
                    if (board.squares[row, col].danger_black)
                        background[row, col].BackColor = Color.Orange;
                    if (board.squares[row, col].danger_white && board.squares[row, col].danger_black)
                        background[row, col].BackColor = Color.Blue;
                }
            }
        }
        private bool Is_Valid_Select(Piece piece)
        {
            if ((board.is_white_turn && (piece.value & 24) == Pieces.Black) || (!board.is_white_turn && (piece.value & 24) == Pieces.White))
            {
                return false;
            }
            return true;
        }
        private void Piece_Mouse_Down(object sender, MouseEventArgs e)
        {
            if (sender is Panel panel)
            {
                Panel piece = panel;
                for (int i = 0; i < Variables.Board_Size; i++)
                    for (int j = 0; j < Variables.Board_Size; j++)
                    {
                        if (piece == piecesDisplay[i, j])
                        {
                            Piece selected_piece = board.pieces[i, j];
                            if (!Is_Valid_Select(selected_piece))
                                return;
                            is_dragging = true;
                            selected_position = new Position { row = i, column = j };
                            Highlight_Board(selected_position);
                            original_location = e.Location;
                        }
                    }
                UpdatePieceBackColor(piece);
            }
        }
        private void Piece_Mouse_Move(object sender, MouseEventArgs e)
        {
            if (is_dragging && e.Button == MouseButtons.Left)
            {
                Point mouse_pos = this.PointToClient(Cursor.Position);
                Panel piece = piecesDisplay[selected_position.row, selected_position.column];
                piece.Location = new Point(mouse_pos.X - original_location.X, mouse_pos.Y - original_location.Y);
                UpdatePieceBackColor(piece);
            }
        }
        private Point Get_Target_Location(int row, int col)
        {
            Point position = new Point(Variables.Margin / 2 + start_pos.X + col * Variables.Square_Size, Variables.Margin / 2 + start_pos.Y + row * Variables.Square_Size);
            return position;
        }
        private async void Piece_Mouse_Up(object sender, MouseEventArgs e)
        {
            if (selected_position.row != -1 && selected_position.column != -1)
            {
                Panel selected = piecesDisplay[selected_position.row, selected_position.column];

                int target_col = (int)Math.Round((float)(selected.Location.X - start_pos.X) / (float)Variables.Square_Size, 0, MidpointRounding.AwayFromZero);
                int target_row = (int)Math.Round((float)(selected.Location.Y - start_pos.Y) / (float)Variables.Square_Size, 0, MidpointRounding.AwayFromZero);

                Position target_pos = new Position { row = target_row, column = target_col };
                Move move;
                bool isPawnMove = false;
                bool isEnPassant = false;
                bool isCapture = false;
                if (board.Check_If_Valid_Move(selected_position, target_pos, out move))
                {
                    if (move.capture)
                    {
                        isCapture = true;
                        Panel captured = piecesDisplay[move.capture_pos.row, move.capture_pos.column];
                        if (captured != null && captured != selected)
                        {
                            Controls.Remove(captured);
                            piecesDisplay[move.capture_pos.row, move.capture_pos.column] = null;
                        }
                    }

                    int pieceType = move.piece & 7;
                    isPawnMove = pieceType == Pieces.Pawn;
                    isEnPassant = move.is_en_passant;
                    selected.Location = Get_Target_Location(target_row, target_col);
                    piecesDisplay[target_row, target_col] = selected;
                    piecesDisplay[selected_position.row, selected_position.column] = null;

                    board.Make_Move(move, false);
                    string moveNotation = notationPanelManager.GetAlgebraicNotation(selected_position, target_pos, board.is_white_turn, isCapture, isPawnMove, isEnPassant, move.piece);
                    notationPanelManager.AddRowToTable(moveNotation, board.is_white_turn);

                    board.SwitchPlayer();

                    if (board.currentPlayer == Pieces.Black && !board.is_white_turn)
                    {
                        MakeAIMove();
                    }
                }
                else
                {
                    selected.Location = Get_Target_Location(selected_position.row, selected_position.column);
                }
                UpdatePieceBackColor(selected);
                selected_position = new Position { row = -1, column = -1 };
                is_dragging = false;

                Reset_Board();
            }
        }

        private void MakeAIMove()
        {
            Move bestMove = aiPlayer.GetBestMove(board);

            if (bestMove.Equals(default(Move)))
            {
                return;
            }
            if (board.is_white_turn) return;
            int pieceColor = board.pieces[bestMove.start_pos.row, bestMove.start_pos.column].value & 24;
            if (pieceColor != aiPlayer.GetColor())
            {
                return;
            }

            Panel aiPiece = piecesDisplay[bestMove.start_pos.row, bestMove.start_pos.column];
            if (aiPiece == null)
            {
                return;
            }

            if (piecesDisplay[bestMove.end_pos.row, bestMove.end_pos.column] != null)
            {
                Controls.Remove(piecesDisplay[bestMove.end_pos.row, bestMove.end_pos.column]);
                piecesDisplay[bestMove.end_pos.row, bestMove.end_pos.column] = null;
            }

            aiPiece.Location = Get_Target_Location(bestMove.end_pos.row, bestMove.end_pos.column);

            piecesDisplay[bestMove.end_pos.row, bestMove.end_pos.column] = aiPiece;
            piecesDisplay[bestMove.start_pos.row, bestMove.start_pos.column] = null;

            board.Make_Move(bestMove, false);

            string moveNotation = notationPanelManager.GetAlgebraicNotation(bestMove.start_pos, bestMove.end_pos, board.is_white_turn, bestMove.capture, (bestMove.piece & 7) == Pieces.Pawn, bestMove.is_en_passant, bestMove.piece);
            notationPanelManager.AddRowToTable(moveNotation, board.is_white_turn);

            aiPiece.BringToFront();
            this.Refresh();
            UpdatePieceBackColor(aiPiece);
            board.SwitchPlayer();
        }
        private void UpdatePieceBackColor(Panel piece)
        {
            int col = (piece.Location.X - start_pos.X) / Variables.Square_Size;
            int row = (piece.Location.Y - start_pos.Y) / Variables.Square_Size;

            if (row >= 0 && row < Variables.Board_Size && col >= 0 && col < Variables.Board_Size)
            {
                piece.BackColor = (row + col) % 2 == 0 ? boardFirstColor : boardSecondColor;
            }
        }
        private void button1_Click_1(object sender, EventArgs e)
        {
            MainMenu mainMenuForm = new MainMenu();
            mainMenuForm.Show();
            this.Hide();
        }
    }
}
