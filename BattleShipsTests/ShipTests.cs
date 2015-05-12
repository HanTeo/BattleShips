using Microsoft.VisualStudio.TestTools.UnitTesting;
using BattleShipsLib;

namespace BattleShipsTests
{
    [TestClass]
    public class ShipTests
    {
        private BattleShip battleShip;
        private Destroyer destroyer;

        [TestInitialize]
        public void Setup()
        {
            battleShip = new BattleShip();
            destroyer = new Destroyer();
        }

        [TestMethod]
        public void ShipsAreInitialisedWithNoDamage()
        {
            Assert.AreEqual(0, battleShip.Damage);
            Assert.AreEqual(0, destroyer.Damage);
        }

        [TestMethod]
        public void ShipsDestoyedWhenAllSquaresAreDamaged()
        {
            Assert.IsFalse(battleShip.IsDestroyed);
            Assert.IsFalse(destroyer.IsDestroyed);

            battleShip.Damage = 5;
            destroyer.Damage = 4;

            Assert.IsTrue(battleShip.IsDestroyed);
            Assert.IsTrue(destroyer.IsDestroyed);
        }

        [TestMethod]
        public void ShipsAreVerticallyOrientedByDefault()
        {
            Assert.AreEqual(Orientation.Vertical, battleShip.Orientation);
            Assert.AreEqual(Orientation.Vertical, destroyer.Orientation);
        }
    }
}
