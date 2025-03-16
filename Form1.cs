namespace Chess_App
{
    public partial class Form1 : Form
    {
        Chessboard board;

        bool is_dragging = false;
        Position selected_position;
        Point drag_offset = Point.Empty;
        Point start_pos;
        Point original_location;
        Panel[,] background;
        Panel[,] piecesDisplay;
        public Form1()
        {
            InitializeComponent();

            board = new Chessboard();
            start_pos = groupBox1.Location;

            piecesDisplay = new Panel[Variables.Board_Size, Variables.Board_Size];
            background = new Panel[Variables.Board_Size, Variables.Board_Size];

            Initialize_Board();
            Initialize_Pieces();
        }
        private void Initialize_Pieces()
        {
            Point start = groupBox1.Location;
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                {
                    if (board.pieces[i, j].value != 0)
                    {
                        Panel piece = Create_Piece(board.pieces[i, j].value);
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
                    panel.BackColor = (i + j) % 2 == 0 ? Color.Green : Color.Beige;
                    panel.Location = new Point(start.X + j * Variables.Square_Size, start.Y + i * Variables.Square_Size);
                    
                    Controls.Add(panel);
                    panel.BringToFront();

                    background[i, j] = panel;
                }
        }
        private static Panel Create_Piece(int value)
        {
            Panel piece = new Panel();
            piece.Size = new Size(Variables.Square_Size - Variables.Margin, Variables.Square_Size - Variables.Margin);
            piece.BackColor = (value & 24) == Pieces.White ? Color.White : Color.Black;
            return piece;
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
                    background[i, j].BackColor = (i + j) % 2 == 0 ? Color.Green : Color.Beige;
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
        private void Validate_Pieces()
        {
            for (int i = 0; i < Variables.Board_Size; i++)
                for (int j = 0; j < Variables.Board_Size; j++)
                {
                    //if (board.pieces[i, j].value == 0)
                }
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
            }
        }
        private void Piece_Mouse_Move(object sender, MouseEventArgs e)
        {
            if (is_dragging && e.Button == MouseButtons.Left)
            {
                Point mouse_pos = this.PointToClient(Cursor.Position);
                piecesDisplay[selected_position.row, selected_position.column].Location = new Point(mouse_pos.X - original_location.X, mouse_pos.Y - original_location.Y);
            }
        }
        private Point Get_Target_Location(int row, int col)
        {
            Point position = new Point(Variables.Margin / 2 + start_pos.X + col * Variables.Square_Size, Variables.Margin / 2 + start_pos.Y + row * Variables.Square_Size);
            return position;
        }
        private void Piece_Mouse_Up(object sender, MouseEventArgs e)
        {
            if (selected_position.row != -1 && selected_position.column != -1)
            {
                Panel selected = piecesDisplay[selected_position.row, selected_position.column];

                int target_col = (int)Math.Round((float)(selected.Location.X - start_pos.X) / (float)Variables.Square_Size, 0, MidpointRounding.AwayFromZero);
                int target_row = (int)Math.Round((float)(selected.Location.Y - start_pos.Y) / (float)Variables.Square_Size, 0, MidpointRounding.AwayFromZero);

                Position target_pos = new Position { row = target_row, column = target_col };
                Move move;

                if (board.Check_If_Valid_Move(selected_position, target_pos, out move))
                {
                    if (move.capture)
                    {
                        Panel captured = piecesDisplay[move.capture_pos.row, move.capture_pos.column];
                        if (captured != null && captured != selected)
                        {
                            Controls.Remove(captured);
                            piecesDisplay[move.capture_pos.row, move.capture_pos.column] = null;
                        }
                    }

                    selected.Location = Get_Target_Location(target_row, target_col);
                    piecesDisplay[target_row, target_col] = selected;
                    piecesDisplay[selected_position.row, selected_position.column] = null;
                    
                    board.Make_Move(move, false);
                }
                else
                {
                    selected.Location = Get_Target_Location(selected_position.row, selected_position.column);
                }
                selected_position = new Position { row = -1, column = -1 };
                is_dragging = false;
                Reset_Board();
            }
        }
    }
}
