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
            Console.Write(game);
            while (game.BlackPosition.Any(x => x.symbol == "K") && game.WhitePosition.Any(x => x.symbol == "K")) {
                Chess.Chess.Piece move;
                do {
                    move = Chess.Chess.InterpretMove();
                } while (!game.ValidMove(move));
                game = game.Move(move);
                Console.Write(game.ToString());

                var tankerekke = GameNode.GetBestMoves(game, 5);
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
        public int score;
        public Chess.Chess.Game game;
        public GameNode(Chess.Chess.Game _game) {
            game = _game;
            //gameTrees = new List<GameTree>();
        }

        public static GameNode GetBestMoves(Chess.Chess.Game _game, int stepsForward, int alpha = -1000, int beta = 1000) {
            var gT = new GameNode(_game);
            gT.score = gT.game.WhiteNext ? -1000 : 1000;
            if(stepsForward<=0){
                gT.score = gT.game.evaluate();
                return gT;
            } else {
                if (gT.game.WhiteNext) {
                    foreach (var move in _game.GetNextMoves()) {
                        var goodMoves = GetBestMoves(move, stepsForward - 1, alpha, beta);
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
                        var goodMoves = GetBestMoves(move, stepsForward - 1, alpha, beta);
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
    }
}
