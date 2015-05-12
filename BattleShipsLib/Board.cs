using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BattleShipsLib
{
    public class Board
    {
        private Grid Grid { get; set; }
        public ICollection<Ship> Ships { get; set; }
        public int ShipsRemaining {
            get
            {
                var remaining = 0;
                foreach(var s in Ships)
                {
                    if (!s.IsDestroyed)
                        remaining++;
                }
                return remaining;
            }
        }

        public Board()
        {
            Grid = new Grid();
            Ships = new List<Ship>();
        }

        private Tuple<char, int> extractCoordinates(string coordinates)
        {
            char? x = null;
            string y = null;

            foreach(char c in coordinates)
            {
                if (char.IsLetter(c))
                    x = c;

                if (char.IsDigit(c))
                    y += c;
            }

            if (!x.HasValue)
                throw new ArgumentException("Coordinates {0} not in correct format", coordinates);

            return Tuple.Create(x.Value, int.Parse(y));
        }

        public IEnumerable<string> GetShipFootprint(Ship ship, string coordinates)
        {
            Tuple<char, int> xy = extractCoordinates(coordinates.ToUpper());
            var x = xy.Item1;
            var y = xy.Item2;

            yield return coordinates;

            // Increasing
            switch (ship.Orientation)
            {
                case Orientation.Horizontal:
                    var xIndex = Grid.xRange.IndexOf(x);
                    var left = xIndex;
                    var right = xIndex;
                    for (var front = ship.Front; front > 0; front--)
                    {
                        left--;
                        if (left < 0)
                            break;
                        yield return string.Format("{0}{1}", Grid.xRange[left], y);
                    }

                    for (var rear = ship.Rear; rear > 0; rear--)
                    {
                        right++;
                        if (right >= Grid.xRange.Count)
                            break;
                        yield return string.Format("{0}{1}", Grid.xRange[right], y);
                    }
                    break;

                case Orientation.Vertical:
                    var yIndex = Grid.yRange.IndexOf(y);
                    var top = yIndex;
                    var bottom = yIndex;
                    for (var front = ship.Front; front > 0; front--)
                    {
                        top--;
                        if (top < 0)
                            break; ;
                        yield return string.Format("{0}{1}", x, Grid.yRange[top]);
                    }
                    for (var rear = ship.Rear; rear > 0; rear--)
                    {
                        bottom++;
                        if (bottom >= Grid.yRange.Count)
                            break;
                        yield return string.Format("{0}{1}", x, Grid.yRange[bottom]);
                    }
                    break;

                default:
                    throw new ArgumentException("Unhandled Orientation");
            }
        }

        public bool CanPlace(Ship ship, string coordinates, out IEnumerable<string> footprint)
        {
            footprint = null;
            var c = coordinates.ToUpper();
            
            // Invalid Coordinates
            if (!Grid.ContainsKey(c)) return false;
            
            // Out of Bounds
            footprint = GetShipFootprint(ship, c);
            if (ship.Squares > footprint.Count())
            {
                footprint = null;
                return false;
            }
            
            // Occupied
            foreach(var address in footprint)
            {
                if (!Grid.CanAssign(address)) return false;
            }

            return true;
        }

        public bool Place(Ship ship, string coordinates)
        {
            IEnumerable<string> footprint;

            // Guard
            if (!CanPlace(ship, coordinates, out footprint)) return false;

            // Assignment
            foreach(var address in footprint)
            {
                Grid.AssignCell(ship, address);
            }

            ship.Coordinates = coordinates;
            Ships.Add(ship);

            return true;
        }

        public Cell Attack(string coordinates)
        {
            var c = coordinates.ToUpper();

            if (!Grid.ContainsKey(c)) return null;

            var cell = Grid[c];

            // Register Damange
            if (cell.IsOccupied && !cell.IsHit)
            {
                cell.IsHit = true;
                cell.Occupier.Damage += 1;
            }

            return cell;
        }

        public void RandomlyAllocate(IEnumerable<Ship> ships)
        {
            var random = new Random();
            
            foreach (var ship in ships)
            {
                // Random Orientation
                var orientation = random.Next(0, 2) == 0 ? Orientation.Horizontal : Orientation.Vertical;
                ship.Orientation = orientation;

                var placed = false;
                do
                {
                    // Random Position
                    var i = random.Next(0, Grid.Keys.Count);
                    var coord = Grid.Keys.ElementAt(i);
                    placed = Place(ship, coord);
                }
                while (!placed);
            }
        }

        public override string ToString()
        {
            var rows = new StringBuilder();
            foreach(var y in Grid.yRange)
            {
                var row = new List<string>();
                foreach(var x in Grid.xRange)
                {
                    var addr = string.Format("{0}{1}", x, y);
                    var cell = Grid[addr];
                    var ship = cell.Occupier;
                    row.Add(ship == null ? "   " : cell.IsHit ? "(X)" : ship.Name);
                }
                rows.AppendLine(string.Join(" | ", row));
            }
            return rows.ToString();
        }
    }

    public class Cell
    {
        public bool IsOccupied { get { return Occupier != null; } }
        public Ship Occupier { get; set; }
        public bool IsHit { get; set; }
    }

    public class Grid : Dictionary<string, Cell>
    {
        private const int size = 10;

        public readonly List<char> xRange;
        public readonly List<int> yRange;

        public Grid()
        {
            xRange = Enumerable.Range('A', size)
                              .Select(x => (char)x)
                              .ToList();

            yRange = Enumerable.Range(1, size).ToList();

            xRange.ForEach( x =>
            {
                yRange.ForEach( y =>
                {
                    var key = string.Format("{0}{1}", x, y);
                    Add(key, new Cell());
                });
            });
        }

        public bool CanAssign(string coordinates)
        {
            Cell cell;

            if (!TryGetValue(coordinates.ToUpper(), out cell)) return false;
            if (cell.IsOccupied) return false; 

            return true;
        }

        public bool AssignCell(Ship ship, string coordinates)
        {
            var cell = this[coordinates];
            cell.Occupier = ship;

            return true;
        }

        public override string ToString()
        {
            return string.Join("-", Keys.ToArray());
        }
    }

}