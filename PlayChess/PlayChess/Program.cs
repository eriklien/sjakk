using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chess;

namespace PlayChess {
    class Program {
        static void Main(string[] args) {
            var game = new Chess.Chess.Game();
            var secondsPerMove = 20;
            while (game.BlackPosition.Any(x => x.symbol == "K") && game.WhitePosition.Any(x => x.symbol == "K")) {
                Chess.Chess.Piece move;
                move = Chess.Chess.InterpretMove();
                while (!game.ValidMove(move)) {
                    Console.WriteLine("Invalid move, try again");
                    move = Chess.Chess.InterpretMove();
                }
                game = game.Move(move);
                Console.Write(game.ToString());
                var tankerekke = GameNode.GetBestMoves(game, DateTime.Now.AddSeconds(secondsPerMove));
                game = tankerekke.BestNode.game;
                Console.Write(game.ToString());
            }
            return;
            //while (tankerekke.game.BlackPosition.Any(x => x.symbol == "K") && tankerekke.game.WhitePosition.Any(x => x.symbol == "K")) {
            //    Console.Write(tankerekke.BestNode.game.ToString());
            //    Console.WriteLine("--------------------------------");
            //    tankerekke = GameNode.GetBestMoves(tankerekke.BestNode.game, 4);
            //}

            //while(tankerekke != null){
            //    Console.Write(tankerekke.game.ToString());
            //    Console.WriteLine("--------------------------------");
            //    tankerekke = tankerekke.BestNode;
            //}
        }
    }

    public class GameNode {
        public GameNode BestNode;
        public double score;
        public Chess.Chess.Game game;
        public GameNode(Chess.Chess.Game _game) {
            game = _game;
            //gameTrees = new List<GameTree>();
        }


        public static GameNode GetBestMoves(Chess.Chess.Game _game, DateTime finishTime) {
            GameNode bestMove = GetBestMoves(_game, 1);
            var nSteps = 2;
            var halfTime = DateTime.Now.AddTicks((finishTime - DateTime.Now).Ticks);
            try {
                while(DateTime.Now < halfTime) {
                    bestMove = GetBestMoves(_game, nSteps, finishTime);
                    nSteps = nSteps + 1;
                }
            }catch(TimeoutException) {
                Console.WriteLine("Looking " + (nSteps - 1).ToString() + " steps ahead");
            }
            return bestMove;
        }

        public static GameNode GetBestMoves(Chess.Chess.Game _game, int stepsForward, double alpha = -1000, double beta = 1000) {
            return GetBestMoves(_game, stepsForward, new DateTime(2099, 1, 1), alpha, beta);
        }

        public static GameNode GetBestMoves(Chess.Chess.Game _game, int stepsForward, DateTime finishTime, double alpha = -1000, double beta = 1000) {
            if(DateTime.Now >= finishTime)
                throw new TimeoutException("Time limit exceeded.");
            var gT = new GameNode(_game);
            gT.score = gT.game.WhiteNext ? -1000 : 1000;
            if(stepsForward<=0){
                gT.score = gT.game.evaluate();
                return gT;
            } else {
                if (gT.game.WhiteNext) {
                    foreach (var move in _game.GetNextMoves()) {
                        var goodMoves = GetBestMoves(move, stepsForward - 1, finishTime, alpha, beta);
                        if(goodMoves.score > gT.score){
                            gT.score = goodMoves.score;
                            gT.BestNode = goodMoves;
                        }
                        alpha = Math.Max(alpha, gT.score);
                        if (alpha >= beta)
                            break;
                    }
                }else{
                    foreach (var move in _game.GetNextMoves()) {
                        var goodMoves = GetBestMoves(move, stepsForward - 1, finishTime, alpha, beta);
                        if(goodMoves.score < gT.score){
                            gT.score = goodMoves.score;
                            gT.BestNode = goodMoves;
                        }
                        beta = Math.Min(beta, gT.score);
                        if (alpha >= beta)
                            break;
                    }
                }
            }
            //Console.WriteLine(" " + stepsForward);
            //Console.Write(_game.ToString()); 
            //Console.WriteLine("Score: " + gT.score);
            //Console.WriteLine("--------------------------------");
            return gT;
        }
        public Chess.Chess.Piece getBestMove(double seconds) {
            return new Chess.Chess.Piece(0, 0, true, "R");
        }
    }
}
