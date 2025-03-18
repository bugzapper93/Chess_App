//using Chess_App;
//using Microsoft.VisualBasic.Devices;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using System.Windows.Forms;

//namespace Chess
//{
//    class AIPlayer
//    {
//        private int maxDepth;
//        private int playerColor;
//        private int timeLimitMs;
//        private Dictionary<long, int> transpositionTable = new Dictionary<long, int>();

//        public AIPlayer(int depth, int color, int timeLimit)
//        {
//            maxDepth = depth;
//            playerColor = color;
//            timeLimitMs = timeLimit;
//        }
//        public Move Get_Best_Move(Chessboard board)
//        {
//            List<Move> moves = new List<Move>();
//            Move bestMove = new Move();
//            Move tempBestMove = new Move();
//            Move move;

//            int bestValue = int.MinValue;
//            object lockObj = new object();
//            var possibleMoves = OrderMoves(Moves.Get_All_Possible_Moves(board), board);

//            moves.AddRange(possibleMoves.moves);

//            if (moves.Count == 0)
//            {
//                return new Move();
//            }

//            var startTime = DateTime.Now;

//            foreach (var depth in Enumerable.Range(1, maxDepth))
//            {
//                if ((DateTime.Now - startTime).TotalMilliseconds > timeLimitMs)
//                {
//                    break;
//                }

//                tempBestMove = bestMove;

//                Parallel.ForEach(moves, move =>
//                {
                    
//                    Chessboard tempBoard = board;
//                    tempBoard.Make_Move(move);

//                    int moveValue = Minimax(tempBoard, depth, int.MinValue, int.MaxValue, false);
//                    lock (lockObj)
//                    {
//                        if (moveValue > bestValue)
//                        {
//                            bestValue = moveValue;
//                            tempBestMove = move;
//                        }
//                    }
//                });

//                bestMove = tempBestMove;
//            }

//            return bestMove ?? possibleMoves.First();
//        }
//        public static Moveset OrderMoves(Moveset possibleMoves, Chessboard board)
//        {
//            // Przykład sortowania: sortujemy ruchy na podstawie jakiejś funkcji oceny
//            List<Move> orderedMoves = possibleMoves.moves.OrderBy(move => EvaluateMove(board, move)).ToList();

//            return new Moveset { moves = orderedMoves, pins = possibleMoves.pins };
//        }
//        private int Minimax(Chessboard board, int depth, int alpha, int beta, bool isMaximizing)
//        {
//            long boardHash = board.GetHashCode();
//            if (transpositionTable.TryGetValue(boardHash, out int cachedEval))
//            {
//                return cachedEval;
//            }
//            if (depth == 0) //|| board.IsGameOver())
//            {
//                int eval = EvaluateBoard(board);
//                transpositionTable[boardHash] = eval;
//                return eval;
//            }

//            int value = isMaximizing ? int.MinValue : int.MaxValue;
//            object lockObj = new object();

//            Parallel.ForEach(OrderMoves(moves.(isMaximizing ? playerColor : -playerColor), board), move =>
//            {
//                Chessboard tempBoard = board.Clone();
//                tempBoard.MakeMove(move);
//                int eval = Minimax(tempBoard, depth - 1, alpha, beta, !isMaximizing);

//                lock (lockObj)
//                {
//                    if (isMaximizing)
//                    {
//                        value = Math.Max(value, eval);
//                        alpha = Math.Max(alpha, value);
//                    }
//                    else
//                    {
//                        value = Math.Min(value, eval);
//                        beta = Math.Min(beta, value);
//                    }

//                    if (beta <= alpha)
//                        return;
//                }
//            });

//            transpositionTable[boardHash] = value;
//            return value;
//        }
//        private int EvaluateBoard(Chessboard board)
//        {
//            int whiteScore = 0, blackScore = 0;

//            foreach (var piece in board.pieces)
//            {
//                int pieceValue = Pieces.Get_Piece_Value(piece.value);
//                if ((piece.value & Pieces.White) != 0)
//                    whiteScore += pieceValue;
//                else
//                    blackScore += pieceValue;
//            }

//            int evaluation = whiteScore - blackScore;
//            return evaluation * (playerColor == Pieces.White ? 1 : -1);
//        }
//    }
//}
