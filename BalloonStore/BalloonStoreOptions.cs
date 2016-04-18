using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommandLine;
using CommandLine.Text;

namespace BalloonStore
{
    public class BalloonStoreOptions
    {
        [Option("gmid", MetaValue = "INT", Required = true, HelpText = "Game manager process id")]
        public int GameManagerId { get; set; }

        [Option("gameid", MetaValue = "INT", Required = true, HelpText = "Game id")]
        public int GameId { get; set; }

        [Option("balloons", MetaValue = "INT", Required = true, HelpText = "Number of balloons to make available")]
        public int Balloon { get; set; }

        [Option("registry", MetaValue = "STRING", Required = true, HelpText = "End Point for the registry")]
        public string RegistryEndPoint { get; set; }

        [Option("storeindex", MetaValue = "INT", Required = true, HelpText = "A number to be used in forming a label for the balloon store")]
        public int StoreIndex { get; set; }

        [Option("minport", MetaValue = "INT", Required = false, HelpText = "Min port in a range of possible ports for this process's communications")]
        public int? MinPortNullable { get; set; }
        public int MinPort { get { return (MinPortNullable == null) ? 0 : (int)MinPortNullable; } }

        [Option("maxport", MetaValue = "INT", Required = false, HelpText = "Max port in a range of possible ports for this process's communications")]
        public int? MaxPortNullable { get; set; }
        public int MaxPort { get { return (MaxPortNullable == null) ? 0 : (int)MaxPortNullable; } }

        [Option("timeout", MetaValue = "INT", Required = false, HelpText = "Default timeout for request-reply communications")]
        public int? TimeoutNullable { get; set; }
        public int Timeout { get { return (TimeoutNullable == null) ? 0 : (int)TimeoutNullable; } }

        [Option('s', "autostart", Required = false, HelpText = "Autostart")]
        public bool AutoStart { get; set; }

        [Option("retries", MetaValue = "INT", Required = false, HelpText = "Default max retries for request-reply communications")]
        public int? RetriesNullable { get; set; }
        public int Retries { get { return (RetriesNullable == null) ? 0 : (int)RetriesNullable; } }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this, current => HelpText.DefaultParsingErrorsHandler(this, current));
        }

        public void SetDefaults()
        {
            if (MinPortNullable == null)
                MinPortNullable = Properties.Settings.Default.MinPort;
            if (MaxPortNullable == null)
                MaxPortNullable = Properties.Settings.Default.MaxPort;
            if (TimeoutNullable == null)
                TimeoutNullable = Properties.Settings.Default.Timeout;
            if (RetriesNullable == null)
                RetriesNullable = Properties.Settings.Default.MaxRetries;
        }
    }
}
