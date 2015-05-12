using BattleShipsLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BattleShipsLibTests
{
    [TestClass()]
    public class PlayerTests
    {
        [TestMethod()]
        public void ComputersHavePredefinedMoves()
        {
            var computer = new Computer();
            Assert.IsNotNull(computer.Move());
        }

        [TestMethod()]
        [ExpectedException(typeof(System.Exception), "No more moves")]
        public void ComputersHaveFiniteMoveSets()
        {
            var computer = new Computer();
            while (true)
                computer.Move();
        }

        [TestMethod]
        [ExpectedException(typeof(System.Exception), "No more moves")]
        public void HumansDontHavePredeterminedMovesByDefault()
        {
            var human = new Human();
            human.Move();
        }
    }
}
