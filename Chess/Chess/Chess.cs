using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess{
    public class Chess{
        public const int nRows = 8;
        public const int nCols = 8;
        public static Piece InterpretMove(string moveDescr = "", bool isWhite = true){
            int row;
            int col;
            string piece;
            while (true) {
                Console.WriteLine("Type a 3-character string (piece+column+row)");
                if(moveDescr == "")
                    moveDescr = Console.ReadLine().ToLower();
                if(moveDescr == "0-0" || moveDescr == "o-o") {
                    moveDescr = isWhite ? "kg1" : "kg8";
                }else if(moveDescr == "0-0-0" || moveDescr == "o-o-o") {
                    moveDescr = isWhite ? "kc1" : "kc8";
                } else if (moveDescr.Count() == 2){
                    moveDescr = "p" + moveDescr;
                }else if (moveDescr.Count() != 3){
                    Console.WriteLine("Invalid move - must be three characters (or two for pawn moves");
                    moveDescr = "";
                    continue;
                }
                piece = moveDescr[0].ToString().ToUpper();
                col = (int)moveDescr[1] - 97;
                row = (int)moveDescr[2] - 49;
                if (!"PRNBQK".Contains(piece)) {
                    Console.WriteLine("Invalid piece (first letter)");
                    moveDescr = "";
                }
                if (col < 0 || col >= nCols) {
                    Console.WriteLine("Invalid column letter (only a to " + (char)((int)'a' + nCols - 1) + " allowed)");
                    moveDescr = "";
                }
                if (col < 0 || col >= nCols) {
                    Console.WriteLine("Invalid row number (only 1 to " + nRows + " allowed)");
                    moveDescr = "";
                }
                if(moveDescr!=""){
                    return new Piece(row, col, isWhite, piece);
                }
                moveDescr = "";
            }
        }

        public static bool InsideBoard(int row, int col){
            return row>=0 && row<nRows && col>=0 && col<nCols;
        }

        public class Piece{
            public int Value;
            public string symbol;
            public string ToString() {
                return (IsWhite ? "" : "b") + symbol;
            }
            public int[][] incrs;// { return new int[][] { }; }
            public int maxSteps;// { return 8; }
            public int[][] takeIncrs;// {return null;}
            FieldStatus oppositeColor(){return IsWhite ? FieldStatus.Black : FieldStatus.White;}
            public bool IsWhite;
            public int Row;
            public int Column;
            public Piece(int row, int column, bool isWhite = true){
                Row=row;
                Column=column;
                IsWhite = isWhite;
            }
            public List<Piece> Moves(FieldStatus[,] Board){
                var moves = new List<Piece>();
                foreach (var incr in incrs) {
                    int ii = incr[0];
                    int jj = incr[1];
                    int steps = 1;
                    while (InsideBoard(Row + ii, Column + jj) && Board[Row + ii, Column + jj] == FieldStatus.Empty && steps <= maxSteps ) {
                        moves.Add(new Piece(Row + ii, Column + jj, IsWhite, symbol));//new P(Row + ii, Column + jj, IsWhite));
                        ii += incr[0];
                        jj += incr[1];
                        steps += 1;
                    }
                    if (takeIncrs == null && InsideBoard(Row + ii, Column + jj) && Board[Row + ii, Column + jj] == oppositeColor() && steps<= maxSteps) {
                        moves.Add(new Piece(Row + ii, Column + jj, IsWhite, symbol));
                    }
                }
                if (takeIncrs != null) {
                    foreach (var incr in takeIncrs) {
                        int ii = incr[0];
                        int jj = incr[1];
                        if (InsideBoard(Row + ii, Column + jj) && Board[Row + ii, Column + jj] == oppositeColor()) {
                            moves.Add(new Piece(Row + ii, Column + jj, IsWhite, symbol));
                        }
                    }
                }
                return moves;
            }
            public Piece(int row, int column, bool isWhite, string type) {
                symbol = type;
                Row = row;
                Column = column;
                IsWhite = isWhite;
                switch(type){
                    case "P":
                        incrs = IsWhite ? new int[][] { new[] { 1, 0 } } : new int[][] { new[] { -1, 0 } };
                        takeIncrs = IsWhite ? new int[][] { new[] { 1, 1 }, new[]{1, -1 }} : new int[][] { new[] { -1, 1 }, new[] { -1, -1 } };
                        maxSteps = IsWhite ? (Row == 1 ? 2 : 1) : Row == 6 ? 2 : 1;
                        Value = 1;
                        break;
                    case "N":
                        incrs = new int[][] { new[] { -1, -2 }, new[] { -1, 2 }, new[] { -2, -1 }, new[] { -2, 1 }, new[] { 1, -2 }, new[] { 1, 2 }, new[] { 2, -1 }, new[] { 2, 1 } };
                        takeIncrs = incrs;
                        maxSteps = 1;
                        Value = 3;
                        break;
                    case "B":
                        incrs = new int[][] { new[] { -1, -1 }, new[] { 1, -1 }, new[] { -1, 1 }, new[] { 1, 1 } };
                        maxSteps = Math.Min(nRows, nCols);
                        Value = 3;
                        break;
                    case "R":
                        incrs = new int[][] { new[] { -1, 0 }, new[] { 1, 0 }, new[] { 0, 1 }, new[] { 0, -1 } };
                        maxSteps = Math.Max(nRows, nCols);
                        Value = 5;
                        break;
                    case "Q":
                        incrs = new int[][] { new[] { -1, -1 }, new[] { 1, -1 }, new[] { -1, 1 }, new[] { 1, 1 }, new[] { -1, 0 }, new[] { 1, 0 }, new[] { 0, 1 }, new[] { 0, -1 } };
                        maxSteps = Math.Max(nRows, nCols);
                        Value = 9;
                        break;
                    case "K":
                        incrs = new int[][] { new[] { -1, -1 }, new[] { 1, -1 }, new[] { -1, 1 }, new[] { 1, 1 }, new[] { -1, 0 }, new[] { 1, 0 }, new[] { 0, 1 }, new[] { 0, -1 } };
                        maxSteps = 1;
                        Value = 1000;
                        break;
                    default:
                        throw new Exception(type + " is not a valid Piece symbol.");
                }
            }

            public static Piece newPawn(int row, int col, bool isWhite=true){
                return new Piece(row, col, isWhite, "P");
            }
            public static Piece newKnight(int row, int col, bool isWhite = true) {
                return new Piece(row, col, isWhite, "N");
            }
            public static Piece newBishop(int row, int col, bool isWhite = true) {
                return new Piece(row, col, isWhite, "B");
            }
            public static Piece newRook(int row, int col, bool isWhite = true) {
                return new Piece(row, col, isWhite, "R");
            }
            public static Piece newQueen(int row, int col, bool isWhite = true) {
                return new Piece(row, col, isWhite, "Q");
            }
            public static Piece newKing(int row, int col, bool isWhite = true) {
                return new Piece(row, col, isWhite, "K");
            }
            
        }
        //public class Pawn : Piece {
        //    public override int Value() { return 1; }
        //    public override string symbol() { return "P"; }
        //    public override int[][] incrs() { return IsWhite ? new int[][] { new[] { 1, 0 } } : new int[][] { new[] { -1, 0 } }; }
        //    public override int[][] takeIncrs() { return IsWhite ? new int[][] { new[] { 1, 1 }, new[]{1, -1 }} : new int[][] { new[] { -1, 1 }, new[] { -1, -1 } }; }
        //    public override int maxSteps() { return IsWhite ? (Row == 1 ? 2 : 1) : Row == 6 ? 2 : 1; }
        //    public Pawn(int row, int column, bool isWhite = true) : base(row, column, isWhite) { }
        //    public override List<Piece> Moves(FieldStatus[,] Board) {
        //        var moves = new List<Piece>();
        //        FieldStatus oppositeColor = IsWhite ? FieldStatus.Black : FieldStatus.White;
        //        var incrs = IsWhite ? new int[][] { new[] { 1, 0 } } : new int[][] { new[] { -1, 0 } };
        //        var max = IsWhite ? (Row == 1 ? 2 : 1) : Row == 6 ? 2 : 1;
        //        foreach (var incr in incrs) {
        //            int ii = incr[0];
        //            int jj = incr[1];
        //            while (InsideBoard(Row + ii, Column + jj) && Board[Row + ii, Column + jj] == FieldStatus.Empty && ii/incr[0] <= max) {
        //                moves.Add(new Pawn(Row + ii, Column + jj, IsWhite));
        //                ii += incr[0];
        //                jj += incr[1];
        //            }
        //        }
        //        foreach(var incr in takeIncrs()){
        //            if (InsideBoard(Row + incr[0], Column + incr[1]) && Board[Row + incr[0], Column + incr[1]] == oppositeColor) {
        //                moves.Add(new Pawn(Row + incr[0], Column + incr[1], IsWhite));
        //            }
        //        }
        //        return moves;
        //    }
        //}
        //public class Knight:Piece{
        //    public override int Value() { return 3; }
        //    public override string symbol() { return "N"; }
        //    public override int[][] incrs() { return new int[][] { new[] { -1, -2 }, new[] { -1, 2 }, new[] { -2, -1 }, new[] { -2, 1 }, new[] { 1, -2 }, new[] { 1, 2 }, new[] { 2, -1 }, new[] { 2, 1 } }; }
        //    public override int[][] takeIncrs() { return null; }
        //    public override int maxSteps() { return 1; }
        //    public Knight(int row, int column, bool isWhite = true) : base(row, column, isWhite) { }
        //    public override List<Piece> Moves(FieldStatus[,] Board) {
        //        var moves = new List<Piece>();
        //        FieldStatus oppositeColor = IsWhite ? FieldStatus.Black : FieldStatus.White;
        //        var incrs = new int[][] { new[] { -1, -2 }, new[] { -1, 2 }, new[] { -2, -1 }, new[] { -2, 1 }, new[] { 1, -2 }, new[] { 1, 2 }, new[] { 2, -1 }, new[] { 2, 1 } };
        //        var max = 1;
        //        foreach (var incr in incrs) {
        //            int ii = incr[0];
        //            int jj = incr[1];
        //            while (InsideBoard(Row + ii, Column + jj) && Board[Row + ii, Column + jj] == FieldStatus.Empty && ii / incr[0] <= max) {
        //                moves.Add(new Knight(Row + ii, Column + jj, IsWhite));
        //                ii += incr[0];
        //                jj += incr[1];
        //            }
        //            if (InsideBoard(Row + incr[0], Column + incr[1]) && Board[Row + incr[0], Column + incr[1]] == oppositeColor) {
        //                moves.Add(new Knight(Row + incr[0], Column + incr[1], IsWhite));
        //            }
        //        }
        //        return moves;
        //    }
        //}
        //public class Bishop:Piece{
        //    public override int Value() { return 3; }
        //    public override string symbol() { return "B"; }
        //    public override int[][] incrs() { return new int[][] { new[] { -1, -1 }, new[] { 1, -1 }, new[] { -1, 1 }, new[] { 1, 1 } }; }
        //    public override int[][] takeIncrs() { return null; }
        //    public override int maxSteps() { return 8; }
        //    public Bishop(int row, int column, bool isWhite = true) : base(row, column, isWhite) { }
        //    public override List<Piece> Moves(FieldStatus[,] Board) {
        //        var moves = new List<Piece>();
        //        FieldStatus sameColor = IsWhite ? FieldStatus.White : FieldStatus.Black;
        //        foreach(var incr in new int[][]{new[]{-1, -1}, new[]{1, -1}, new[]{-1, 1}, new[]{1, 1}}){
        //            int ii = incr[0];
        //            int jj = incr[1];
        //            while (InsideBoard(Row + ii, Column + jj) && Board[Row + ii, Column + jj] == FieldStatus.Empty) {
        //                moves.Add(new Bishop(Row + ii, Column + jj, IsWhite));
        //                ii+=incr[0];
        //                jj+=incr[1];
        //            }
        //            if (InsideBoard(Row + ii, Column + jj) && Board[Row + ii, Column + jj] != sameColor) {
        //                moves.Add(new Bishop(Row + ii, Column + jj, IsWhite));
        //            }
        //        }
        //        return moves;
        //    }
        //}
        //public class Rook:Piece{
        //    public override int Value() { return 5; }
        //    public override string symbol() { return "R"; }
        //    public override int[][] incrs() { return new int[][] { new[] { -1, 0 }, new[] { 1, 0 }, new[] { 0, 1 }, new[] { 0, -1 } }; }
        //    public override int[][] takeIncrs() { return null; }
        //    public override int maxSteps() { return 8; }
        //    public Rook(int row, int column, bool isWhite = true) : base(row, column, isWhite) { }
        //    public override List<Piece> Moves(FieldStatus[,] Board) {
        //        var moves = new List<Piece>();
        //        FieldStatus sameColor = IsWhite ? FieldStatus.White : FieldStatus.Black;
        //        foreach(var incr in new int[][]{new[]{-1, 0}, new[]{1, 0}, new[]{0, 1}, new[]{0, -1}}){
        //            int ii = incr[0];
        //            int jj = incr[1];
        //            while (InsideBoard(Row + ii, Column + jj) && Board[Row + ii, Column + jj] == FieldStatus.Empty) {
        //                moves.Add(new Rook(Row + ii, Column + jj, IsWhite));
        //                ii+=incr[0];
        //                jj+=incr[1];
        //            }
        //            if (InsideBoard(Row + ii, Column + jj) && Board[Row + ii, Column + jj] != sameColor) {
        //                moves.Add(new Rook(Row + ii, Column + jj, IsWhite));
        //            }
        //        }
        //        return moves;
        //    }
        //}
        //public class Queen:Piece{
        //    public override int Value() { return 9; }
        //    public override string symbol() { return "Q"; }
        //    public override int[][] incrs() { return new int[][] { new[] { -1, -1 }, new[] { 1, -1 }, new[] { -1, 1 }, new[] { 1, 1 }, new[] { -1, 0 }, new[] { 1, 0 }, new[] { 0, 1 }, new[] { 0, -1 } }; }
        //    public override int[][] takeIncrs() { return null; }
        //    public override int maxSteps() { return 8; }
        //    public Queen(int row, int column, bool isWhite = true) : base(row, column, isWhite) { }
        //    public override List<Piece> Moves(FieldStatus[,] Board) {
        //        var moves = new List<Piece>();
        //        FieldStatus sameColor = IsWhite ? FieldStatus.White : FieldStatus.Black;
        //        foreach(var incr in new int[][]{new[]{-1, -1}, new[]{1, -1}, new[]{-1, 1}, new[]{1, 1}, new[]{-1, 0}, new[]{1, 0}, new[]{0, 1}, new[]{0, -1}}){
        //            int ii = incr[0];
        //            int jj = incr[1];
        //            while (InsideBoard(Row + ii, Column + jj) && Board[Row + ii, Column + jj] == FieldStatus.Empty) {
        //                moves.Add(new Queen(Row + ii, Column + jj, IsWhite));
        //                ii+=incr[0];
        //                jj+=incr[1];
        //            }
        //            if (InsideBoard(Row + ii, Column + jj) && Board[Row + ii, Column + jj] != sameColor) {
        //                moves.Add(new Queen(Row + ii, Column + jj, IsWhite));
        //            }
        //        }
        //        return moves;
        //    }
        //}
        //public class King:Piece{
        //    public override int Value() { return 500; }
        //    public override string symbol() { return "K"; }
        //    public override int[][] incrs() { return new int[][] { new[] { -1, -1 }, new[] { 1, -1 }, new[] { -1, 1 }, new[] { 1, 1 }, new[] { -1, 0 }, new[] { 1, 0 }, new[] { 0, 1 }, new[] { 0, -1 } }; }
        //    public override int[][] takeIncrs() { return null; }
        //    public override int maxSteps() { return 1; }
        //    public King(int row, int column, bool isWhite = true) : base(row, column, isWhite) { }
        //    public override List<Piece> Moves(FieldStatus[,] Board){ return Moves2<King>(Board);}
        //}
        //public enum Column{
        //    A=1, B, C, D, E, F, G, H
        //}

        public enum FieldStatus {
            Empty,
            White,
            Black,
        }

        public class Game {
            public FieldStatus[,] Board;
            public List<Piece> WhitePosition;
            public List<Piece> BlackPosition;
            public Game(List<Piece> whitePosition, List<Piece> blackPosition, bool whiteNext = true, bool whiteCanCastleShort = true, bool blackCanCastleShort = true, bool whiteCanCastleLong = true, bool blackCanCastleLong = true) {
                WhitePosition = whitePosition;
                BlackPosition = blackPosition;
                Board = new FieldStatus[8, 8];
                UpdateBoard();
                WhiteNext = whiteNext;
                WhiteCanCastleShort = whiteCanCastleShort;
                BlackCanCastleShort = blackCanCastleShort;
                WhiteCanCastleLong = whiteCanCastleLong;
                BlackCanCastleLong = blackCanCastleLong;
                evaluate = evaluateWithPieceValueOnly;
            }
            public bool WhiteNext;
            public bool WhiteCanCastleShort;
            public bool BlackCanCastleShort;
            public bool WhiteCanCastleLong;
            public bool BlackCanCastleLong;
            public Game() {
                WhitePosition = new List<Piece>{
                    Piece.newPawn(1, 0),
                    Piece.newPawn(1, 1),
                    Piece.newPawn(1, 2),
                    Piece.newPawn(1, 3),
                    Piece.newPawn(1, 4),
                    Piece.newPawn(1, 5),
                    Piece.newPawn(1, 6),
                    Piece.newPawn(1, 7),
                    Piece.newRook(0, 0),
                    Piece.newKnight(0, 1),
                    Piece.newBishop(0, 2),
                    Piece.newQueen(0, 3),
                    Piece.newKing(0, 4),
                    Piece.newBishop(0, 5),
                    Piece.newKnight(0, 6),
                    Piece.newRook(0, 7)
                };
                BlackPosition = new List<Piece>{
                    Piece.newPawn(6, 0, false),
                    Piece.newPawn(6, 1, false),
                    Piece.newPawn(6, 2, false),
                    Piece.newPawn(6, 3, false),
                    Piece.newPawn(6, 4, false),
                    Piece.newPawn(6, 5, false),
                    Piece.newPawn(6, 6, false),
                    Piece.newPawn(6, 7, false),
                    Piece.newRook(7, 0, false),
                    Piece.newKnight(7, 1, false),
                    Piece.newBishop(7, 2, false),
                    Piece.newQueen(7, 3, false),
                    Piece.newKing(7, 4, false),
                    Piece.newBishop(7, 5, false),
                    Piece.newKnight(7, 6, false),
                    Piece.newRook(7, 7, false)
                };
                Board = new FieldStatus[nRows,nCols];
                for (int i = 0; i <= 7; i++) {
                    Board[0, i] = FieldStatus.White;
                    Board[1, i] = FieldStatus.White;
                    Board[6, i] = FieldStatus.Black;
                    Board[7, i] = FieldStatus.Black;
                }
                WhiteNext = true;
                WhiteCanCastleShort = true;
                BlackCanCastleShort = true;
                WhiteCanCastleLong = true;
                BlackCanCastleLong = true;
                evaluate = evaluateWithPieceValueOnly;
            }
            public void UpdateBoard(){
                for (var rr = 0; rr < nRows; rr++) {
                    for (var cc = 0; cc < nCols; cc++) {
                        Board[rr, cc] = FieldStatus.Empty;
                    }
                }
                foreach (var piece in WhitePosition) {
                    Board[piece.Row, piece.Column] = FieldStatus.White;
                }
                foreach (var piece in BlackPosition) {
                    Board[piece.Row, piece.Column] = FieldStatus.Black;
                }
            }
            public override string ToString() {
                var BoardString = new string[nRows,nCols];
                foreach (var piece in BlackPosition.Union(WhitePosition)) {
                    BoardString[piece.Row, piece.Column] = piece.ToString();
                }
                var sb = new StringBuilder();
                sb.Append("----A---B---C---D---E---F---G---H--" + Environment.NewLine);
                for (var rr = nRows-1; rr >= 0; rr--) {
                    sb.Append((rr + 1).ToString() + "| ");
                    for (var cc = 0; cc < nCols; cc++) {
                        sb.Append((BoardString[rr, cc] ?? "").PadRight(4, ' '));
                    }
                    sb.Append(Environment.NewLine);
                }
                return sb.ToString();
            }
            public List<Game> GetWhiteMoves() {
                var games = new List<Game>();
                foreach (var piece in WhitePosition) {
                    foreach (var move in piece.Moves(Board)) {
                        var newWhitePosition = new List<Piece>(WhitePosition);
                        newWhitePosition.Remove(piece);
                        newWhitePosition.Add(move);
                        var newBlackPosition = new List<Piece>(BlackPosition);
                        newBlackPosition.RemoveAll(x=> x.Row == move.Row && x.Column == move.Column);
                        games.Add(new Game(newWhitePosition, newBlackPosition, false, WhiteCanCastleShort, BlackCanCastleShort, WhiteCanCastleLong, BlackCanCastleLong));
                    }
                }
                if(WhiteCanCastleLong) {
                    if(Board[0,0] == FieldStatus.White && Board[0,1] == FieldStatus.Empty && Board[0, 2] == FieldStatus.Empty && Board[0, 3] == FieldStatus.Empty && Board[0,4] == FieldStatus.White) {
                        if(WhitePosition.Any(x=> x.Column == 0 && x.Row == 0 && x.symbol == "R") &&
                            WhitePosition.Any(x => x.Column == 0 && x.Row == 4 && x.symbol == "K")) {
                            var newWhitePosition = new List<Piece>(WhitePosition);
                            newWhitePosition.First(x => x.Column == 0 && x.Row == 0 && x.symbol == "R").Row = 3;
                            newWhitePosition.First(x => x.Column == 0 && x.Row == 4 && x.symbol == "K").Row = 2;
                            games.Add(new Game(newWhitePosition, new List<Piece>(BlackPosition), false, WhiteCanCastleShort, BlackCanCastleShort, false, BlackCanCastleLong));
                        }
                    }
                }
                if(WhiteCanCastleShort) {
                    if(Board[0, 7] == FieldStatus.White && Board[0, 6] == FieldStatus.Empty && Board[0, 5] == FieldStatus.Empty && Board[0, 4] == FieldStatus.White) {
                        if(WhitePosition.Any(x => x.Column == 0 && x.Row == 7 && x.symbol == "R") &&
                            WhitePosition.Any(x => x.Column == 0 && x.Row == 4 && x.symbol == "K")) {
                            var newWhitePosition = new List<Piece>(WhitePosition);
                            newWhitePosition.First(x => x.Column == 0 && x.Row == 7 && x.symbol == "R").Row = 5;
                            newWhitePosition.First(x => x.Column == 0 && x.Row == 4 && x.symbol == "K").Row = 6;
                            games.Add(new Game(newWhitePosition, new List<Piece>(BlackPosition), false, false, BlackCanCastleShort, WhiteCanCastleLong, BlackCanCastleLong));
                        }
                    }
                }
                return games;
            }
            public List<Game> GetBlackMoves() {
                var games = new List<Game>();
                foreach (var piece in BlackPosition) {
                    foreach (var move in piece.Moves(Board)) {
                        var newBlackPosition = new List<Piece>(BlackPosition);
                        newBlackPosition.Remove(piece);
                        newBlackPosition.Add(move);
                        var newWhitePosition = new List<Piece>(WhitePosition);
                        newWhitePosition.RemoveAll(x => x.Row == move.Row && x.Column == move.Column);
                        games.Add(new Game(newWhitePosition, newBlackPosition, true, WhiteCanCastleShort, BlackCanCastleShort, WhiteCanCastleLong, BlackCanCastleLong));
                    }
                }
                if(BlackCanCastleLong) {
                    if(Board[7, 0] == FieldStatus.Black && Board[7, 1] == FieldStatus.Empty && Board[7, 2] == FieldStatus.Empty && Board[7, 3] == FieldStatus.Empty && Board[7, 4] == FieldStatus.Black) {
                        if(BlackPosition.Any(x => x.Column == 7 && x.Row == 0 && x.symbol == "R") &&
                            BlackPosition.Any(x => x.Column == 7 && x.Row == 4 && x.symbol == "K")) {
                            var newBlackPosition = new List<Piece>(BlackPosition);
                            newBlackPosition.First(x => x.Column == 7 && x.Row == 0 && x.symbol == "R").Row = 3;
                            newBlackPosition.First(x => x.Column == 7 && x.Row == 4 && x.symbol == "K").Row = 2;
                            games.Add(new Game(new List<Piece>(WhitePosition), newBlackPosition, false, WhiteCanCastleShort, false, WhiteCanCastleLong, BlackCanCastleLong));
                        }
                    }
                }
                if(BlackCanCastleShort) {
                    if(Board[7, 7] == FieldStatus.Black && Board[7, 6] == FieldStatus.Empty && Board[7, 5] == FieldStatus.Empty && Board[7, 4] == FieldStatus.Black) {
                        if(BlackPosition.Any(x => x.Column == 7 && x.Row == 7 && x.symbol == "R") &&
                            BlackPosition.Any(x => x.Column == 7 && x.Row == 4 && x.symbol == "K")) {
                            var newBlackPosition = new List<Piece>(BlackPosition);
                            newBlackPosition.First(x => x.Column == 7 && x.Row == 0 && x.symbol == "R").Row = 5;
                            newBlackPosition.First(x => x.Column == 7 && x.Row == 4 && x.symbol == "K").Row = 6;
                            games.Add(new Game(new List<Piece>(WhitePosition), newBlackPosition, false, WhiteCanCastleShort, BlackCanCastleShort, WhiteCanCastleLong, false));
                        }
                    }
                }
                return games;
            }
            public List<Game> GetNextMoves() {
                return WhiteNext ? GetWhiteMoves() : GetBlackMoves();
            }
            public Func<double> evaluate;

            public double evaluateWithPieceValueOnly() {
                return (WhitePosition.Sum(x => x.Value) - BlackPosition.Sum(x => x.Value));
            }
            public double evaluateWithPieceValueAndNumberOfMoves() {
                return (WhitePosition.Sum(x => x.Value) - BlackPosition.Sum(x => x.Value)) + (GetWhiteMoves().Count - GetBlackMoves().Count)/1000;
            }
            public bool ValidMove(Piece move) {
                return move.IsWhite == WhiteNext && (WhiteNext ?
                    WhitePosition.Where(x => x.symbol == move.symbol).SelectMany(y => y.Moves(Board)).Any(x => x.Row == move.Row && x.Column == move.Column)
                    :
                    BlackPosition.Where(x => x.symbol == move.symbol).SelectMany(y => y.Moves(Board)).Any(x => x.Row == move.Row && x.Column == move.Column));
                    //TODO: Legg inn rokade-håndtering her
            }
            public Game Move(Piece move) {
                if (move.IsWhite) {
                    Piece piece = WhitePosition.First(x => x.symbol == move.symbol && x.Moves(Board).Any(y=>y.Row == move.Row && y.Column == move.Column));
                    var newWhitePosition = new List<Piece>(WhitePosition);
                    newWhitePosition.Remove(piece);
                    newWhitePosition.Add(move);
                    var newBlackPosition = new List<Piece>(BlackPosition);
                    newBlackPosition.RemoveAll(x => x.Row == move.Row && x.Column == move.Column);
                    return new Game(newWhitePosition, newBlackPosition, false);
                } else {
                    Piece piece = BlackPosition.Where(x => x.symbol == move.symbol).SelectMany(y => y.Moves(Board)).First(x => x.Row == move.Row && x.Column == move.Column);
                    var newBlackPosition = new List<Piece>(BlackPosition);
                    newBlackPosition.Remove(piece);
                    newBlackPosition.Add(move);
                    var newWhitePosition = new List<Piece>(WhitePosition);
                    newWhitePosition.RemoveAll(x => x.Row == move.Row && x.Column == move.Column);
                    return new Game(newWhitePosition, newBlackPosition);
                }
            }
        }
    }
}
