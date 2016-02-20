using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommSub;

using CommandLine;
using CommandLine.Text;

namespace Player
{
    public class PlayerOptions : RuntimeOptions
    {
        [Option('f', Required = true, HelpText = "The Player's first name")]
        public string FirstName { get; set; }

        [Option('l', Required = true, HelpText = "The Player's last name")]
        public string LastName { get; set; }

        [Option('n', Required = true, HelpText = "The Player's A-number")]
        public string ANumber { get; set; }

        [Option('a', Required = true, HelpText = "The Player's alias")]
        public string Alias { get; set; }
        
        public override void SetDefaults()
        {
            EndPoint = "127.0.0.1:8080";
            FirstName = "Andrew";
            LastName = "Torgeson";
            ANumber = "A01519794";
            Alias = "alt";
        }
    }
}
