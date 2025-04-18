using System;
using System.Collections.Generic;
using System.IO;

namespace RockPaperScissorsAdventure
{
    // Player class manages adventurer's stats and dynamite usage
    class Player
    {
        public string Name { get; set; } // Player's name
        public int HP { get; set; }     // Current health points
        public int Dynamite { get; set; } // Number of dynamite uses
        public int Wins { get; set; }   // Number of enemies defeated

        public Player(string name)
        {
            Name = name;
            HP = 50;       // Starting HP
            Dynamite = 2;  // Starting dynamite
            Wins = 0;      // Start with no wins
        }

        // Attempts to use dynamite, returns true if successful
        public bool UseDynamite()
        {
            if (Dynamite > 0)
            {
                Dynamite--;
                return true;
            }
            return false;
        }
    }

    // Enemy class handles enemy stats and move selection
    class Enemy
    {
        public string Name { get; set; } // Enemy's name
        public int HP { get; set; }      // Current health points
        public double RockBias { get; set; } // Bias for picking rock

        public Enemy(string name, int hp, double rockBias)
        {
            Name = name;
            HP = hp;
            RockBias = rockBias; // Higher rockBias means more likely to pick rock
        }

        // Randomly picks a move based on rock bias
        public string MakeChoice()
        {
            Random rand = new Random();
            double roll = rand.NextDouble(); // Random value between 0 and 1
            if (roll < RockBias) return "rock"; // E.g., 0.6 means 60% chance of rock
            if (roll < RockBias + (1 - RockBias) / 2) return "paper";
            return "scissors";
        }
    }

    class Game
    {
        static void Main(string[] args)
        {
            // Initialize player
            Player player = new Player("Adventurer");

            // List of enemies with increasing HP and rock bias
            List<Enemy> enemies = new List<Enemy>
            {
                new Enemy("Goblin", 20, 0.6), // 60% rock
                new Enemy("Wizard", 30, 0.4), // 40% rock
                new Enemy("Dark Lord", 40, 0.3) // 30% rock
            };

            Console.WriteLine("Welcome to Rock-Paper-Scissors Adventure!");

            // Fight each enemy in sequence
            foreach (Enemy currentEnemy in enemies)
            {
                Console.WriteLine("\nA wild " + currentEnemy.Name + " appears!");

                // Battle loop until player or enemy is defeated
                while (player.HP > 0 && currentEnemy.HP > 0)
                {
                    // Show current status
                    Console.WriteLine(player.Name + ": HP = " + player.HP + ", Dynamite = " + player.Dynamite);
                    Console.WriteLine(currentEnemy.Name + ": HP = " + currentEnemy.HP);

                    // Get valid player move
                    string playerChoice = GetPlayerMove(player);

                    // Handle dynamite or regular move
                    if (playerChoice == "dynamite")
                        Console.WriteLine("BOOM! Dynamite used!");
                    else
                        Console.WriteLine("You chose: " + playerChoice);

                    // Enemy picks a move
                    string enemyChoice = currentEnemy.MakeChoice();
                    Console.WriteLine(currentEnemy.Name + " chose: " + enemyChoice);

                    // Determine battle result
                    ProcessBattle(player, currentEnemy, playerChoice, enemyChoice);
                }

                // Check if player lost
                if (player.HP <= 0)
                {
                    Console.WriteLine("\nGame Over! You were defeated...");
                    SaveGameResults(player, false);
                    Console.ReadKey();
                    return;
                }

                // Player won against this enemy
                Console.WriteLine("\nVictory! You defeated the " + currentEnemy.Name + "!");
                player.Wins++;
                // Heal 15 HP, capped at 50
                player.HP = Math.Min(player.HP + 15, 50);
            }

            // All enemies defeated
            Console.WriteLine("\nYou saved the land! Wins: " + player.Wins);
            SaveGameResults(player, true);
            Console.ReadKey();
        }

        // Gets a valid move from the player
        static string GetPlayerMove(Player player)
        {
            while (true)
            {
                Console.Write("Choose your move (rock/paper/scissors/dynamite): ");
                string choice = Console.ReadLine()?.ToLower() ?? "";
                if (choice == "rock" || choice == "paper" || choice == "scissors")
                    return choice;
                if (choice == "dynamite" && player.UseDynamite())
                    return choice;
                if (choice == "dynamite")
                    Console.WriteLine("No dynamite left!");
                else
                    Console.WriteLine("Invalid move, try again!");
            }
        }

        // Processes battle outcome and updates HP
        static void ProcessBattle(Player player, Enemy enemy, string playerChoice, string enemyChoice)
        {
            if (playerChoice == "dynamite")
            {
                // Dynamite deals 25 damage
                enemy.HP -= 25;
                Console.WriteLine("Dynamite deals 25 damage to " + enemy.Name + "!");
            }
            else if (playerChoice == enemyChoice)
            {
                Console.WriteLine("It's a tie! No damage!");
            }
            else if ((playerChoice == "rock" && enemyChoice == "scissors") ||
                     (playerChoice == "paper" && enemyChoice == "rock") ||
                     (playerChoice == "scissors" && enemyChoice == "paper"))
            {
                // Player wins, deal 10 damage
                int damage = 10;
                enemy.HP -= damage;
                Console.WriteLine("You deal " + damage + " damage to " + enemy.Name + "!");
            }
            else
            {
                // Enemy wins, deal 8 damage
                int damage = 8;
                player.HP -= damage;
                Console.WriteLine(enemy.Name + " deals " + damage + " damage to you!");
            }
        }

        // Saves game results to a file
        static void SaveGameResults(Player player, bool won)
        {
            try
            {
                string result = "Game on " + DateTime.Now + "\n" +
                               "Player: " + player.Name + "\n" +
                               "Wins: " + player.Wins + "\n" +
                               "Final HP: " + player.HP + "\n" +
                               "Result: " + (won ? "Saved the land!" : "Defeated") + "\n\n";
                File.AppendAllText("results.txt", result);
                Console.WriteLine("Results saved to results.txt!");
            }
            catch (IOException e)
            {
                Console.WriteLine("Error saving results: " + e.Message);
            }
        }
    }
}
