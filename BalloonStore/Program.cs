using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

using log4net;
using log4net.Config;
using CommandLine;

namespace BalloonStore
{
    static class Program
    {
        private static ILog Logger = LogManager.GetLogger(typeof(Program));

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            XmlConfigurator.Configure();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Logger.Info("Starting up");

            BalloonStoreOptions options = new BalloonStoreOptions();

            if (Parser.Default.ParseArguments(args, options))
            {
                options.SetDefaults();

                BalloonStore store = new BalloonStore() { Options = options };
                BalloonStoreForm form = new BalloonStoreForm() { BalloonStore = store };
                Application.Run(form);
            }
        }
    }
}
