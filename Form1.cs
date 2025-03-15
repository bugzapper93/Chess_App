namespace Chess_App
{
    //public class ChessBoardControl : Control
    //{
    //    Chessboard board;
    //    int squareSize = 70;
    //    bool isDragging = false;
    //    // Holds the position of the piece being dragged.
    //    Position? selectedPosition = null;
    //    // Offset for smooth dragging.
    //    Point draggingOffset = Point.Empty;

    //    public ChessBoardControl()
    //    {
    //        DoubleBuffered = true;
    //        // Initialize the chessboard using the FEN from your Pieces class.
    //        board = new Chessboard(Pieces.Default_Starting_Position);

    //        MouseDown += ChessBoardControl_MouseDown;
    //        MouseMove += ChessBoardControl_MouseMove;
    //        MouseUp += ChessBoardControl_MouseUp;
    //    }

    //    protected override void OnPaint(PaintEventArgs e)
    //    {
    //        base.OnPaint(e);
    //        Graphics g = e.Graphics;

    //        // Draw board squares.
    //        for (int row = 0; row < Variables.Board_Size; row++)
    //        {
    //            for (int col = 0; col < Variables.Board_Size; col++)
    //            {
    //                bool isLightSquare = (row + col) % 2 == 0;
    //                Color squareColor = isLightSquare ? Color.Beige : Color.Brown;
    //                Rectangle squareRect = new Rectangle(col * squareSize, row * squareSize, squareSize, squareSize);
    //                using (SolidBrush brush = new SolidBrush(squareColor))
    //                {
    //                    g.FillRectangle(brush, squareRect);
    //                }
    //                g.DrawRectangle(Pens.Black, squareRect);

    //                // Draw the piece if one exists and it is not being dragged.
    //                Piece piece = board.pieces[row, col];
    //                if (piece.value != 0)
    //                {
    //                    if (selectedPosition.HasValue &&
    //                        selectedPosition.Value.row == row &&
    //                        selectedPosition.Value.column == col &&
    //                        isDragging)
    //                    {
    //                        continue;
    //                    }
    //                    DrawPiece(g, piece, new Point(col * squareSize, row * squareSize));
    //                }
    //            }
    //        }

    //        // Draw the dragged piece following the mouse cursor.
    //        if (isDragging && selectedPosition.HasValue)
    //        {
    //            Piece piece = board.pieces[selectedPosition.Value.row, selectedPosition.Value.column];
    //            Point mousePos = PointToClient(Cursor.Position);
    //            Point drawPos = new Point(mousePos.X - draggingOffset.X, mousePos.Y - draggingOffset.Y);
    //            DrawPiece(g, piece, drawPos, isDragging: true);
    //        }
    //    }

    //    // Draws a piece on the board.
    //    private void DrawPiece(Graphics g, Piece piece, Point location, bool isDragging = false)
    //    {
    //        // For demonstration we simply use text to represent the piece.
    //        // You can replace this with images later.
    //        string text = GetPieceText(piece.value);
    //        using (Font font = new Font("Arial", 24))
    //        {
    //            SizeF textSize = g.MeasureString(text, font);
    //            PointF textLocation = new PointF(
    //                location.X + (squareSize - textSize.Width) / 2,
    //                location.Y + (squareSize - textSize.Height) / 2);
    //            Brush brush = isDragging ? Brushes.Red : Brushes.Black;
    //            g.DrawString(text, font, brush, textLocation);
    //        }
    //    }

    //    // Converts the piece's numeric value to a simple text representation.
    //    // This sample assumes positive values represent white pieces and negative values black pieces.
    //    // Adjust the mapping as needed based on your bit flags.
    //    private string GetPieceText(int value)
    //    {
    //        bool isWhite = (value & Pieces.White) == Pieces.White;
    //        int absValue = value & ~Pieces.White & ~Pieces.Black; // remove color bits
    //        string letter = absValue switch
    //        {
    //            1 => "K",
    //            2 => "P",
    //            3 => "N",
    //            4 => "B",
    //            5 => "R",
    //            6 => "Q",
    //            _ => "?"
    //        };
    //        return isWhite ? letter : letter.ToLower();
    //    }

    //    // MouseDown: Begin dragging if a piece is clicked.
    //    private void ChessBoardControl_MouseDown(object sender, MouseEventArgs e)
    //    {
    //        int col = e.X / squareSize;
    //        int row = e.Y / squareSize;
    //        if (row < 0 || row >= Variables.Board_Size || col < 0 || col >= Variables.Board_Size)
    //            return;

    //        Piece piece = board.pieces[row, col];
    //        if (piece.value != 0)
    //        {
    //            selectedPosition = new Position { row = row, column = col };
    //            isDragging = true;
    //            draggingOffset = new Point(e.X - col * squareSize, e.Y - row * squareSize);
    //        }
    //    }

    //    // MouseMove: Update dragging.
    //    private void ChessBoardControl_MouseMove(object sender, MouseEventArgs e)
    //    {
    //        if (isDragging)
    //            Invalidate();
    //    }

    //    // MouseUp: Drop the piece at the new location.
    //    private void ChessBoardControl_MouseUp(object sender, MouseEventArgs e)
    //    {
    //        if (isDragging && selectedPosition.HasValue)
    //        {
    //            int targetCol = e.X / squareSize;
    //            int targetRow = e.Y / squareSize;

    //            // Get the list of all possible moves for the current board state.
    //            Moveset moveset = Moves.Get_All_Possible_Moves(board);

    //            // Check if a move exists in the list matching the selected piece and the target square.
    //            bool valid = moveset.moves.Any(m =>
    //                m.start_pos.row == selectedPosition.Value.row &&
    //                m.start_pos.column == selectedPosition.Value.column &&
    //                m.end_pos.row == targetRow &&
    //                m.end_pos.column == targetCol);

    //            if (valid)
    //            {
    //                // Execute the move.
    //                Piece movingPiece = board.pieces[selectedPosition.Value.row, selectedPosition.Value.column];
    //                board.pieces[targetRow, targetCol] = movingPiece;
    //                board.pieces[selectedPosition.Value.row, selectedPosition.Value.column] = new Piece();
    //                movingPiece.position = new Position { row = targetRow, column = targetCol };

    //                // Optionally update other board state (e.g. en passant, castling rights, etc.)
    //            }
    //            else
    //            {
    //                // The move is invalid; you may choose to provide feedback here.
    //                // For now, we simply do nothing and the piece remains in its original place.
    //            }

    //            isDragging = false;
    //            selectedPosition = null;
    //            Invalidate();
    //        }
    //    }
    //}
    public partial class Form1 : Form
    {
        Chessboard board;

        bool is_dragging = false;
        Position? selected_position = null;
        Point drag_offset = Point.Empty;
        public Form1()
        {
            InitializeComponent();

        }
    }
}
