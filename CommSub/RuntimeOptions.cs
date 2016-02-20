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

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this, curr => HelpText.DefaultParsingErrorsHandler(this, curr));
        }
    }
}
