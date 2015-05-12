using System;
using System.Text;

namespace BattleShipsLib
{
    public enum GameState
    {
        Start,
        Playing,
        End
    }

    public class Game
    {
        public Player Player1 { get; private set; }
        public Player Player2 { get; private set; }
        public Board Player1Board { get; private set; }
        public Board Player2Board { get; private set; }
        public int Round { get; private set; }
        public Player Challenger { get; private set; }
        public Player Opponent { get; private set; }
        public Player Winner { get; private set; }
        public GameState State { get; private set; }
        public string Log { get { return sb.ToString(); } }
        public bool HasTwoPlayers
        {
            get
            {
                if (Player1 == null || Player2 == null)
                {
                    return false;
                }

                return true;
            }
        }

        private StringBuilder sb = new StringBuilder();
        private void GenerateBoards()
        {
            Player1Board = new Board();
            Player2Board = new Board();
        }

        public void AddPlayer(Player player)
        {
            // Guard Condition
            if (HasTwoPlayers) { return; }

            // Assignment
            if (Player1 == null)
            {
                Player1 = player;
            }
            else if (Player2 == null)
            {
                Player2 = player;

                // Game Boards Are Initialised after second player is assigned
                GetReady();
            }
        }

        public void GetReady()
        {
            // Boards
            GenerateBoards();

            // Ships
            var ships1 = new Ship[] {
                new BattleShip { Name = "USS Voyager" },
                new Destroyer { Name = "USS Enterprise" },
                new Destroyer { Name = "USS Darkstar" } };

            var ships2 = new Ship[] {
                new BattleShip { Name = "HMS Dolphin" },
                new Destroyer { Name = "HMS Shark" },
                new Destroyer { Name = "HMS Stingray" } };

            Player1Board.RandomlyAllocate(ships1);
            Player2Board.RandomlyAllocate(ships2);

            // Random Player Starts
            var random = new Random();
            if(random.Next(1, 3) == 1)
            {
                Challenger = Player2;
                Opponent = Player1;
            }
            else
            {
                Challenger = Player1;
                Opponent = Player2;
            }
        }

        public bool HasPlayer(Player player)
        {
            if (Player1 == player) return true;
            if (Player2 == player) return true;

            return false;
        }

        public void MoveNext()
        {
            if (State == GameState.End) return;
            if (!HasTwoPlayers) return;
            if (State == GameState.Start)
            {
                State = GameState.Playing;
            }
           
            if (Round % 2 == 0)
            {
                sb.AppendFormat("===================== Round {0} ======================\n", Round / 2 + 1);
            }
        
            Cell cell = null;
            Board target = null;

            var address = Challenger.Move();

            if (Challenger == Player1)
                target = Player2Board;

            if (Challenger == Player2)
                target = Player1Board;

            cell = target.Attack(address);

            // Hit
            if (cell.IsHit)
            {
                sb.AppendFormat("    {0} Shoots at {1} and Hits!", Challenger.Name, address);
                sb.AppendLine();
                if (cell.Occupier.IsDestroyed)
                {
                    sb.AppendFormat("    {0}'s Ship {1} Destroyed!! {2} Ships Remaining", Opponent.Name, cell.Occupier.Name, target.ShipsRemaining);
                    sb.AppendLine();
                }
            }
            else
            {
                sb.AppendFormat("    {0} Shoots at {1} and Misses", Challenger.Name, address);
                sb.AppendLine();

            }

            // End Game
            if(target.ShipsRemaining == 0)
            {
                State = GameState.End;
                Winner = Challenger;

                sb.AppendFormat("================ Game Over! {0} Wins !!! ===================", Winner.Name, Round);
                sb.AppendLine();
                return;
            }

            // Switch
            SwitchTurns();
        }

        public void SwitchTurns()
        {
            var temp = Challenger;
            Challenger = Opponent;
            Opponent = temp;
            Round++;
        }
    }
}
