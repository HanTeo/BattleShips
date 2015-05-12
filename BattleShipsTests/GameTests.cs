using Microsoft.VisualStudio.TestTools.UnitTesting;
using BattleShipsLib;
using System;

namespace BattleShipsTests
{
    [TestClass]
    public class GameTests
    {
        private Game game;
        private Human human;
        private Computer computer;
        private Human human2;
        private Board board1;
        private Board board2;

        [TestInitialize]
        public void Setup()
        {
            game = new Game();
            human = new Human {Name="Bob"};
            human2 = new Human();
            computer = new Computer { Name="Computer"};
            board1 = new Board();
            board2 = new Board();
        }

        [TestMethod]
        public void BothPlayersInAGameAreInitiallyUnassigned()
        {
            Assert.IsNull(game.Player1);
            Assert.IsNull(game.Player2);
        }

        [TestMethod]
        public void BothPlayersBoardsInAGameAreInitiallyUnassigned()
        {
            Assert.IsNull(game.Player1Board);
            Assert.IsNull(game.Player2Board);
        }

        [TestMethod]
        public void CanAddHumanPlayer()
        {
            game.AddPlayer(human);
            Assert.IsTrue(game.HasPlayer(human));
        }

        [TestMethod]
        public void CanAddComputerPlayer()
        {
            game.AddPlayer(computer);
            Assert.IsTrue(game.HasPlayer(computer));
        }

        [TestMethod]
        public void CanOnlyAddTwoPlayers()
        {
            game.AddPlayer(human);
            Assert.IsTrue(game.HasPlayer(human));
            game.AddPlayer(computer);
            Assert.IsTrue(game.HasPlayer(computer));

            // Won't Add more than 2
            game.AddPlayer(human2);
            Assert.IsFalse(game.HasPlayer(human2));
        }

        [TestMethod]
        public void KnowsWhenItHasTwoPlayers()
        {
            game.AddPlayer(human);
            Assert.IsFalse(game.HasTwoPlayers);
            game.AddPlayer(computer);
            Assert.IsTrue(game.HasTwoPlayers);
        }

        [TestMethod]
        public void BothGameBoardsGetAssignedWhenTwoPlayersAreAdded()
        {
            game.AddPlayer(human);
            Assert.IsNull(game.Player1Board);
            Assert.IsNull(game.Player2Board);

            game.AddPlayer(computer);
            Assert.IsNotNull(game.Player1Board);
            Assert.IsNotNull(game.Player2Board);
        }

        [TestMethod]
        public void BothGameShipsGetAssignedToBoardWhenTwoPlayersAreAdded()
        {
            game.AddPlayer(human);
            Assert.IsNull(game.Player1Board);
            Assert.IsNull(game.Player2Board);

            game.AddPlayer(computer);
            Assert.AreEqual(3, game.Player1Board.Ships.Count);
            Assert.AreEqual(3, game.Player2Board.Ships.Count);
        }

        [TestMethod]
        public void GameStateIsStartAtInit()
        {
            Assert.AreEqual(GameState.Start, game.State);
        }

        [TestMethod]
        public void GameStartsWithNoWinner()
        {
            Assert.IsNull(game.Winner);
        }

        [TestMethod]
        public void GameChallengerOpponentAreUnassignedAtStart()
        {
            Assert.IsNull(game.Challenger);
            Assert.IsNull(game.Opponent);
        }

        [TestMethod]
        public void GameOpponentsAreProperlyAssignedAfterBothPlayersJoined()
        {
            game.AddPlayer(human);
            Assert.IsNull(game.Challenger);
            Assert.IsNull(game.Opponent);

            game.AddPlayer(computer);
            Assert.IsNotNull(game.Challenger);
            Assert.IsNotNull(game.Opponent);
            Assert.AreNotEqual(game.Opponent, game.Challenger);
        }

        [TestMethod]
        public void GameMoveNextDoesNothingWhenThereAreLessThanTwoPlayers()
        {
            game.MoveNext();
            Assert.AreEqual(GameState.Start, game.State);
            Assert.IsTrue(string.IsNullOrEmpty(game.Log));
        }

        [TestMethod]
        public void GameOpponentsAreSwitchedOnMoveNext()
        {
            game.AddPlayer(human);
            human.AddMove("C2");
            game.AddPlayer(computer);

            var opponent = game.Opponent;
            var challenger = game.Challenger;
            game.MoveNext();
            Assert.AreEqual(opponent, game.Challenger);
            Assert.AreEqual(challenger, game.Opponent);
            Assert.IsFalse(string.IsNullOrEmpty(game.Log));
        }

        [TestMethod]
        public void GameCanReachEndStateWhenAllShipsAreDestroyed()
        {
            var computer2 = new Computer { Name = "R2D2" };
            game.AddPlayer(computer2);
            game.AddPlayer(computer);

            while(game.State != GameState.End)
            {
                game.MoveNext();
            }

            Assert.IsNotNull(game.Winner);
            var loserBoard = game.Winner == game.Player1 ? game.Player2Board : game.Player1Board;
            Assert.AreEqual(0, loserBoard.ShipsRemaining);
            Console.WriteLine(game.Log);
        }
    }
}
