using BattleShipsLib;
using System;

namespace BattleShips
{
    class Program
    {
        static void Main(string[] args)
        {
            var key = ConsoleKey.Y;

            do {
                var game = new Game();
                Console.WriteLine("=========== Battle Ships! =================");
                Console.WriteLine("Please Enter Your Name");
                var input = Console.ReadLine();
                var user = new Human { Name = input };
                game.AddPlayer(user);
                game.AddPlayer(new Computer { Name = "Computer" });
                
                while (game.State != GameState.End)
                {
                    //Console.WriteLine(game.Player1Board);
                    //Console.WriteLine(game.Player2Board);

                    if (game.Challenger is Human)
                    {
                        Console.WriteLine("Pick a Target:");
                        if (!user.AddMove(Console.ReadLine()))
                            continue;
                    }

                    game.MoveNext();
                    Console.Clear();
                    Console.WriteLine(game.Log);
                }

                Console.WriteLine("Would you like to play another game (Y/N)?");
                key = Console.ReadKey().Key;
                Console.WriteLine();
            
            } while (key == ConsoleKey.Y);
        }
    }
}
