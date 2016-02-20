using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommandLine;

using SharedObjects;

using log4net;
using log4net.Config;
using System.Windows.Forms;

namespace Player
{
    class Program
    {
        class Options
        {
            [Option('p', Required = true, HelpText = "The Registry's communication endpoint")]
            //[ValueOption(0)]
            public string EndPoint { get; set; }

            [Option('f', Required = true, HelpText = "The Player's first name")]
            //[ValueOption(1)]
            public string FirstName { get; set; }

            [Option('l', Required = true, HelpText = "The Player's last name")]
            //[ValueOption(2)]
            public string LastName { get; set; }

            [Option('n', Required = true, HelpText = "The Player's A-number")]
            //[ValueOption(3)]
            public string ANumber { get; set; }

            [Option('a', Required = true, HelpText = "The Player's alias")]
            //[ValueOption(4)]
            public string Alias { get; set; }

            [HelpOption]
            public string GetUsage()
            {
                var usage = new StringBuilder();
                usage.AppendLine("MyDSoak 1.0");
                usage.AppendLine("-p: Registry's endpoint");
                usage.AppendLine("-f: Player's first name");
                usage.AppendLine("-l: Player's last name");
                usage.AppendLine("-n: Player's A-number");
                usage.AppendLine("-a: Player's alias");
                return usage.ToString();
            }
        }

        private static ILog Logger = LogManager.GetLogger(typeof(Program));

        [STAThread]
        static void Main(string[] args)
        {
            XmlConfigurator.Configure();
            Logger.Info("Starting up");

            Options options = new Options();

            if (Parser.Default.ParseArguments(args, options))
            {
                IdentityInfo info = new IdentityInfo()
                {
                    FirstName = options.FirstName,
                    LastName = options.LastName,
                    ANumber = options.ANumber,
                    Alias = options.Alias
                };

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                OldPlayer player = new OldPlayer(options.EndPoint, info);
                PlayerForm form = new PlayerForm();
                player.Subscribe(form);
                Application.Run(form);
            }
        }
    }
}
