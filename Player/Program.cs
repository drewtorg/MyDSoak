using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommandLine;

using SharedObjects;

using CommSub;

using log4net;
using log4net.Config;
using System.Windows.Forms;
using System.Threading;

namespace Player
{
    class Program
    {
        private static ILog Logger = LogManager.GetLogger(typeof(Program));

        // [STAThread]
        static void Main(string[] args)
        {
            XmlConfigurator.Configure();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Logger.Info("Starting up");

            RuntimeOptions options = new PlayerOptions();

            if (Parser.Default.ParseArguments(args, options))
            {
                options.SetDefaults();

                Player player = new Player() { Options = options };
                PlayerForm form = new PlayerForm() { Player = player };
                Application.Run(form);
            }
        }
    }
}
