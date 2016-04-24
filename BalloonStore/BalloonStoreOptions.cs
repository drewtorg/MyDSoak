using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommSub;

using CommandLine;
using CommandLine.Text;

namespace BalloonStore
{
    public class BalloonStoreOptions : RuntimeOptions
    {
        [Option("gmid", MetaValue = "INT", Required = true, HelpText = "Game manager process id")]
        public int GameManagerId { get; set; }

        [Option("gameid", MetaValue = "INT", Required = true, HelpText = "Game id")]
        public int GameId { get; set; }

        [Option("balloons", MetaValue = "INT", Required = true, HelpText = "Number of balloons to make available")]
        public int NumBalloons { get; set; }

        [Option("storeindex", MetaValue = "INT", Required = true, HelpText = "A number to be used in forming a label for the balloon store")]
        public int StoreIndex { get; set; }

        [Option("gmep", MetaValue = "STRING", Required = false, HelpText = "The end point for the game manager")]
        public string GameManagerEndPoint { get; set; }

        public override void SetDefaults()
        {
            if (string.IsNullOrWhiteSpace(ANumber))
                ANumber = Properties.Settings.Default.ANumber;
            if (string.IsNullOrWhiteSpace(FirstName))
                FirstName = Properties.Settings.Default.FirstName;
            if (string.IsNullOrWhiteSpace(LastName))
                LastName = Properties.Settings.Default.LastName;
            if (string.IsNullOrWhiteSpace(Alias))
                Alias = Properties.Settings.Default.Alias;
            if (MinPortNullable == null)
                MinPortNullable = Properties.Settings.Default.MinPort;
            if (MaxPortNullable == null)
                MaxPortNullable = Properties.Settings.Default.MaxPort;
            if (TimeoutNullable == null)
                TimeoutNullable = Properties.Settings.Default.Timeout;
            if (RetriesNullable == null)
                RetriesNullable = Properties.Settings.Default.MaxRetries;
            if (string.IsNullOrWhiteSpace(Label))
                Label = string.Format("{0}'s Player", Alias);
        }
    }
}
