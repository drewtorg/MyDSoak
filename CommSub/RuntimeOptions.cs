using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommandLine;
using CommandLine.Text;

namespace CommSub
{
    public abstract class RuntimeOptions
    {
        public abstract void SetDefaults();

        [Option('p', Required = true, HelpText = "The Registry's communication endpoint")]
        public string EndPoint { get; set; }

        [Option('f', Required = true, HelpText = "The Player's first name")]
        public string FirstName { get; set; }

        [Option('l', Required = true, HelpText = "The Player's last name")]
        public string LastName { get; set; }

        [Option('n', Required = true, HelpText = "The Player's A-number")]
        public string ANumber { get; set; }

        [Option('a', Required = true, HelpText = "The Player's alias")]
        public string Alias { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this, curr => HelpText.DefaultParsingErrorsHandler(this, curr));
        }
    }
}
