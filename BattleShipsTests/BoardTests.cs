using BattleShipsLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace BattleShipsTests
{
    [TestClass]
    public class GridTests
    {
        private Grid grid;
        private Destroyer destroyer;
        private BattleShip battleShip;

        [TestInitialize]
        public void Setup()
        {
            grid = new Grid();
            destroyer = new Destroyer();
            battleShip = new BattleShip();
        }

        [TestMethod]
        public void GridIsInitialisedWithCorrectCellAddresses()
        {
            Assert.AreEqual("A1-A2-A3-A4-A5-A6-A7-A8-A9-A10-B1-B2-B3-B4-B5-B6-B7-B8-B9-B10-C1-C2-C3-C4-C5-C6-C7-C8-C9-C10-D1-D2-D3-D4-D5-D6-D7-D8-D9-D10-E1-E2-E3-E4-E5-E6-E7-E8-E9-E10-F1-F2-F3-F4-F5-F6-F7-F8-F9-F10-G1-G2-G3-G4-G5-G6-G7-G8-G9-G10-H1-H2-H3-H4-H5-H6-H7-H8-H9-H10-I1-I2-I3-I4-I5-I6-I7-I8-I9-I10-J1-J2-J3-J4-J5-J6-J7-J8-J9-J10", grid.ToString());
        }

        [TestMethod]
        public void GridIsPopulatedWithCellsByDefault()
        {
            foreach(var cell in grid.Values)
            {
                Assert.IsInstanceOfType(cell, typeof(Cell));
            }
        }

        [TestMethod]
        public void GridIsPopulatedWithUnoccupiedCellsByDefault()
        {
            foreach (var cell in grid.Values)
            {
                Assert.IsFalse(cell.IsOccupied);
            }
        }

        [TestMethod]
        public void GridCanAcceptShipPlacementIfGivenValidAddress()
        { 
            Assert.IsTrue(grid.AssignCell(destroyer, "C1"));
        }

        [TestMethod]
        public void GridAssignsCorrectShipToCell()
        {
            Assert.IsTrue(grid.AssignCell(destroyer, "C1"));
            Assert.AreEqual(grid["C1"].Occupier, destroyer);
        }
    }

    [TestClass]
    public class BoardTests
    {
        private Board board;
        private BattleShip battleShip;

        [TestInitialize]
        public void Setup()
        {
            board = new Board();
            battleShip = new BattleShip();
        }

        [TestMethod]
        public void BoardGetFootprintMethodReturnsCorrectShipFootprintVertical()
        {
            Assert.AreEqual("C3,C2,C1,C4,C5",string.Join(",",board.GetShipFootprint(battleShip, "C3")));
        }

        [TestMethod]
        public void BoardGetFoootprintMethodReturnsCorrectShipFootprintHorizontal()
        {
            battleShip.Orientation = Orientation.Horizontal;
            Assert.AreEqual("C3,B3,A3,D3,E3", string.Join(",", board.GetShipFootprint(battleShip, "C3")));
        }

        [TestMethod]
        public void BoardGetFootprintMethodWillOnlyReturnValidCoordinatesTopEdge()
        {
            Assert.AreEqual("C1,C2,C3", string.Join(",", board.GetShipFootprint(battleShip, "C1")));
        }

        [TestMethod]
        public void BoardGetFootprintMethodWillOnlyReturnValidCoordinatesBottomEdge()
        {
            Assert.AreEqual("C10,C9,C8", string.Join(",", board.GetShipFootprint(battleShip, "C10")));
        }

        [TestMethod]
        public void BoardGetFootprintMethodWillOnlyReturnValidCoordinatesLeftEdge()
        {
            battleShip.Orientation = Orientation.Horizontal;
            Assert.AreEqual("A1,B1,C1", string.Join(",", board.GetShipFootprint(battleShip, "A1")));
        }

        [TestMethod]
        public void BoardGetFootprintMethodWillOnlyReturnValidCoordinatesRightEdge()
        {
            battleShip.Orientation = Orientation.Horizontal;
            Assert.AreEqual("J1,I1,H1", string.Join(",", board.GetShipFootprint(battleShip, "J1")));
        }

        [TestMethod]
        public void BoardCanPlaceShipInValidAddress()
        {
            Assert.IsNull(battleShip.Coordinates);
            var placed = board.Place(battleShip, "C3");

            Assert.IsTrue(placed);
            Assert.AreEqual("C3", battleShip.Coordinates);

        }

        [TestMethod]
        public void BoardCannotAcceptShipPlacementIfGivenInvalidAddress()
        {
            Assert.IsFalse(board.Place(battleShip, "Z1"));
            Assert.IsFalse(board.Place(battleShip, "C24"));
        }

        [TestMethod]
        public void BoardAcceptsAddressCaseInsensitive()
        {
            IEnumerable<string> footprint;
            Assert.IsTrue(board.CanPlace(battleShip, "c3", out footprint));
            Assert.IsTrue(board.Place(battleShip, "c3"));
        }

        [TestMethod]
        public void BoardCanAcceptShipPlacementIfCellIsUnoccupied()
        {
            var ship = new Destroyer();

            Assert.IsTrue(board.Place(battleShip, "C3"));
            Assert.IsFalse(board.Place(ship, "C3"));
        }

        [TestMethod]
        public void BoardAttackWillRegisterDamageIfCordinatesAreOccupied()
        {
            Assert.AreEqual(0, battleShip.Damage);
            board.Place(battleShip, "C3");
            var cell = board.Attack("C3");
            Assert.AreEqual(1, battleShip.Damage);
            Assert.IsTrue(cell.IsHit);
            Assert.IsTrue(cell.IsOccupied);
        }

        [TestMethod]
        public void BoardAttackSameCellWillNotRegisterAdditionalDamage()
        {
            Assert.AreEqual(0, battleShip.Damage);
            board.Place(battleShip, "C3");
            var cell = board.Attack("C3");
            Assert.AreEqual(1, battleShip.Damage);
            Assert.IsTrue(cell.IsHit);
            Assert.IsTrue(cell.IsOccupied);
            cell = board.Attack("C3");
            Assert.AreEqual(1, battleShip.Damage);
        }

        [TestMethod]
        public void BoardAttackAdjacentCellWillRegisterAdditionalDamage()
        {
            Assert.AreEqual(0, battleShip.Damage);
            board.Place(battleShip, "C3");
            var cell = board.Attack("C3");
            Assert.AreEqual(1, battleShip.Damage);
            Assert.IsTrue(cell.IsHit);
            Assert.IsTrue(cell.IsOccupied);
            cell = board.Attack("C2");
            Assert.AreEqual(2, battleShip.Damage);
            Assert.IsTrue(cell.IsHit);
            Assert.IsTrue(cell.IsOccupied);
        }

        [TestMethod]
        public void BoardAttackAllShipCellsWillDestroyShip()
        {
            board.Place(battleShip, "C3");

            Assert.AreEqual(0, battleShip.Damage);
            Assert.IsFalse(battleShip.IsDestroyed);

            new List<string> { "C1", "C2", "C3", "C4", "C5" }.ForEach(coordinates => 
            {
                var cell = board.Attack(coordinates);
            });

            Assert.AreEqual(5, battleShip.Damage);
            Assert.IsTrue(battleShip.IsDestroyed);
        }

        [TestMethod]
        public void BoardCanRandomlyCorrectlyAllocateShips()
        {
            board.RandomlyAllocate(new Ship[] {
                new BattleShip { Name = "B1" },
                new Destroyer { Name = "D1" },
                new Destroyer { Name = "D2" } });
            Assert.AreEqual(3, board.Ships.Count);
            Console.WriteLine(board);
        }
    }
}
