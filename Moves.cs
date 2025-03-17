using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess_App
{
    public struct Position
    {
        public int row;
        public int column;
    }
    public struct Move
    {
        public int piece;
        public Position start_pos;
        public Position end_pos;
        public bool capture;
        public bool possible_capture;
        public int captured_piece;
        public Position capture_pos;
        internal bool is_en_passant;
    }
    public struct Moveset
    {
        public List<Move> moves;
        public List<Position> pins;
    }
    class Moves
    {
        public static Moveset Get_All_Possible_Moves(Chessboard board)
        {
            List<Move> moves = new List<Move>();
            List<Position> pins = new List<Position>();
            for (int i = 0; i < Variables.Board_Size; i++)
                for (int j = 0; j < Variables.Board_Size; j++)
                {
                    Position start = new Position { row = i, column = j };

                    bool capture = false;
                    int temp_piece = Pieces.Get_Value(board.pieces, start);
                    int row_diff, col_diff;

                    if (temp_piece == 0)
                        continue;
                    if (board.pieces[i, j].pinned)
                        continue;

                    int base_piece = temp_piece & 7;
                    int color = temp_piece & 24;
                    Position considered_position;
                    Position default_capture = new Position { row = -1, column = -1 };

                    switch (base_piece)
                    {
                        case Pieces.Pawn:
                            if (color == Pieces.White)
                                row_diff = -1;
                            else
                                row_diff = 1;
                            int[] vars = [-1, 1];
                            considered_position = new Position { row = i + row_diff, column = j };

                            if (Pieces.Get_Value(board.pieces, considered_position) == 0 && i + row_diff >= 0 && i + row_diff < 8)
                                moves.Add(new Move 
                                { 
                                    piece = temp_piece, 
                                    start_pos = start, 
                                    end_pos = considered_position, 
                                    capture = false, 
                                    possible_capture = false,
                                    capture_pos = default_capture
                                });

                            if ((color == Pieces.White && i == 6) || (color == Pieces.Black && i == 1))
                            {
                                considered_position = new Position { row = i + row_diff * 2, column = j };
                                if (Pieces.Get_Value(board.pieces, considered_position) == 0 && Pieces.Check_Move(board.pieces, start, considered_position))
                                    moves.Add(new Move
                                    {
                                        piece = temp_piece,
                                        start_pos = start,
                                        end_pos = considered_position,
                                        capture = false,
                                        possible_capture = false,
                                        capture_pos = default_capture
                                    });
                            }

                            foreach (int var in vars)
                            {
                                considered_position = new Position { row = i + row_diff, column = j + var };
                                if (Pieces.Check_Move(board.pieces, start, considered_position) && board.en_passant_target_color != color && Pieces.Get_Value(board.pieces, considered_position) == 0 && board.en_passant_target.HasValue && Compare_Positions(considered_position, board.en_passant_target.Value))
                                {
                                    Position pawn_target_pos = new Position { row = i, column = j + var };
                                    moves.Add(new Move
                                    {
                                        piece = temp_piece,
                                        start_pos = start,
                                        end_pos = considered_position,
                                        capture = true,
                                        possible_capture = true,
                                        captured_piece = Pieces.Get_Value(board.pieces, pawn_target_pos),
                                        capture_pos = new Position { row = i, column = j + var },
                                        is_en_passant = true
                                    });
                                }
                                if (Pieces.Check_Move(board.pieces, start, considered_position) && Pieces.Get_Value(board.pieces, considered_position) != 0)
                                    moves.Add(new Move
                                    {
                                        piece = temp_piece,
                                        start_pos = start,
                                        end_pos = considered_position,
                                        capture = true,
                                        possible_capture = true,
                                        captured_piece = Pieces.Get_Value(board.pieces, considered_position),
                                        capture_pos = considered_position
                                    });
                            }
                            break;

                        case Pieces.Knight:
                            int[] row_vars = [-2, -1, 1, 2];
                            int[] col_vars = [-2, -1, 1, 2];
                            foreach (int row in row_vars)
                            {
                                foreach (int col in col_vars)
                                {
                                    if ((Math.Abs(row) == 2 && Math.Abs(col) == 1) || (Math.Abs(row) == 1 && Math.Abs(col) == 2))
                                    {
                                        considered_position = new Position { row = i + row, column = j + col };
                                        if (considered_position.row < 0 || considered_position.row >= 8 || considered_position.column < 0 || considered_position.column >= 8)
                                            continue;
                                        if (Pieces.Check_Move(board.pieces, start, considered_position))
                                        {
                                            capture = Pieces.Get_Value(board.pieces, considered_position) != 0;
                                            moves.Add(new Move
                                            {
                                                piece = temp_piece,
                                                start_pos = start,
                                                end_pos = considered_position,
                                                capture = capture,
                                                possible_capture = true,
                                                captured_piece = capture ? Pieces.Get_Value(board.pieces, considered_position) : 0,
                                                capture_pos = capture ? considered_position : default_capture
                                            });
                                        }
                                    }
                                }
                            }
                            break;

                        case Pieces.Bishop:
                            int[] row_directions = [1, 1, -1, -1];
                            int[] col_directions = [1, -1, 1, -1];

                            for (int d = 0; d < 4; d++)
                            {
                                row_diff = row_directions[d];
                                col_diff = col_directions[d];
                                int step = 1;

                                while (true)
                                {
                                    considered_position = new Position { row = i + step * row_diff, column = j + step * col_diff };

                                    if (!Pieces.Check_Move(board.pieces, start, considered_position))
                                    {
                                        break;
                                    }

                                    int square_value = Pieces.Get_Value(board.pieces, considered_position);

                                    if (square_value == 0)
                                    {
                                        moves.Add(new Move
                                        {
                                            piece = temp_piece,
                                            start_pos = start,
                                            end_pos = considered_position,
                                            capture = false,
                                            possible_capture = true,
                                        });
                                    }
                                    else
                                    {
                                        int next_step = step + 1;
                                        while (true)
                                        {
                                            Position check_pos = new Position { row = i + next_step * row_diff, column = j + next_step * col_diff };

                                            if (!Pieces.Check_Move(board.pieces, start, check_pos))
                                            {
                                                break;
                                            }

                                            int next_value = Pieces.Get_Value(board.pieces, check_pos);
                                            if (next_value == 0)
                                            {
                                                next_step++;
                                                continue;
                                            }
                                            else
                                            {
                                                if (((next_value & 7) == Pieces.King) && ((next_value & 24) != (temp_piece & 24)))
                                                {
                                                   pins.Add(considered_position);
                                                }
                                                break;
                                            }
                                        }

                                        moves.Add(new Move
                                        {
                                            piece = temp_piece,
                                            start_pos = start,
                                            end_pos = considered_position,
                                            capture = true,
                                            possible_capture = true,
                                            captured_piece = square_value,
                                            capture_pos = considered_position
                                        });
                                        
                                        break;
                                    }
                                    step++;
                                }
                            }

                            break;

                        case Pieces.Rook:
                            row_directions = [0, 0, 1, -1];
                            col_directions = [1, -1, 0, 0];

                            for (int d = 0; d < 4; d++)
                            {
                                row_diff = row_directions[d];
                                col_diff = col_directions[d];
                                int step = 1;

                                while (true)
                                {
                                    considered_position = new Position
                                    {
                                        row = i + step * row_diff,
                                        column = j + step * col_diff
                                    };

                                    if (!Pieces.Check_Move(board.pieces, start, considered_position))
                                    {
                                        break;
                                    }

                                    int square_value = Pieces.Get_Value(board.pieces, considered_position);

                                    if (square_value == 0)
                                    {
                                        moves.Add(new Move
                                        {
                                            piece = temp_piece,
                                            start_pos = start,
                                            end_pos = considered_position,
                                            capture = false,
                                            possible_capture = true,
                                        });
                                    }
                                    else
                                    {
                                        int next_step = step + 1;
                                        while (true)
                                        {
                                            Position check_pos = new Position
                                            {
                                                row = i + next_step * row_diff,
                                                column = j + next_step * col_diff
                                            };

                                            if (!Pieces.Check_Move(board.pieces, start, check_pos))
                                            {
                                                break;
                                            }

                                            int next_value = Pieces.Get_Value(board.pieces, check_pos);
                                            if (next_value == 0)
                                            {
                                                next_step++;
                                                continue;
                                            }
                                            else
                                            {
                                                if (((next_value & 7) == Pieces.King) && ((next_value & 24) != (temp_piece & 24)))
                                                {
                                                    pins.Add(considered_position);
                                                }
                                                break;
                                            }
                                        }

                                        moves.Add(new Move
                                        {
                                            piece = temp_piece,
                                            start_pos = start,
                                            end_pos = considered_position,
                                            capture = true,
                                            possible_capture = true,
                                            captured_piece = square_value,
                                            capture_pos = considered_position
                                        });

                                        break;
                                    }
                                    step++;
                                }
                            }
                            break;

                        case Pieces.Queen:
                            row_directions = [1, 1, -1, -1, 0, 0, 1, -1];
                            col_directions = [1, -1, 1, -1, 1, -1, 0, 0];

                            for (int d = 0; d < 8; d++)
                            {
                                row_diff = row_directions[d];
                                col_diff = col_directions[d];
                                int step = 1;

                                while (true)
                                {
                                    considered_position = new Position
                                    {
                                        row = i + step * row_diff,
                                        column = j + step * col_diff
                                    };

                                    if (!Pieces.Check_Move(board.pieces, start, considered_position))
                                    {
                                        break;
                                    }

                                    int square_value = Pieces.Get_Value(board.pieces, considered_position);

                                    if (square_value == 0)
                                    {
                                        moves.Add(new Move
                                        {
                                            piece = temp_piece,
                                            start_pos = start,
                                            end_pos = considered_position,
                                            capture = false,
                                            possible_capture = true,
                                        });
                                    }
                                    else
                                    {
                                        int next_step = step + 1;
                                        while (true)
                                        {
                                            Position check_pos = new Position
                                            {
                                                row = i + next_step * row_diff,
                                                column = j + next_step * col_diff
                                            };

                                            if (!Pieces.Check_Move(board.pieces, start, check_pos))
                                            {
                                                break;
                                            }

                                            int next_value = Pieces.Get_Value(board.pieces, check_pos);
                                            if (next_value == 0)
                                            {
                                                next_step++;
                                                continue;
                                            }
                                            else
                                            {
                                                if (((next_value & 7) == Pieces.King) && ((next_value & 24) != (temp_piece & 24)))
                                                {
                                                    pins.Add(considered_position);
                                                }
                                                break;
                                            }
                                        }

                                        moves.Add(new Move
                                        {
                                            piece = temp_piece,
                                            start_pos = start,
                                            end_pos = considered_position,
                                            capture = true,
                                            possible_capture = true,
                                            captured_piece = square_value,
                                            capture_pos = considered_position
                                        });
                                        break;
                                    }
                                    step++;
                                }
                            }
                            break;

                        case Pieces.King:
                            row_directions = [1, 1, 1, 0, 0, -1, -1, -1];
                            col_directions = [1, 0, -1, 1, -1, 1, 0, -1];

                            for (int d = 0; d < 8; d++)
                            {
                                considered_position = new Position
                                {
                                    row = i + row_directions[d],
                                    column = j + col_directions[d]
                                };

                                if (!Pieces.Check_Move(board.pieces, start, considered_position))
                                {
                                    continue;
                                }
                                if ((board.squares[considered_position.row, considered_position.column].danger_white && color == Pieces.Black))
                                {
                                    continue;
                                }
                                if ((board.squares[considered_position.row, considered_position.column].danger_black && color == Pieces.White))
                                    continue;

                                int square_value = Pieces.Get_Value(board.pieces, considered_position);

                                if (square_value == 0)
                                {
                                    moves.Add(new Move
                                    {
                                        piece = temp_piece,
                                        start_pos = start,
                                        end_pos = considered_position,
                                        capture = false,
                                        possible_capture = true,
                                    });
                                }
                                else
                                {
                                    moves.Add(new Move
                                    {
                                        piece = temp_piece,
                                        start_pos = start,
                                        end_pos = considered_position,
                                        capture = true,
                                        possible_capture = true,
                                        captured_piece = square_value,
                                        capture_pos = considered_position
                                    });
                                }
                            }
                            break;
                    }
                }
            Moveset moveset = new Moveset { moves = moves, pins = pins };
            return moveset;
        }
        public static bool Compare_Positions(Position pos_1, Position pos_2)
        {
            if (pos_1.row == pos_2.row && pos_1.column == pos_2.column)
                return true;
            return false;
        }
    }
}
