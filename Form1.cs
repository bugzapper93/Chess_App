namespace Chess_App
{
    public partial class Form1 : Form
    {
        Chessboard board;

        bool is_dragging = false;
        Position? selected_position = null;
        Point drag_offset = Point.Empty;
        Panel[,] background;
        Panel[,] piecesDisplay;
        public Form1()
        {
            InitializeComponent();
            board = new Chessboard();

            Initialize_Board();
            Initialize_Pieces();
        }
        public void Initialize_Pieces()
        {
            Point start = groupBox1.Location;
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                {
                    Panel panel = new Panel();
                    panel.Size = new Size(Variables.Square_Size - Variables.Margin, Variables.Square_Size - Variables.Margin);
                    panel.Location = new Point(Variables.Margin / 2 + start.X + i * Variables.Square_Size, Variables.Margin / 2 + start.Y + j * Variables.Square_Size);
                    panel.BackColor = Color.Black;
                    Controls.Add(panel);
                    panel.BringToFront();
                }
        }
        public void Initialize_Board()
        {
            Point start = groupBox1.Location;
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                {
                    Panel panel = new Panel();
                    panel.Size = new Size(Variables.Square_Size, Variables.Square_Size);
                    panel.BackColor = (i + j) % 2 == 0 ? Color.Green : Color.Beige;
                    panel.Location = new Point(start.X + i * Variables.Square_Size, start.Y + j * Variables.Square_Size);
                    Controls.Add(panel);
                    panel.BringToFront();
                }
        }
    }
}
