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
            Logger.Info("Starting up");

            PlayerOptions options = new PlayerOptions();

            if (Parser.Default.ParseArguments(args, options))
            {
                // Application.EnableVisualStyles();
                // Application.SetCompatibleTextRenderingDefault(false);

                Player player = new Player(options);
                player.Start();
                // PlayerForm form = new PlayerForm();
                // player.Subscribe(form);
                // Application.Run(form);
                while (player.Status == "Running") Thread.Sleep(0);
            }
        }
    }
}
