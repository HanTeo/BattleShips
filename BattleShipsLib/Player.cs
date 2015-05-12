using System;
using System.Collections.Generic;
using System.Linq;

namespace BattleShipsLib
{
    public abstract class Player
    {
        protected HashSet<string> validCoordinates = new HashSet<string>();

        public string Name { get; set; }

        public Player()
        {
            var size = 10;

            var xRange = Enumerable.Range('A', size)
                              .Select(x => (char)x)
                              .ToList();

            var yRange = Enumerable.Range(1, size).ToList();

            xRange.ForEach(x =>
            {
                yRange.ForEach(y =>
                {
                    var key = string.Format("{0}{1}", x, y);
                    validCoordinates.Add(key);
                });
            });
        }

        public abstract string Move();
    }

    public class Human : Player
    {
        public Queue<string> coordinates = new Queue<string>();

        public override string Move()
        {
            if (coordinates.Count == 0) throw new Exception("No more moves");

            return coordinates.Dequeue();
        }

        public bool AddMove(string coordinate)
        {
            if (!validCoordinates.Contains(coordinate.ToUpper())) return false;

            coordinates.Enqueue(coordinate.ToUpper());

            return true;
        }
    }

    public class Computer : Player
    {
        private List<string> coordinates;
        private Random random = new Random(); 

        public Computer()
        {
            coordinates = new List<string>(validCoordinates);
        }

        public override string Move()
        {
            if (coordinates.Count() == 0) throw new Exception("No more moves");

            int i = random.Next(0, coordinates.Count());
            string addr = coordinates[i];
            coordinates.RemoveAt(i);

            return addr;
        }
    }
}