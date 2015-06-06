using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCentralStation
{
    static class Downloads
    {
        private static List<DownloadingGame> games = new List<DownloadingGame>();




        public static bool hasGame(Game game)
        {
            return false;
        }
    }

    public class DownloadingGame {
        private int id;
        private double progress;

        

    }
}
