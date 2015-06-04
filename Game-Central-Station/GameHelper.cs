using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCentralStation
{
    public class GameContract
    {
        public string id { get; set; }

        public string name { get; set; }

        public string executableName { get; set; }

        public string versionString { get; set; }

        public string zipLength { get; set; }

        public string ready { get; set; }

        public string archived { get; set; }

        public string uploadTimeStamp { get; set; }

        public string idGroup { get; set; }
    }

    public class Game
    {
        public class HeaderGame : Game
        {
            public HeaderGame()
                : base(null)
            {
            }


            public override string ToString()
            {
                return "Version\tStatus\tName";
            }
        }

        public static Game headerGame = new HeaderGame();

        public string id;
        public int versionInteger;
        public string version;
        public string name;
        public string executableName;
        public string displayName;
        public int zipLength;
        public bool ready;
        public bool archived;
        public string uploadTimeStamp;
        public int idGroup;

        private Game(GameContract contract)
        {
            if (contract != null)
            {
                id = contract.id;
                versionInteger = Int32.Parse(contract.versionString);
                name = contract.name;
                executableName = contract.executableName;
                ready = Boolean.Parse(contract.ready);
                archived = Boolean.Parse(contract.archived);
                zipLength = Int32.Parse(contract.zipLength);
                displayName = name.Replace("&", "&&");
                version = "" + versionInteger;
                this.uploadTimeStamp = contract.uploadTimeStamp;
                this.idGroup = Int32.Parse(contract.idGroup);
            }
        }

        public static Game getGame(GameContract contract)
        {
            try
            {
                Game game = new Game(contract);
                return game;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public override string ToString()
        {
            return versionInteger + "\t" + (archived ? "Archived" : (ready ? "Published" : "Corrupt")) + "\t" + name;
        }

    }
}
