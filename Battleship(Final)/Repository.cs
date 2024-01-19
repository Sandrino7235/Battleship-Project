using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleship_Final_
{
    internal class Repository
    {
        private BattleshipEntities repository;
        public string title;
        public string creator;
        public string opponent;
        private Dictionary<string, Score> scores = new Dictionary<string, Score>();

        public Repository(BattleshipEntities repository)
        {
            this.repository = repository;
        }

        public string SavePlayer(Player player)
        {
            repository.Players.Add(player);
            repository.SaveChanges();
            Console.WriteLine("Player details saved successfully.");
            return player.Username;
        }

        public bool UsernameLogin(string username)
        {
            if (repository.Players.Any(p => p.Username == username))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool PasswordLogin(string password)
        {
            if (repository.Players.Any(p => p.Password == password))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public Game CreateAndSaveGame(string username)
        {
            title = $"{username} vs {username}";
            int gameID = repository.Games.Any() ? repository.Games.Max(g => g.ID) + 1 : 1;
            creator = username;
            opponent = username;
            Game game = new Game(gameID, title, creator, opponent, true);
            repository.Games.Add(game);
            repository.SaveChanges();
            return game;
        }

        public GameShipConfiguration GameShipConfiguration1(string username1, int shipid, int coordinate)
        {
            int gameShipConfigurationID = repository.GameShipConfigurations.Any() ? repository.GameShipConfigurations.Max(g => g.ID) + 1 : 1;
            GameShipConfiguration gameShipConfiguration = new GameShipConfiguration(gameShipConfigurationID, username1, shipid, coordinate); //changed to shipid as multiple exceptions were thrown
            repository.GameShipConfigurations.Add(gameShipConfiguration);
            repository.SaveChanges();
            return gameShipConfiguration;
        }
        public GameShipConfiguration GameShipConfiguration2(string username2, int shipid, int coordinate)
        {
            int gameShipConfigurationID = repository.GameShipConfigurations.Any() ? repository.GameShipConfigurations.Max(g => g.ID) + 1 : 1;
            GameShipConfiguration gameShipConfiguration = new GameShipConfiguration(gameShipConfigurationID, username2, shipid, coordinate);
            repository.GameShipConfigurations.Add(gameShipConfiguration);
            repository.SaveChanges();
            return gameShipConfiguration;
        }
        
        public bool IsHit(string username, int coordinate)
        {
            if (repository.GameShipConfigurations.Any(g => g.PlayerFK == username && g.Coordinate == coordinate))
            {
                Console.WriteLine("Hit!");
            }
            else
            {
                Console.WriteLine("Miss!");
            }
            return repository.GameShipConfigurations.Any(g => g.PlayerFK == username && g.Coordinate == coordinate);
        }
     
            public void IncrementHits(string username)
            {
                if (!scores.ContainsKey(username))
                {
                    scores[username] = new Score { Username = username };
                }

                scores[username].Hits++;
            }

            public int GetHits(string username)
            {
                if (!scores.ContainsKey(username))
                {
                    return 0;
                }

                return scores[username].Hits;
            }

            public void IncrementMisses(string username)
            {
                if (!scores.ContainsKey(username))
                {
                    scores[username] = new Score { Username = username };
                }

                scores[username].Misses++;
            }

            public string GetWinner()
            {
                var winner = scores.Values.OrderByDescending(s => s.Hits).FirstOrDefault();
                return winner != null ? winner.Username : null;
            }
        
        /*public Attack Attack(int coordinate, bool hit)
        {
            int attackID = repository.Attacks.Any() ? repository.Attacks.Max(g => g.ID) + 1 : 1;
            Attack attack = new Attack(attackID, coordinate, hit, gameID);
            repository.Attacks.Add(attack);
            repository.SaveChanges();
            return attack;
        }*/

    }
}
