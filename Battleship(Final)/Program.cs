using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleship_Final_
{
    internal class Program
    {
        public static BattleshipEntities db = new BattleshipEntities();
        public static string username1;
        public static string username2;
        static Game currentGame;
        static void Main(string[] args)
        {
            int option; //to store user option
            
            do
            {
                DisplayMenu();
                option = int.Parse(Console.ReadLine());

                switch (option)
                {
                    case 1:
                        AddPlayer(); // save player details in repository
                        break;
                    case 2:
                        ConfigureShips(); // configure ships for both players
                        break;
                    case 3:
                        LaunchAttack(username1, currentGame); // launch attack for player 1 and player 2
                        break;
                    case 4:
                        Console.WriteLine("Quitting the game.");
                        break;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }

            } while (option != 4);
        }

        static void DisplayMenu()
        {
            Console.WriteLine("----- Game Menu -----");
            Console.WriteLine("1. Add Player Details");
            Console.WriteLine("2. Configure Ships");
            Console.WriteLine("3. Launch Attack");
            Console.WriteLine("4. Quit");
            Console.Write("Enter your choice: ");
        }

        static void AddPlayer()
        {
            Repository repository = new Repository(db);

            for (int p = 1; p <= 2; p++)
            {
                Console.WriteLine($"Player {p} ");
                Console.Write("Enter Username: ");
                string username = Console.ReadLine();

                Console.Write("Password: ");
                string password = GetMaskedPassword();
                if (repository.UsernameLogin(username) == true)
                {
                    if (repository.PasswordLogin(password) == true)
                    {
                        Console.Write("Login Successful");
                        Console.WriteLine();
                    }
                    else
                    {
                        Console.Write("Login Unsuccessful");
                        Console.WriteLine();
                        break;
                    }
                    Console.WriteLine();
                }
                else
                {
                    Console.WriteLine("Account unidentified");
                    Console.WriteLine($"Player {p}: Create New Player");

                    Console.WriteLine("Enter Username: ");
                    string newUsername = Console.ReadLine();
                    Console.WriteLine("Password: ");
                    string newPassword = GetMaskedPassword();

                    Player player = new Player(newUsername, newPassword);
                    repository.SavePlayer(player);
                }
                if (p == 1)
                {
                    username1 = username; //store username of player 1
                }
                else
                {
                    username2 = username; //store username of player 2
                }
                repository.CreateAndSaveGame(username);
            }
        }

        static string GetMaskedPassword()
        {
            StringBuilder password = new StringBuilder();
            ConsoleKeyInfo key;

            do
            {
                key = Console.ReadKey(true);

                if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
                {
                    password.Append(key.KeyChar);
                    Console.Write("*");
                }
                else if (key.Key == ConsoleKey.Backspace && password.Length > 0)
                {
                    password.Remove(password.Length - 1, 1);
                    Console.Write("\b \b");
                }
            } while (key.Key != ConsoleKey.Enter);

            Console.WriteLine();

            return password.ToString();
        }

        static void ConfigureShips()
        {
            int[] shipSizes = new int[] { 2, 3, 3, 4, 5 }; //to store ship sizes
            int[,] shipLocations1 = new int[8, 8];
            int[,] shipLocations2 = new int[8, 8];

            // Configure ships for both players
            ConfigurePlayerShips(username1, shipSizes, shipLocations1);
            ConfigurePlayerShips(username2, shipSizes, shipLocations2);
        }

        static void ConfigurePlayerShips(string playerName, int[] shipSizes, int[,] shipLocations)
        {
            Console.WriteLine($"{playerName} configure ships:"); //to display player number
            Console.WriteLine("(1) Destroyer = 2  |  (2) Submarine = 3  |  (3) Cruiser = 3  |  (4) Battleship = 4  |  (5) Carrier = 5");

            DisplayGrid();

            for (int j = 1; j <= 5; j++) //to loop through ships
            {
                Console.WriteLine($"Configuring ship {j} for Player {playerName}."); //to display ship number
                int coordinate;
                int shipId;
                string orientation;
                bool isValidPlacement;

                do
                {
                    Console.Write("Choose Ship (1-5): ");
                    shipId = int.Parse(Console.ReadLine());
                } while (shipId < 1 || shipId > 5); //to check if ship id is valid

                do
                {
                    isValidPlacement = true;

                    Console.Write($"Enter starting coordinate for ship (1-64): ");
                    coordinate = int.Parse(Console.ReadLine());

                    Repository repository = new Repository(db);
                    repository.GameShipConfiguration1(playerName, shipId, coordinate);
                    repository.GameShipConfiguration2(playerName, shipId, coordinate);


                    Console.Write("Choose orientation (V for vertical, H for horizontal): ");
                    orientation = Console.ReadLine();

                    int row = (coordinate - 1) / 8; //to calculate row
                    int col = (coordinate - 1) % 8; //to calculate column

                    // Validate ship's orientation
                    if ((orientation.ToUpper() == "V" && row + shipSizes[shipId - 1] > 8) ||
                        (orientation.ToUpper() == "H" && col + shipSizes[shipId - 1] > 8))
                    {
                        Console.WriteLine("Invalid ship placement. Ships must be placed within the grid.");
                        isValidPlacement = false;
                        continue;
                    }

                    for (int k = 0; k < shipSizes[shipId - 1]; k++)
                    {
                        if (shipLocations[row, col] != 0) //to check if ship location is empty
                        {
                            Console.WriteLine("Invalid ship placement. Ships cannot overlap.");
                            isValidPlacement = false;
                            break;
                        }

                        if (orientation.ToUpper() == "V")
                        {
                            row++;
                        }
                        else
                        {
                            col++;
                        }
                    }

                } while (!isValidPlacement);

                int rowStart = (coordinate - 1) / 8; //to calculate row
                int colStart = (coordinate - 1) % 8; //to calculate column

                for (int k = 0; k < shipSizes[shipId - 1]; k++)
                {
                    shipLocations[rowStart, colStart] = shipId; //to store ship id in ship locations

                    if (orientation.ToUpper() == "V")
                    {
                        rowStart++;
                    }
                    else
                    {
                        colStart++;
                    }
                }

                Console.WriteLine($"Ship {j} for Player {playerName} configured.");
            }
            UpdatedGrid(shipLocations); //to display updated grid
        }
        static void DisplayGrid()
        {
            int[,] grid = new int[8, 8];
            int counter = 1;

            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    grid[x, y] = counter;
                    Console.Write($"|{counter.ToString().PadLeft(2, '0')}|");
                    counter++;
                }
                Console.WriteLine();
            }

        }
        static void UpdatedGrid(int[,] shipLocations)
        {
            int counter = 1;

            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    if (shipLocations[x, y] != 0) //to check if ship location is not empty
                    {
                        Console.Write("|XX|");
                    }
                    else
                    {
                        shipLocations[x, y] = counter; //to store counter in ship locations
                        Console.Write($"|{counter.ToString().PadLeft(2, '0')}|");
                    }
                    counter++;
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        static void LaunchAttack(string currentUsername, Game currentGame)
        {
            Repository repository = new Repository(db);
            string opponentUsername = currentUsername == username1 ? username2 : username1; // Get the opponent's username

            // Initialize a 2D array to store the attack results
            string[,] attackResults = new string[8, 8];
            HashSet<int> attackedCoordinates = new HashSet<int>();

            Console.WriteLine();
            Console.WriteLine($"{currentUsername}, launch your attack.");
            DisplayGrid();

            while (true) // Loop until a player reaches 17 hits or user decides to finish
            {
                Console.Write("Enter attack coordinate (1-64) or 0 to finish: ");
                string input = Console.ReadLine(); // Store user input

                if (!int.TryParse(input, out int attackCoordinate) || attackCoordinate < 0 || attackCoordinate > 64) // Check if input is valid
                {
                    Console.WriteLine("Invalid input. Please enter a number between 1 and 64, or 0 to finish.");
                    continue;
                }

                if (attackCoordinate == 0) // Check if user wants to finish
                {
                    break;
                }
                // Check if the coordinate has already been attacked
                if (attackedCoordinates.Contains(attackCoordinate))
                {
                    Console.WriteLine("You have already attacked this coordinate. Please enter a different coordinate.");
                    continue;
                }

                // Add the coordinate to the attacked coordinates
                attackedCoordinates.Add(attackCoordinate);

                // Calculate the row and column indices in the array
                int attackX = (attackCoordinate - 1) / 8;
                int attackY = (attackCoordinate - 1) % 8;

                // Check if the attack coordinate hits a ship
                if (repository.IsHit(opponentUsername, attackCoordinate)) // Check if ship is hit
                {
                    repository.IncrementHits(currentUsername);
                    //repository.Attack(attackCoordinate, true, currentGame);

                    // Check if the current player has reached 17 hits
                    if (repository.GetHits(currentUsername) >= 17)
                    {
                        break;
                    }

                    attackResults[attackX, attackY] = "|HH|";
                }
                else
                {
                    repository.IncrementMisses(currentUsername);
                    attackResults[attackX, attackY] = "|MM|";
                }

                // Display the updated grid
                DisplayUpdatedGrid(attackResults);

                // Display the score
                Console.WriteLine($"{username1} {repository.GetHits(username1)} : {username2} {repository.GetHits(username2)}");
                // Switch to the next player
                string temp = currentUsername;
                currentUsername = opponentUsername;
                opponentUsername = temp;
                Console.WriteLine($"{currentUsername}, launch your attack.");
                
            }

            // Game over
            string winner = repository.GetWinner(); // Get the winner
            Console.WriteLine($"{winner} wins the game!");
        }

        static void DisplayUpdatedGrid(string[,] attackResults)
        {
            int counter = 1;

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    // If the cell is null, display the counter, otherwise display the cell value
                    Console.Write(attackResults[i, j] ?? $"|{counter.ToString().PadLeft(2, '0')}|");
                    counter++;
                }
                Console.WriteLine();
            }
        }
    }
}
