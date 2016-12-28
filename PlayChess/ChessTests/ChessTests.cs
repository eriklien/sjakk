using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Chess;
using PlayChess;

namespace ChessTests{
    [TestClass]
    public class TestChess{
        private static Chess.Chess.Game newGame;
        public static Chess.Chess.Game getNewGame() {
            if(newGame == null)
                newGame = new Chess.Chess.Game();
            return newGame;
        }
        [TestMethod]
        public void TestNumberOfStartMoves(){
            Assert.IsTrue(getNewGame().GetNextMoves().Count == 20);
        }
        [TestMethod]
        public void TestSomeMoves() {
            Assert.IsTrue(getNewGame().ValidMove(new Chess.Chess.Piece(2, 0, true, "N")));
            Assert.IsFalse(getNewGame().ValidMove(new Chess.Chess.Piece(0, 3, true, "K")));
        }
        [TestMethod]
        public void TestAISpeed() {
            //while(game.BlackPosition.Any(x => x.symbol == "K") && game.WhitePosition.Any(x => x.symbol == "K")) {
            var game = new Chess.Chess.Game();
            for(int ii = 1; ii <= 30; ii++) {
                var tankerekke = GameNode.GetBestMoves(game, 5);
                game = tankerekke.BestNode.game;
                Console.Write(game.ToString());
            }
            Assert.IsTrue(true);
        }
    }
}
