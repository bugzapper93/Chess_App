using Chess_App;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Chess
{
    class AIPlayer
    {
        private int maxDepth;
        private int playerColor;
        private int timeLimitMs;
        private ConcurrentDictionary<long, int> transpositionTable = new ConcurrentDictionary<long, int>();

        public AIPlayer(int depth, int color, int timeLimit)
        {
            maxDepth = depth;
            playerColor = color;
            timeLimitMs = timeLimit;
        }

        public Move GetBestMove(Chessboard board)
        {
            if (playerColor == Pieces.Black)
            {
                Move bestMove = default(Move);
                int bestValue = int.MinValue;
                object lockObj = new object();

                var allPossibleMoves = Moves.Get_All_Possible_Moves(board);

                var possibleMoves = new Moveset
                {
                    moves = allPossibleMoves.moves
                        .Where(move => (board.pieces[move.start_pos.row, move.start_pos.column].value & 24) == playerColor)
                        .ToList(),
                    pins = allPossibleMoves.pins
                };

                if (!possibleMoves.moves.Any())
                {
                    return bestMove;
                }

                var startTime = DateTime.Now;

                foreach (var depth in Enumerable.Range(1, maxDepth))
                {
                    if ((DateTime.Now - startTime).TotalMilliseconds > timeLimitMs)
                    {
                        break;
                    }

                    Move tempBestMove = bestMove;

                    Parallel.ForEach(possibleMoves.moves, move =>
                    {
                        Chessboard tempBoard = board.Clone();
                        tempBoard.Make_Move(move);

                        int moveValue = Minimax(tempBoard, depth, int.MinValue, int.MaxValue, false);

                        lock (lockObj)
                        {
                            if (moveValue > bestValue)
                            {
                                bestValue = moveValue;
                                tempBestMove = move;
                            }
                        }
                    });
                    bestMove = tempBestMove;
                }

                return bestMove;
            }
            return default;
        }

        private int Minimax(Chessboard board, int depth, int alpha, int beta, bool isMaximizing)
        {
            if (depth == 0)
            {
                return EvaluateBoard(board, playerColor);
            }

            var moves = OrderMoves(Moves.Get_All_Possible_Moves(board), board);

            if (isMaximizing)
            {
                int maxEval = int.MinValue;
                foreach (var move in moves.moves)
                {
                    Chessboard tempBoard = board.Clone();
                    tempBoard.Make_Move(move);

                    int eval = Minimax(tempBoard, depth - 1, alpha, beta, false);
                    maxEval = Math.Max(maxEval, eval);
                    alpha = Math.Max(alpha, eval);

                    if (beta <= alpha)
                        break;
                }
                return maxEval;
            }
            else
            {
                int minEval = int.MaxValue;
                foreach (var move in moves.moves)
                {
                    Chessboard tempBoard = board.Clone();
                    tempBoard.Make_Move(move);

                    int eval = Minimax(tempBoard, depth - 1, alpha, beta, true);
                    minEval = Math.Min(minEval, eval);
                    beta = Math.Min(beta, eval);

                    if (beta <= alpha)
                        break;
                }
                return minEval;
            }
        }

        private static int EvaluateBoard(Chessboard board, int playerColor)
        {
            int whiteScore = 0, blackScore = 0;

            foreach (var piece in board.pieces)
            {
                int pieceValue = Pieces.GetPieceValue(piece.value);
                if ((piece.value & Pieces.White) != 0)
                    whiteScore += pieceValue;
                else
                    blackScore += pieceValue;
            }

            foreach (var move in Moves.Get_All_Possible_Moves(board).moves)
            {
                if (move.capture)
                {
                    int victimValue = Pieces.GetPieceValue(move.captured_piece);
                    if ((move.piece & 24) == Pieces.White)
                        whiteScore += victimValue * 2; 
                    else
                        blackScore += victimValue * 2; 
                }
            }

            int evaluation = whiteScore - blackScore;
            return evaluation * (playerColor == Pieces.White ? 1 : -1);
        }
        private Moveset OrderMoves(Moveset possibleMoves, Chessboard board)
        {
            List<Move> orderedMoves = possibleMoves.moves
                .OrderByDescending(move => GetMoveScore(move, board))
                .ToList();

            return new Moveset { moves = orderedMoves, pins = possibleMoves.pins };
        }

        private int GetMoveScore(Move move, Chessboard board)
        {
            int score = 0;

            if (move.capture)
            {
                int victimValue = Pieces.GetPieceValue(move.captured_piece);
                int attackerValue = Pieces.GetPieceValue(move.piece);
                score += 10 * victimValue - attackerValue; 
            }

            Chessboard tempBoard = board.Clone();
            tempBoard.Make_Move(move);

            return score;
        }
        public int GetColor()
        {
            return playerColor;
        }

    }
}