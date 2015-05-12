namespace BattleShipsLib
{
    public enum Orientation
    {
        Vertical,
        Horizontal
    }

    public abstract class Ship
    {
        public int Front { get; protected set; }
        public int Rear { get; protected set; }
        public int Squares { get; protected set; }
        public Orientation Orientation { get; set; }
        public string Coordinates { get; set; }
        public bool IsDestroyed { get { return Damage == Squares; } }
        public int Damage { get; set; }
        public string Name { get; set; }
    }

    public class BattleShip : Ship
    {
        public BattleShip()
        {
            Front = 2;
            Rear = 2;
            Squares = 5;
        }
    }

    public class Destroyer : Ship
    {
        public Destroyer()
        {
            Front = 2;
            Rear = 1;
            Squares = 4;
        }
    }
}