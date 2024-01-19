using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleship_Final_
{
    internal class Constructors
    {
    }
    partial class Attack
    {
        public Attack(int ID, int Coordinate, bool Hit, int gameID)
        {
            this.ID = ID;
            this.Coordinate = Coordinate;
            this.Hit = Hit;
            this.GameFK = gameID;
        }
        public virtual Game Game { get; set; }

    }

    partial class Game
    {
        public Game(int ID, string Title, string CreatorFK, string OpponentFK, bool Complete)
        {
            this.ID = ID;
            this.Title = Title;
            this.CreatorFK = CreatorFK;
            this.OpponentFK = OpponentFK;
            this.Complete = Complete;
        }
    }

    partial class GameShipConfiguration
    {
        public GameShipConfiguration(int ID, string PlayerFK, int Ship, int Coordinate)
        {
            this.ID = ID;
            this.PlayerFK = PlayerFK;
            this.Ship = Ship;
            this.Coordinate = Coordinate;
        }
    }
    partial class Player
    {
        public Player(string Username, string Password)
        {
            this.Username = Username;
            this.Password = Password;
        }
    }

    partial class Ship
    {
        public Ship(int ID, string Title, int Size)
        {
            this.ID = ID;
            this.Title = Title;
            this.Size = Size;
        }
    }
}
